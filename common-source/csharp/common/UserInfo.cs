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
                T4OO_ORG_ID = "",
                T4OO_USER_ID ="",
                T4OO_PASSWORD = "",
                CLASSIII_ACCESS_KEY = "",
                CLASSIII_SECRET_KEY = "",
                FILE_CONTRACT_ID = "",
                TEXT_CONTRACT_ID = ""
            };
            var obj = JsonConvert.DeserializeAnonymousType(text, definition);            
            UserInfo.T4OO_ORG_ID = obj.T4OO_ORG_ID;
            UserInfo.T4OO_USER_ID = obj.T4OO_USER_ID;
            UserInfo.T4OO_PASSWORD = obj.T4OO_PASSWORD;
            UserInfo.CLASSIII_ACCESS_KEY = obj.CLASSIII_ACCESS_KEY;
            UserInfo.CLASSIII_SECRET_KEY = obj.CLASSIII_SECRET_KEY;
            UserInfo.FILE_CONTRACT_ID = obj.FILE_CONTRACT_ID;
            UserInfo.TEXT_CONTRACT_ID = obj.TEXT_CONTRACT_ID;
        }
        public static string T4OO_ORG_ID  { get; private set; }
        public static string T4OO_USER_ID { get; private set; }
        public static string T4OO_PASSWORD { get; private set; }
        public static string CLASSIII_ACCESS_KEY { get; private set; }
        public static string CLASSIII_SECRET_KEY { get; private set; }
        public static string FILE_CONTRACT_ID { get; private set; }
        public static string TEXT_CONTRACT_ID { get; private set; }
    }
}
