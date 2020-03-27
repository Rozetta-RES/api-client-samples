using System;

namespace FileTranslationDemo
{
    public class Translate
    {
        public string translateId { get; set; }
        public string[] langs { get; set; }
        public int fieldId { get; set; }
        public TranslateItem[] items { get; set; }
        public Boolean done { get; set; }
        public DateTime createdAt { get; set; }
    }
}
