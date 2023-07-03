using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using PasswordVerificationResult = Microsoft.AspNet.Identity.PasswordVerificationResult;

namespace PizzaDeliveryApi.Helpers;

public static class HashProvider 
{
    public static string ComputeHash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the input string to a byte array
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            // Compute the hash value of the byte array
            byte[] hashBytes = sha256Hash.ComputeHash(bytes);

            // Convert the byte array to a hexadecimal string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }

    }

    public static bool VerifyHash(string input, string hash)
    {
        string hashedInput = ComputeHash(input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        // Compare the computed hash with the provided hash
        return comparer.Compare(hashedInput, hash) == 0;
    }
}

public class IdentityHashProvider : IPasswordHasher
{
    public string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the input string to a byte array
            byte[] bytes = Encoding.UTF8.GetBytes(password);

            // Compute the hash value of the byte array
            byte[] hashBytes = sha256Hash.ComputeHash(bytes);

            // Convert the byte array to a hexadecimal string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
    public Microsoft.AspNet.Identity.PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        string hashedInput = HashPassword(providedPassword);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        // Compare the computed hash with the provided hash
        if (comparer.Compare(hashedInput, hashedPassword) == 0)
            return PasswordVerificationResult.Success;
        else return PasswordVerificationResult.Failed;
    }
}
