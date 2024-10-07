using Microsoft.WindowsAzure.Storage;

namespace Fliq.Application.Common.Helpers
{
    public class AzureConnectionString
    {
        public static CloudStorageAccount GetConnectionString()
        {
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName=connectvibemedia;AccountKey=zgzApFbWgrbCW3PzPBhnVFJGysQAdAdbwS55ADxfRdtkr36sHzVrOR9JjAFv6bmeiOjoHCY1gWZz+AStXLk93g==;EndpointSuffix=core.windows.net");
            return CloudStorageAccount.Parse(connectionString);
        }
    }
}