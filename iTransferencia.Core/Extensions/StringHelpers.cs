using System.Security.Cryptography;
using System.Text;

namespace iTransferencia.Core.Extensions
{
    static class StringHelper
    {
        public static string GenerateIdempotenceHash(string idSourceAccount, string idDestinationAccount, decimal value)
        {
            string combinedString = idSourceAccount + idDestinationAccount + value + DateTime.Now.ToString("yyyy-MM-dd");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
                StringBuilder hashStringBuilder = new StringBuilder();

                foreach (byte b in hashBytes)
                    hashStringBuilder.Append(b.ToString("x2"));

                return hashStringBuilder.ToString();
            }
        }
    }
}