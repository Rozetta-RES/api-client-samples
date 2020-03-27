using common;

namespace TextTranslationDemo
{
    public class TextTranslationOption 
    {
        public int FieldId { get; set; }
        public string TargetLang { get; set; }
        public string SourceLang { get; set; }
        public bool? AutoSplit { get; set; }
        public TranslationEngineType Type { get; set; }
        public bool? RemoveFakeLineBreak { get; set; }
    }
}
