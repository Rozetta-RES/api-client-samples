using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace common
{
    public class Utils
    {
        public static string GetSignature(string nonce, string path, string secretKey)
        {
            string ret;
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(nonce + path));
                ret = ByteArrayToString(hashValue);
            }
            return ret;
        }
        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static void UnzipStream(Stream stream, string savePath)
        {
            var ar = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);

            foreach (var entry in ar.Entries)
            {
                var path = Path.Combine(savePath, entry.FullName);

                if (string.IsNullOrEmpty(entry.Name))
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                    continue;
                }

                using (var entryStream = entry.Open())
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (var file = File.Create(path))
                    {
                        entryStream.CopyTo(file);
                    }
                }
            }
        }
    }
}
