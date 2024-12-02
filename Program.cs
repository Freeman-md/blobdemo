using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;


// BlobServiceClient GetBlobServiceClient()
// {
//     try
//     {
//         // Initialize BlobServiceClient with DefaultAzureCredential
//         BlobServiceClient client = new BlobServiceClient(
//             new Uri("https://blobdemostorageaccount.blob.core.windows.net"),
//             new DefaultAzureCredential()
//         );

//         // Attempt to list the containers in the Blob Storage account
//         Console.WriteLine("Testing connection to Azure Blob Storage...");
//         Console.WriteLine("Connection successful!");

//         return client;
//     }
//     catch (Exception ex)
//     {
//         // Catch and display any errors
//         Console.WriteLine("Failed to connect to Azure Blob Storage.");
//         Console.WriteLine($"Error: {ex.Message}");
//     }

//     return null;
// }

// BlobServiceClient blobServiceClient = GetBlobServiceClient();

// ContainerService containerService = new ContainerService(blobServiceClient);

// string containerName = "container-e55355af-d14a-481b-ae97-112b65ca02bc";
// BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

// BlobService blobService = new BlobService(blobContainerClient);

// try
// {
//     BlobContainerSasPermissions permissions = BlobContainerSasPermissions.Read | BlobContainerSasPermissions.List;
//     DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddHours(1);

//     Uri sasUri = await containerService.GenerateUserDelegationSasToken(blobServiceClient, blobContainerClient, permissions, expiresOn);

//     Console.WriteLine($"Generated SAS URI: {sasUri}");

//     BlobContainerClient containerClientSAS = new BlobContainerClient(sasUri);

//     await blobService.ListBlobs(containerClientSAS); // should throw an error because only write permissions are allowed
// }
// catch (System.Exception ex)
// {
//     Console.WriteLine($"An error occurred: {ex.Message}");
// }

BlobContainerClient containerClientSas = new BlobContainerClient(new Uri("https://blobdemostorageaccount.blob.core.windows.net/container-e55355af-d14a-481b-ae97-112b65ca02bc?skoid=e0be665e-085c-44ed-99fb-de4714a3320a&sktid=35344dff-83d1-4e5b-ae26-5496f73fb500&skt=2024-12-02T13%3A05%3A20Z&ske=2024-12-03T13%3A05%3A20Z&sks=b&skv=2025-01-05&sv=2025-01-05&st=2024-12-02T13%3A05%3A20Z&se=2024-12-02T14%3A05%3A20Z&sr=c&sp=rl&sig=vkozMEvuT%2B%2FtztED5m9OQ%2BR8V7%2FtU779iN3OGXv%2Fdv4%3D"));

Console.WriteLine("Listing blobs in the container:");
await foreach (var blobItem in containerClientSas.GetBlobsAsync())
{
    Console.WriteLine($"Blob Name: {blobItem.Name}");
}