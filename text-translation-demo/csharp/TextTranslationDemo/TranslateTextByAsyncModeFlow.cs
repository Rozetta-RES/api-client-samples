using common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TextTranslationDemo
{
    public class TranslateTextByAsyncModeFlow
    {
        private TextTranslationClient client;
        private System.Timers.Timer aTimer = null;
        private RozettaApiUser rozettaApiUser;
        private string queueId;
        private bool done = false;
        private const int Interval = 10000;
        private TextTranslationResult[] result;
        public TranslateTextByAsyncModeFlow(string baseUrl)
        {
            client = new TextTranslationClient(baseUrl);
        }

        public async Task<TextTranslationResult[]> TranslateFlowAsync(RozettaApiUser rozettaApiUser, TextTranslationOption option, string[] text)
        {
            this.rozettaApiUser = rozettaApiUser;            

            this.queueId = await client.TranslateTextByAsyncModeAsync(rozettaApiUser, option,text);

            if (aTimer != null)
                StopTimer(aTimer);

            aTimer = new System.Timers.Timer();

            aTimer.Interval = Interval;

            aTimer.Elapsed += OnTimedEvent;

            aTimer.AutoReset = true;

            aTimer.Enabled = true;

            while (!done)
            {
                Thread.Sleep(1000);
            }

            return result;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // waiting translation done
            Task<TextTranslationResult[]> task = client.GetTranslationResultAsync(rozettaApiUser, queueId);

            task.Wait(Interval);

            if (task.Result.Length > 0)
            {                                
                foreach(TextTranslationResult result in task.Result)
                {
                    Console.WriteLine(string.Format("source:{0},translated:{1}", result.sourceText, result.translatedText));
                }
                result = task.Result;
                done = true;
                StopTimer(aTimer);
            }

        }
        private void StopTimer(System.Timers.Timer timer)
        {
            aTimer.Enabled = false;
            aTimer.Stop();
            aTimer.Dispose();
        }
    }
}
