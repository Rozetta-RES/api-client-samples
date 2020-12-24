using System.Threading.Tasks;

namespace StreamingSTT
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var accessKey = "YOUR_ACCESS_KEY";
            var secretKey = "YOUR_SECRET_KEY";
            var contractId = "YOUR_CONTRACT_ID";

            var client = new SpeechClient(accessKey, secretKey, contractId);
            await client.SpeechToText();
        }
    }
}
