using Newtonsoft.Json;
using System.IO;

namespace common
{
    public class UserInfo
    {
        public static void init(string filePath)
        {
            string text = File.ReadAllText(filePath);
            var definition = new {
                ROZETTA_API_ACCESS_KEY = "",
                ROZETTA_API_SECRET_KEY = "",
                FILE_CONTRACT_ID = "",
                TEXT_CONTRACT_ID = ""
            };
            var obj = JsonConvert.DeserializeAnonymousType(text, definition);            
            UserInfo.ROZETTA_API_ACCESS_KEY = obj.ROZETTA_API_ACCESS_KEY;
            UserInfo.ROZETTA_API_SECRET_KEY = obj.ROZETTA_API_SECRET_KEY;
            UserInfo.FILE_CONTRACT_ID = obj.FILE_CONTRACT_ID;
            UserInfo.TEXT_CONTRACT_ID = obj.TEXT_CONTRACT_ID;
        }
        public static string ROZETTA_API_ACCESS_KEY { get; private set; }
        public static string ROZETTA_API_SECRET_KEY { get; private set; }
        public static string FILE_CONTRACT_ID { get; private set; }
        public static string TEXT_CONTRACT_ID { get; private set; }
    }
}
