using common;
using System.Diagnostics;

namespace TextTranslationDemo
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {            
            UserInfo.init("config.json");            
            string baseUrl = "https://translate.rozetta-api.io/api/v1";
            RozettaApiUser rozettaApiUser = new RozettaApiUser {
                AccessKey = UserInfo.ROZETTA_API_ACCESS_KEY,
                SecretKey = UserInfo.ROZETTA_API_SECRET_KEY,
                ContractId = UserInfo.TEXT_CONTRACT_ID
            };
            // テキスト翻訳を試す
            await TestTextTranslation(baseUrl, rozettaApiUser);
            // ユーザー辞書を試す
            await TestUserDictionary(baseUrl, rozettaApiUser);
        }

        private static async System.Threading.Tasks.Task TestTextTranslation(string baseUrl, RozettaApiUser rozettaApiUser)
        {
            string[] text = new string[] { "hello" };
            TextTranslationOption option = new TextTranslationOption
            {
                FieldId = 1,
                TargetLang = "ja",
                SourceLang = "en"
            };
            TextTranslationClient textTranslationClient = new TextTranslationClient(baseUrl);
            //  同期テキスト翻訳
            TextTranslationResult[] ret = await textTranslationClient.TranslateTextBySyncModeAsync(rozettaApiUser, option, text);

            Debug.Assert(ret.Length == 1);
            Debug.Assert(ret[0].sourceText == "hello"); 
            Debug.Assert(ret[0].translatedText == "挨拶");

            //  非同期テキスト翻訳
            TranslateTextByAsyncModeFlow flow = new TranslateTextByAsyncModeFlow(baseUrl);
            ret = await flow.TranslateFlowAsync(rozettaApiUser, option, text);

            Debug.Assert(ret.Length == 1);
            Debug.Assert(ret[0].sourceText == "hello");
            Debug.Assert(ret[0].translatedText == "挨拶");
        }
        private static async System.Threading.Tasks.Task TestUserDictionary(string baseUrl, RozettaApiUser rozettaApiUser)
        {
            UserDictionaryClient userDictionaryClient = new UserDictionaryClient(baseUrl);

            UserDictionaryItem item = new UserDictionaryItem { fromLang = "en", fromText = "hello", toLang = "ja", toText = "おはよう" };

            bool bRet;

            string[] text = new string[] { "hello" };
            TextTranslationOption option = new TextTranslationOption
            {
                FieldId = 1,
                SourceLang = "en",
                TargetLang = "ja"
            };
            TextTranslationClient textTranslationClient = new TextTranslationClient(baseUrl);
            TextTranslationResult[] textTranslationResults;
            UserDictionaryItem[] userDictionaryItems;

            // ユーザー辞書を登録
            bRet = await userDictionaryClient.AddUserDictionaryItemAsync(rozettaApiUser, item);
            Debug.Assert(bRet);

            // ユーザー辞書を取得
            userDictionaryItems = await userDictionaryClient.GetUserDictionaryAsync(rozettaApiUser);
            Debug.Assert(userDictionaryItems.Length == 1);
            Debug.Assert(userDictionaryItems[0] == item);

            // ユーザー辞書を確認 -----------------------------------------------------             
            textTranslationResults = await textTranslationClient.TranslateTextBySyncModeAsync(rozettaApiUser, option, text);
            Debug.Assert(textTranslationResults.Length == 1);
            Debug.Assert(textTranslationResults[0].sourceText == "hello");
            Debug.Assert(textTranslationResults[0].translatedText == "おはよう");
            // ユーザー辞書を確認 ----------------------------------------------------- end


            // ユーザー辞書を削除
            bRet = await userDictionaryClient.DeleteUserDictionaryItemAsync(rozettaApiUser, userDictionaryItems[0].id);
            Debug.Assert(bRet);

            // ユーザー辞書数を確認
            userDictionaryItems = await userDictionaryClient.GetUserDictionaryAsync(rozettaApiUser);
            Debug.Assert(userDictionaryItems.Length == 0);                    
        }

    }
}
