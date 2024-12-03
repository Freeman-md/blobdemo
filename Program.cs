using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;


BlobServiceClient GetBlobServiceClient()
{
    try
    {
        // Initialize BlobServiceClient with DefaultAzureCredential
        BlobServiceClient client = new BlobServiceClient(
            new Uri("https://blobdemostorageaccount.blob.core.windows.net"),
            new ChainedTokenCredential(new EnvironmentCredential(), new AzureCliCredential())
        );

        // Attempt to list the containers in the Blob Storage account
        Console.WriteLine("Testing connection to Azure Blob Storage...");
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

BlobServiceClient blobServiceClient = GetBlobServiceClient();

ContainerService containerService = new ContainerService(blobServiceClient);

string containerName = "projects";
BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

BlobService blobService = new BlobService(blobContainerClient);

await blobService.UploadTextBlobWithPrefixAsync();

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