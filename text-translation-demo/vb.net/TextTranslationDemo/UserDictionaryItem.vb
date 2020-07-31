Imports System

Namespace TextTranslationDemo
    Public Class UserDictionaryItem
        Public Property id As Integer
        Public Property fromLang As String
        Public Property fromText As String
        Public Property toLang As String
        Public Property toText As String

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return TypeOf obj Is UserDictionaryItem AndAlso Me Is CType(obj, UserDictionaryItem)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Tuple.Create(Of String, Global.System.[String], Global.System.[String], Global.System.[String])(CStr(fromLang), CStr(fromText), CStr(toLang), CStr(toText)).GetHashCode()
        End Function

        Public Shared Operator =(ByVal x As UserDictionaryItem, ByVal y As UserDictionaryItem) As Boolean
            Return Equals(x.fromLang, y.fromLang) AndAlso Equals(x.fromText, y.fromText) AndAlso Equals(x.toLang, y.toLang) AndAlso Equals(x.toText, y.toText)
        End Operator

        Public Shared Operator <>(ByVal x As UserDictionaryItem, ByVal y As UserDictionaryItem) As Boolean
            Return Not x Is y
        End Operator
    End Class
End Namespace
