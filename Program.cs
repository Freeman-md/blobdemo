using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

try
{
    // Initialize BlobServiceClient with DefaultAzureCredential
    BlobServiceClient client = new BlobServiceClient(
        new Uri("https://blobdemostorageaccount.blob.core.windows.net"),
        new DefaultAzureCredential()
    );

    // Attempt to list the containers in the Blob Storage account
    Console.WriteLine("Testing connection to Azure Blob Storage...");
    await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
    {
        Console.WriteLine($"Found container: {container.Name}");
    }

    Console.WriteLine("Connection successful!");
}
catch (Exception ex)
{
    // Catch and display any errors
    Console.WriteLine("Failed to connect to Azure Blob Storage.");
    Console.WriteLine($"Error: {ex.Message}");
}