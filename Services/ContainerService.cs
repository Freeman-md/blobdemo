using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

public class ContainerService
{

    public static async Task<BlobContainerClient> CreateBlobContainer(BlobServiceClient blobServiceClient)
    {
        string containerName = "container-" + Guid.NewGuid();

        try
        {
            BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

            if (await container.ExistsAsync())
            {
                Console.WriteLine("Created container {0}", container.Name);
                return container;
            }
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine("HTTP error code {0}: {1}",
                                e.Status, e.ErrorCode);
            Console.WriteLine(e.Message);
        }

        return null;

    }

    public static void CreateRootContainer(BlobServiceClient blobServiceClient)
    {
        try
        {
            BlobContainerClient container = blobServiceClient.CreateBlobContainer("$root");

            if (container.Exists())
            {
                Console.WriteLine("Created root container.");
            }
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine("HTTP error code {0}: {1}",
                                e.Status, e.ErrorCode);
            Console.WriteLine(e.Message);
        }
    }

    public static async Task DeleteSamepleContainerAsync(BlobServiceClient client, string contaainerName)
    {
        BlobContainerClient container = client.GetBlobContainerClient(contaainerName);

        try
        {
            // Delete the specified container and handle the exception.
            await container.DeleteAsync();
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine("HTTP error code {0}: {1}",
                                e.Status, e.ErrorCode);
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }
    }

    public async static Task ListContainers(BlobServiceClient blobServiceClient, string prefix, int? segmentSize)
    {
        try
        {
            // Call the listing operation and retrieve pages of results
            var resultSegment = blobServiceClient
                .GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix, default)
                .AsPages(default, segmentSize);

            await foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
            {
                Console.WriteLine("Containers in this page:");

                foreach (BlobContainerItem containerItem in containerPage.Values)
                {
                    Console.WriteLine($"- Container name: {containerItem.Name}");

                    // Include metadata if available
                    if (containerItem.Properties.Metadata != null)
                    {
                        foreach (var metadata in containerItem.Properties.Metadata)
                        {
                            Console.WriteLine($"  Metadata: {metadata.Key} = {metadata.Value}");
                        }
                    }
                }

                Console.WriteLine(); // Blank line between pages
            }

            Console.WriteLine("Container listing completed!");
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine("Error occurred while listing containers:");
            Console.WriteLine(e.Message);
        }
    }

    public static async Task<BlobLeaseClient> AcquireContainerLeaseAsync(BlobContainerClient blobContainerClient)
    {
        BlobLeaseClient leaseClient = blobContainerClient.GetBlobLeaseClient();

        // acquire a 30-second lease
        var leaseResponse = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(30));

        Console.WriteLine($"Lease acquired. Lease ID: {leaseResponse.Value.LeaseId}");
        return leaseClient;
    }

    public static async Task RenewContainerLeaseAsync(BlobContainerClient containerClient, string leaseID)
{
    // Create a BlobLeaseClient with the lease ID
    BlobLeaseClient leaseClient = containerClient.GetBlobLeaseClient(leaseID);

    // Renew the lease
    await leaseClient.RenewAsync();
    Console.WriteLine($"Lease with ID {leaseID} has been renewed.");
}

public static async Task ReleaseContainerLeaseAsync(BlobContainerClient containerClient, string leaseID)
{
    // Create a BlobLeaseClient with the lease ID
    BlobLeaseClient leaseClient = containerClient.GetBlobLeaseClient(leaseID);

    // Release the lease
    await leaseClient.ReleaseAsync();
    Console.WriteLine($"Lease with ID {leaseID} has been released.");
}

public static async Task BreakContainerLeaseAsync(BlobContainerClient containerClient)
{
    // Create a BlobLeaseClient
    BlobLeaseClient leaseClient = containerClient.GetBlobLeaseClient();

    // Break the lease
    await leaseClient.BreakAsync();
    Console.WriteLine("The lease has been broken.");
}

public static async Task CheckLeaseStateAsync(BlobContainerClient containerClient)
{
    var properties = await containerClient.GetPropertiesAsync();
    Console.WriteLine($"Lease State: {properties.Value.LeaseState}");
    Console.WriteLine($"Lease Duration: {properties.Value.LeaseDuration}");
    Console.WriteLine($"Lease Status: {properties.Value.LeaseStatus}");
}

public static async Task LeaseWorkflow(BlobContainerClient containerClient)
{
    // Acquire a lease
    BlobLeaseClient leaseClient = await AcquireContainerLeaseAsync(containerClient);

    // Perform operations...

    // Renew the lease
    await RenewContainerLeaseAsync(containerClient, leaseClient.LeaseId);

    // Perform more operations...

    // Release the lease
    await ReleaseContainerLeaseAsync(containerClient, leaseClient.LeaseId);
}

public static async Task ManageProjectContainersAsync(BlobContainerClient container)
{
    // Step 1: Add metadata
    await AddContainerMetadataAsync(container);

    // Step 2: Retrieve and display metadata
    await ReadContainerMetadataAsync(container);

    // Step 3: Fetch system properties
    await ReadContainerPropertiesAsync(container);
}

// Set Metadata
public static async Task AddContainerMetadataAsync(BlobContainerClient container)
{
    IDictionary<string, string> metadata = new Dictionary<string, string>
    {
        { "projectId", "12345" },
        { "owner", "team-a" },
        { "status", "active" }
    };

    await container.SetMetadataAsync(metadata);
    Console.WriteLine("Project metadata added.");
}

// Retrieve Metadata
public static async Task ReadContainerMetadataAsync(BlobContainerClient container)
{
    var properties = await container.GetPropertiesAsync();

    Console.WriteLine("Metadata:");
    foreach (var metadataItem in properties.Value.Metadata)
    {
        Console.WriteLine($"{metadataItem.Key}: {metadataItem.Value}");
    }
}

// Read System Properties
public static async Task ReadContainerPropertiesAsync(BlobContainerClient container)
{
    var properties = await container.GetPropertiesAsync();

    Console.WriteLine($"Public access level: {properties.Value.PublicAccess}");
    Console.WriteLine($"Last modified: {properties.Value.LastModified}");
}


}