using System.Diagnostics.CodeAnalysis;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;


ContainerService containerService = new ContainerService();

string containerName = "container-2829bc18-6549-4279-95c2-2c991eae6a3a";

BlobContainerClient blobContainerClient = containerService.blobServiceClient.GetBlobContainerClient(containerName);

// await containerService.ManageProjectContainersAsync(blobContainerClient);

BlobService blobService = new BlobService(blobContainerClient);