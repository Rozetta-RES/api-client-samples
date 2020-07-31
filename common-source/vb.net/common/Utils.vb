Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Namespace common
    Public Class Utils
        Public Shared Function GetSignature(ByVal nonce As String, ByVal path As String, ByVal secretKey As String) As String
            Dim ret As String

            Using hmac As HMACSHA256 = New HMACSHA256(Encoding.UTF8.GetBytes(secretKey))
                Dim hashValue As Byte() = hmac.ComputeHash(Encoding.UTF8.GetBytes(nonce & path))
                ret = ByteArrayToString(hashValue)
            End Using

            Return ret
        End Function

        Private Shared Function ByteArrayToString(ByVal ba As Byte()) As String
            Dim hex As StringBuilder = New StringBuilder(ba.Length * 2)

            For Each b As Byte In ba
                hex.AppendFormat("{0:x2}", b)
            Next

            Return hex.ToString()
        End Function

        Public Shared Sub UnzipStream(ByVal stream As Stream, ByVal savePath As String)
            Dim ar = New Compression.ZipArchive(stream, Compression.ZipArchiveMode.Read)

            For Each entry In ar.Entries
                Dim path = IO.Path.Combine(savePath, entry.FullName)

                If String.IsNullOrEmpty(entry.Name) Then
                    Directory.CreateDirectory(IO.Path.GetDirectoryName(path))
                    Continue For
                End If

                Using entryStream = entry.Open()
                    Directory.CreateDirectory(IO.Path.GetDirectoryName(path))

                    Using file = IO.File.Create(path)
                        entryStream.CopyTo(file)
                    End Using
                End Using
            Next
        End Sub
    End Class
End Namespace
