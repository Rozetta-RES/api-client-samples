Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace common
    Public Class RC4
        Private s As Char() = New Char(255) {}
        Private privateKey As String

        Public Sub New(ByVal key As String)
            privateKey = key
            BuildKeyBytes()
        End Sub

        Private Sub BuildKeyBytes()
            Dim k As Char() = New Char(255) {}

            ' Init keystream
            Dim kLen As Integer = privateKey.Length

            For i As Integer = 0 To 256 - 1
                s(i) = ChrW(i)
                k(i) = privateKey.Substring(i Mod kLen, 1)(0)
            Next

            Dim j As Integer = 0

            For i As Integer = 0 To 256 - 1
                j = (j + AscW(k(i)) + AscW(s(i))) Mod 256
                Dim tmp As Char = s(i)
                s(i) = s(j)
                s(j) = tmp
            Next
        End Sub

        Public Function Enc(ByVal message As String) As String
            Dim x As Integer = 0, y As Integer = 0
            Dim ret As String = ""

            For i As Integer = 0 To message.Length - 1
                x = (x + 1) Mod 256
                y = (y + AscW(s(x))) Mod 256
                Dim tmp As Char = s(x)
                s(x) = s(y)
                s(y) = tmp
                Dim ch As Integer = AscW(s((AscW(s(x)) + AscW(s(y))) Mod 256)) Xor AscW(message.Substring(i, 1)(0))
                ret += String.Format("{0:x02}", ch)
            Next

            Return ret
        End Function

        Public Function Dec(ByVal encryptedMessage As String) As String
            Dim x As Integer = 0, y As Integer = 0
            Dim ret As String = ""
            Dim word As Regex = New Regex("[a-z0-9]{2}")
            Dim results = word.Matches(encryptedMessage)

            For Each match As Match In results
                x = (x + 1) Mod 256
                y = (y + AscW(s(x))) Mod 256
                Dim tmp As Char = s(x)
                s(x) = s(y)
                s(y) = tmp
                Dim numb As Integer = Integer.Parse(match.Value, NumberStyles.HexNumber)
                Dim ch As Integer = AscW(s((AscW(s(x)) + AscW(s(y))) Mod 256)) Xor numb
                ret += Convert.ToChar(ch)
            Next

            Return ret
        End Function
    End Class
End Namespace
