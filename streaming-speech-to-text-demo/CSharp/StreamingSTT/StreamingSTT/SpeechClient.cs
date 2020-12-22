using NAudio.Wave;
using Newtonsoft.Json;
using StreamingSTT.Models;
using System;
using System.Collections;
using System.IO;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingSTT
{
    public class SpeechClient
    {
        private static string path => "/api/v1/translate/stt-streaming";
        private static string endpoint => $"wss://translate.classiii.io{ path }";
        private string AccessKey { get; set; }
        private string SecretKey { get; set; }
        private string ContractId { get; set; }

        private Queue audioBuffers;

        private CancellationTokenSource clientAsyncTokenSource = new CancellationTokenSource();
        private CancellationToken clientAsyncToken;

        public SpeechClient(string accessKey, string secretKey, string contractId)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
            ContractId = contractId;

            audioBuffers = new Queue();

            clientAsyncToken = clientAsyncTokenSource.Token;
        }

        public async Task SpeechToText()
        {
            var authData = GenerateAuthData(path, AccessKey, SecretKey, ContractId);
            var authString = GenerateAuthString(authData);
            var uri = $"{endpoint}?auth={authString}";

            var waveIn = new WaveInEvent();
            // デフォルト録音デバイスを利用します。
            waveIn.DeviceNumber = 0;
            // サンプルレート、ビットレート、チャンネル数を16000Hz、16bits、1に指定します。
            waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
            waveIn.DataAvailable += (object sender, WaveInEventArgs e) =>
            {
                var inputMemStream = new MemoryStream(e.Buffer);
                var rawWaveStream = new RawSourceWaveStream(inputMemStream, waveIn.WaveFormat);
                var outputMemStream = new MemoryStream();
                WaveFileWriter.WriteWavFileToStream(outputMemStream, rawWaveStream);
                audioBuffers.Enqueue(outputMemStream.ToArray());
            };
            waveIn.RecordingStopped += (object sender, StoppedEventArgs e) =>
            {
                clientAsyncTokenSource.Cancel();
            };

            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(uri), CancellationToken.None);

            // 日本語の音声を認識します。
            _ = await SetLanguageAsync(client, "ja");
            _ = await SetSamplingRateAsync(client, 16000);

            try
            {
                waveIn.StartRecording();
                Console.WriteLine("（音声認識：認識中です。）");

                var sendLoop = this.InitSendLoop(client);
                var readLoop = this.InitReadLoop(client);
                Console.Read();

                waveIn.StopRecording();
                Console.WriteLine("（音声認識：完了しました。）");

                await sendLoop;
                await readLoop;

                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("（音声認識：サーバとの通信を止めました。）");
            }
        }

        private string GenerateSignature(string nonce, string path, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{nonce}{path}"));

                var strBuilder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    strBuilder.Append(hashedBytes[i].ToString("x2"));
                }
                return strBuilder.ToString();
            }
        }

        private AuthData GenerateAuthData(string path, string accessKey, string secretKey, string contractId)
        {
            var nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            return new AuthData()
            {
                accessKey = accessKey,
                nonce = nonce,
                signature = GenerateSignature(nonce, path, secretKey),
                remoteUrl = path,
                contractId = contractId,
            };
        }
        private string GenerateAuthString(AuthData data)
        {
            var json = JsonConvert.SerializeObject(data);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private async Task<ResponseData> SendCommand(ClientWebSocket client, RequestData data)
        {
            var request = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            await client.SendAsync(new ArraySegment<byte>(request), WebSocketMessageType.Text, true, CancellationToken.None);

            var strBuilder = new StringBuilder();
            while (true)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                var result = await client.ReceiveAsync(buffer, this.clientAsyncToken);
                strBuilder.Append(Encoding.UTF8.GetString(buffer.Array));
                if (result.EndOfMessage)
                {
                    break;
                }
            }
            return JsonConvert.DeserializeObject<ResponseData>(strBuilder.ToString());
        }

        private async Task<bool> SetLanguageAsync(ClientWebSocket client, string language)
        {
            var requestData = new RequestData()
            {
                command = RequestConstants.SetLanguage,
                value = language
            };
            var response = await SendCommand(client, requestData);
            return ((response.type != null) && response.type.Equals(ResponseConstants.LanguageReady));
        }

        private async Task<bool> SetSamplingRateAsync(ClientWebSocket client, int rate)
        {
            var requestData = new RequestData()
            {
                command = RequestConstants.SetSamplingRate,
                value = rate
            };
            var response = await SendCommand(client, requestData);
            return ((response.type != null) && response.type.Equals(ResponseConstants.SamplingRateReady));
        }

        private async Task<bool> InitSendLoop(ClientWebSocket client)
        {
            while (true)
            {
                if (!client.State.Equals(WebSocketState.Open))
                {
                    break;
                }
                if (this.audioBuffers.Count == 0)
                {
                    continue;
                }
                var data = this.audioBuffers.Dequeue() as byte[];
                await client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, this.clientAsyncToken);
            }
            return true;
        }

        private async Task<bool> InitReadLoop(ClientWebSocket client)
        {
            while (true)
            {
                if (!client.State.Equals(WebSocketState.Open))
                {
                    break;
                }
                var response = new StringBuilder();
                while (true)
                {
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    var result = await client.ReceiveAsync(buffer, this.clientAsyncToken);
                    response.Append(Encoding.UTF8.GetString(buffer.Array));
                    if (result.EndOfMessage)
                    {
                        break;
                    }
                }
                var json = JsonConvert.DeserializeObject<ResponseData>(response.ToString());
                if (json == null)
                {
                    continue;
                }
                var output = "";
                if ((json.type != null) && json.type.Equals(ResponseConstants.RecognitionResult))
                {
                    output = json.value;
                }
                Console.WriteLine("音声認識：認識結果→ {0}", output);
            }
            return true;
        }
    }
}
