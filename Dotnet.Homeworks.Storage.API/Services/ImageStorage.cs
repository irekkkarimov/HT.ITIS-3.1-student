using System.Reactive.Linq;
using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Constants;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucket;

    public ImageStorage(IMinioClient minioClient, string bucket)
    {
        _minioClient = minioClient;
        _bucket = bucket;
    }

    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        try
        {
            item.Metadata.Add(MetadataKeys.Destination, _bucket);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(Buckets.Pending)
                .WithObject(item.FileName)
                .WithStreamData(item.Content)
                .WithObjectSize(item.Content.Length)
                .WithHeaders(item.Metadata)
                .WithContentType(item.ContentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    }

    public async Task<Image?> GetItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var downloadStream = new MemoryStream(); 
            var objectStat = await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(itemName)
                .WithCallbackStream(async (stream, ct) =>
                {
                    await stream.CopyToAsync(downloadStream, ct);
                }), cancellationToken);

            return new Image(downloadStream, objectStat.ObjectName, objectStat.ContentType, objectStat.MetaData);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Result> RemoveItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(itemName), cancellationToken);

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    }

    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(CancellationToken cancellationToken = default)
    {
        var listObjectArgs = new ListObjectsArgs().WithBucket(_bucket);
        
        var items = await _minioClient.ListObjectsAsync(listObjectArgs, cancellationToken)
            .Select(x => x.Key)
            .ToList();

        return items;
    }

    public async Task<Result> CopyItemToBucketAsync(string itemName, string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new CopyObjectArgs()
                .WithCopyObjectSource(new CopySourceObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(itemName))
                .WithBucket(destinationBucketName);
            
            await _minioClient.CopyObjectAsync(args, cancellationToken);

            return new Result(true);
        }
        catch (Exception e)
        {
            return new Result(false, e.Message);
        }
    }
}