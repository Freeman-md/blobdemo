using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

BlobServiceClient? client = await GetBlobServiceClient();

string containerName = "container-2829bc18-6549-4279-95c2-2c991eae6a3a";

// await ContainerService.DeleteSamepleContainerAsync(client, "container-bf44142f-5e1a-4d67-aa31-07b594523017");

// await ContainerService.ListContainers(client, "", 2);

// BlobContainerClient blobContainerClient = await ContainerService.CreateBlobContainer(client);
BlobContainerClient blobContainerClient = client.GetBlobContainerClient(containerName);

// Blob Lease Client
// await ContainerService.LeaseWorkflow(blobContainerClient);

await ContainerService.ManageProjectContainersAsync(blobContainerClient);

async Task<BlobServiceClient?> GetBlobServiceClient()
{
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

        return client;
    }
    catch (Exception ex)
    {
        // Catch and display any errors
        Console.WriteLine("Failed to connect to Azure Blob Storage.");
        Console.WriteLine($"Error: {ex.Message}");
    }

    return null;
}