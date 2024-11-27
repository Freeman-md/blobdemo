﻿using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;


BlobServiceClient GetBlobServiceClient()
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

string containerName = "container-e55355af-d14a-481b-ae97-112b65ca02bc";

BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

BlobService blobService = new BlobService(blobContainerClient);

await blobService.GetBlobsInContainer(blobContainerClient, "project-1");

// await blobService.UploadTextBlobWithPrefixAsync();