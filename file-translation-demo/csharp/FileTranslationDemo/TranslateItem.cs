using System;

namespace FileTranslationDemo
{
    public class TranslateItem
    {
        public string translateItemId { get; set; }
        public string lang { get; set; }
        public string originalName { get; set; }
        public float? wordCount { get; set; }
        public string detectedLang { get; set; }
        public Boolean done { get; set; }
    }
}
