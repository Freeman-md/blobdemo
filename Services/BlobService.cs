using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobService
{
    private readonly BlobContainerClient _blobContainerClient;
    public BlobService(BlobContainerClient containerClient)
    {
        _blobContainerClient = containerClient;
    }

    public async Task<(string, string)?> UploadTextBlobWithPrefixAsync()
    {
        try
        {
            string localPath = "project-" + (new Random()).Next(1, 11); // Prefix for "folders"
            string fileName = "file-" + (new Random()).Next(1, 11) + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            Directory.CreateDirectory(localPath);
            await File.WriteAllTextAsync(localFilePath, fileName);

            // Create a prefix for the blob (e.g., project-1/file-1.txt)
            string blobName = $"{localPath}/{fileName}";

            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            Console.WriteLine($"Uploading to Blob Storage with prefix:\n\t{blobClient.Uri}");

            using FileStream fileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            IDictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "status", "in-progress" },
                { "category", localPath }
            };
            await blobClient.SetMetadataAsync(metadata);

            Console.WriteLine($"Upload for {0} completed successfully.", localFilePath);

            return (blobName, localFilePath);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");

            return null;
        }
    }

    public async Task DownloadBlob(string blobName, string localFilePath) {
        string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

        Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

        try {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            await blobClient.DownloadToAsync(downloadFilePath);
        } catch (RequestFailedException ex) {
            Console.WriteLine($"Request failed: {ex.Message}");
        }
    }


    public async Task GetBlobsInContainer(BlobContainerClient blobContainerClient)
    {
        try
        {
            await foreach (var blob in blobContainerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blob.Name);
            }
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task GetBlobsInContainer(BlobContainerClient blobContainerClient, string prefix)
    {
        try
        {
            string exactPrefix = prefix.EndsWith("/") ? prefix : prefix + "/";

            Console.WriteLine($"Blobs with prefix '{exactPrefix}':");

            await foreach (var blobItem in blobContainerClient.GetBlobsAsync(prefix: exactPrefix))
            {
                Console.WriteLine($"Blob Name: {blobItem.Name}");

                BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                BlobProperties properties = await blobClient.GetPropertiesAsync();

                Console.WriteLine("Metadata:");
                foreach (var metadata in properties.Metadata)
                {
                    Console.WriteLine($"\tKey: {metadata.Key}, Value: {metadata.Value}");
                }
            }
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Azure Request Failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async Task GetBlobsByMetaData(BlobContainerClient blobContainerClient, string key, string value)
    {
        Console.WriteLine($"Blobs with metadata '{key}={value}':");

        await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
            BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

            if (blobProperties.Metadata.ContainsKey(key) && blobProperties.Metadata[key] == value)
            {
                Console.WriteLine(blobItem.Name);

            }
        }

    }
}