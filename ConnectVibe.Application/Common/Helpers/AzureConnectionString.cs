using Azure.Storage.Blobs;

namespace Fliq.Application.Common.Helpers
{
    public class AzureConnectionString
    {
        public static BlobServiceClient GetConnectionString()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=connectvibemedia;AccountKey=zgzApFbWgrbCW3PzPBhnVFJGysQAdAdbwS55ADxfRdtkr36sHzVrOR9JjAFv6bmeiOjoHCY1gWZz+AStXLk93g==;EndpointSuffix=core.windows.net";
            return new BlobServiceClient(connectionString);
        }
    }
}