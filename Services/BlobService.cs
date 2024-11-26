using Azure;
using Azure.Storage.Blobs;

public class BlobService
{
    private readonly BlobContainerClient _blobContainerClient;
    public BlobService(BlobContainerClient containerClient)
    {
        _blobContainerClient = containerClient;
    }

    public async Task UploadBlobToContainer(string fileName, string localFilePath)
    {
        try
        {
            // Get a reference to a blob
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            await blobClient.UploadAsync(localFilePath, true);
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task UploadTextBlobToContainer()
    {
        string localPath = "data";
        Directory.CreateDirectory(localPath);
        string fileName = "quickstart-" + Guid.NewGuid().ToString() + ".txt";
        string localFilePath = Path.Combine(localPath, fileName);

        // Write text to the file
        await File.WriteAllTextAsync(localFilePath, "Hello, World");

        await UploadBlobToContainer(fileName, localFilePath);
    }
}