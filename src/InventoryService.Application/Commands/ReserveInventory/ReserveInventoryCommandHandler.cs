using InventoryService.Application.Interfaces;
using InventoryService.Domain.Exceptions;
using InventoryService.Domain.Events;
using Microsoft.Extensions.Logging;

public sealed class ReserveInventoryCommandHandler
{
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IProcessedMessageRepository _processedMessageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitofWorkRepository _unitOfWorkRepository;
    private readonly ILogger<ReserveInventoryCommandHandler> _logger;
    public ReserveInventoryCommandHandler(IOutboxMessageRepository outboxMessageRepository, IProcessedMessageRepository processedMessageRepository, IProductRepository productRepository, IUnitofWorkRepository unitOfWorkRepository, ILogger<ReserveInventoryCommandHandler> logger)
    {
        _outboxMessageRepository = outboxMessageRepository;
        _processedMessageRepository = processedMessageRepository;
        _productRepository = productRepository;
        _unitOfWorkRepository = unitOfWorkRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ReserveInventoryCommand command, CancellationToken cancellationToken)
    {
        //Check for already processed message and if yes exit gracefully. This will help in achieving idempotency in case of message redelivery.
        bool isAlreadyProcessed = await _processedMessageRepository.ExistsAsync(command.MessageId, cancellationToken);
        if (isAlreadyProcessed)
        {
            _logger.LogWarning("Message {MessageId} has already been processed.", command.MessageId);
            return;
        }
        //Remove duplicate items for same product.
        List<ReserveInventoryItem> items = command.Items.GroupBy(i => i.ProductId).Select(g => new ReserveInventoryItem(g.Key, g.Sum(i => i.Quantity))).ToList();
        
        List<int> ids = items.Select(i => i.ProductId).ToList();

        //To get the products for the given product ids.
        List<Product> products = await _productRepository.GetByIdsAsync(ids, cancellationToken);

        Dictionary<int, Product> productDictionary = products.ToDictionary(p => p.ProductId);

        var missingProducts = items.Where(i => !productDictionary.ContainsKey(i.ProductId)).Select(i => i.ProductId).ToList();

        if (missingProducts.Any())
        {
            _logger.LogWarning("Some products are not found for the given IDs.");
            var failedEvt = new InventoryReservedFailedEvent
            {
                MessageId = command.MessageId,
                OrderNumber = command.OrderNumber,
                Reason = $"Some products are not found for the given IDs: {string.Join(", ", missingProducts)}"
            };

            await HandleFailureAsync(command.MessageId, failedEvt, cancellationToken);

            return;
        }

        //To validate the inventory for the items.
        try
        {
            foreach (var item in items)
            {
                productDictionary[item.ProductId].EnsureCanReserve(item.Quantity);
            }
        }
        catch (InsufficientStockException ex)
        {
            _logger.LogError(ex, "Insufficient stock for product {ProductId}. Reserved quantity: {ReservedQuantity}, Available quantity: {AvailableQuantity}.", ex.ProductId, ex.ReservedQuantity, ex.AvailableQuantity);
            var failedEvt = new InventoryReservedFailedEvent
            {
                MessageId = command.MessageId,
                OrderNumber = command.OrderNumber,
                Reason = ex.Message
            };

            await HandleFailureAsync(command.MessageId, failedEvt, cancellationToken);

            return;
        }

        //To reserve the inventory for the items.
        foreach (var item in items)
        {
            productDictionary[item.ProductId].Reserve(item.Quantity);
        }

        //If no exception above then mark the message as processed.
        await _processedMessageRepository.AddAsync(ProcessedMessage.Create(command.MessageId), cancellationToken);

        var evt = new InventoryReservedEvent
        {
            MessageId = command.MessageId,
            OrderNumber = command.OrderNumber,
            CustomerId = command.CustomerId,
            Amount = command.Amount
        };
        
        await _outboxMessageRepository.AddAsync(OutboxMessage.Create(evt), cancellationToken);

        _logger.LogInformation("Inventory reserved successfully for OrderNumber: {OrderNumber}, MessageId: {MessageId}", command.OrderNumber, command.MessageId);

       //if everything above is successful then save the changes to the database.
        await _unitOfWorkRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task HandleFailureAsync(Guid messageId, InventoryReservedFailedEvent evt, CancellationToken cancellationToken)
    {
        await _processedMessageRepository.AddAsync(ProcessedMessage.Create(messageId), cancellationToken);

        await _outboxMessageRepository.AddAsync(OutboxMessage.Create(evt), cancellationToken);

        await _unitOfWorkRepository.SaveChangesAsync(cancellationToken);
    }
        
}