Imports common.common

Namespace TextTranslationDemo
    Public Class TextTranslationOption
        Public Property FieldId As Integer
        Public Property TargetLang As String
        Public Property SourceLang As String
        Public Property AutoSplit As Boolean?
        Public Property Type As TranslationEngineType
        Public Property RemoveFakeLineBreak As Boolean?
    End Class
End Namespace
