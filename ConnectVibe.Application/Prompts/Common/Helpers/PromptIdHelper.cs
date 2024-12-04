using System.Security.Cryptography;
using System.Text;

namespace Fliq.Application.Prompts.Common.Helpers
{
    public static class PromptIdHelper
    {
        public static string GenerateCustomPromptId(int userId, string category, DateTime? date = null)
        {
            // Use date or default to current time
            date ??= DateTime.UtcNow;

            // Step 1: Extract unique components
            string userIdentifier = $"U{userId}";
            string categoryCode = category.Length > 3 ? category.Substring(0, 3).ToUpper() : category.ToUpper();
            string timestamp = date.Value.ToString("yyyyMMddHHmmss");

            // Step 2: Concatenate components for hash
            string rawId = $"{userIdentifier}-{categoryCode}-{timestamp}";

            // Step 3: Create a short, unique hash from the rawId
            string shortHash = GenerateShortHash(rawId);

            // Step 4: Concatenate everything to form a unique, descriptive CustomPromptId
            return $"{userIdentifier}-{categoryCode}-{shortHash}";
        }

        // Hash generator for extra uniqueness
        private static string GenerateShortHash(string input)
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            // Convert to base64 for a short, URL-safe string
            return Convert.ToBase64String(hashBytes)
                .Replace("+", "") // Remove special characters
                .Replace("/", "") // Remove special characters
                .Substring(0, 6); // Keep it compact
        }
    }
}
