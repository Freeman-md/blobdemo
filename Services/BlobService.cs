using Azure.Storage.Blobs;

public class BlobService {
    private readonly BlobContainerClient _blobContainerClient;
    public BlobService(BlobContainerClient containerClient) {
        _blobContainerClient = containerClient;
    }
}