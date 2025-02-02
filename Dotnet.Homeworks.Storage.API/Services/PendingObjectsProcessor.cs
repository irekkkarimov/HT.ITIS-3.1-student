using Dotnet.Homeworks.Storage.API.Constants;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private readonly IStorageFactory _storageFactory;

    public PendingObjectsProcessor(IStorageFactory storageFactory)
    {
        _storageFactory = storageFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pendingStorage = await _storageFactory.CreateImageStorageWithinBucketAsync(Buckets.Pending);

        while (true)
        {
            var pendingItems = await pendingStorage.EnumerateItemNamesAsync(stoppingToken);
            
            foreach (var pendingItem in pendingItems)
            {
                var item = await pendingStorage.GetItemAsync(pendingItem, stoppingToken);

                if (item!.Metadata.TryGetValue(MetadataKeys.Destination, out var destinationBucket))
                {
                    await pendingStorage.CopyItemToBucketAsync(pendingItem, destinationBucket, stoppingToken);
                }

                await pendingStorage.RemoveItemAsync(pendingItem, stoppingToken);
            }

            await Task.Delay(PendingObjectProcessor.Period, stoppingToken);
        }
    }
}