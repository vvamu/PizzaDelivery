using System.Security.Cryptography;
using System.Text;

namespace PizzaDelivery.Application.Helpers;
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
            var res = builder.ToString();

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
