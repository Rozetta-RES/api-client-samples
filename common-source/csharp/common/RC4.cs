using System;
using System.Text.RegularExpressions;


namespace common
{

    public class RC4
    {
        private char[] s = new char[256];
        private string privateKey;
        public RC4(string key)
        {
            privateKey = key;
            BuildKeyBytes();
        }
        private void BuildKeyBytes()
        {
            char[] k = new char[256];

            // Init keystream
            int kLen = privateKey.Length;
            for (int i = 0; i < 256; i++)
            {
                s[i] = (char)i;
                k[i] = privateKey.Substring(i % kLen, 1)[0];
            }

            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + k[i] + s[i]) % 256;
                char tmp = s[i];
                s[i] = s[j];
                s[j] = tmp;
            }

        }

        public string Enc(string message)
        {


            int x = 0, y = 0;
            string ret = "";
            for (int i = 0; i < message.Length; i++)
            {
                x = (x + 1) % 256;
                y = (y + s[x]) % 256;
                char tmp = s[x];
                s[x] = s[y];
                s[y] = tmp;
                int ch = s[(s[x] + s[y]) % 256] ^ message.Substring(i, 1)[0];
                ret += string.Format("{0:x02}", ch);
            }
            return ret;
        }
        public string Dec(string encryptedMessage)
        {
            int x = 0, y = 0;
            string ret = "";
            Regex word = new Regex(@"[a-z0-9]{2}");
            var results = word.Matches(encryptedMessage);
            foreach (Match match in results)
            {
                x = (x + 1) % 256;
                y = (y + s[x]) % 256;
                char tmp = s[x];
                s[x] = s[y];
                s[y] = tmp;
                int numb = int.Parse(match.Value, System.Globalization.NumberStyles.HexNumber);
                int ch = s[(s[x] + s[y]) % 256] ^ numb;
                ret += Convert.ToChar(ch);

            }

            return ret;
        }
    }


}
