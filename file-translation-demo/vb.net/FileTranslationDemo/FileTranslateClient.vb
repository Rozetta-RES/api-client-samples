Imports common.common

Namespace FileTranslationDemo
    Public Class FileTranslateClient
        Private baseUrl As String

        Public Sub New(ByVal baseUrl As String)
            Me.baseUrl = baseUrl
        End Sub

        Public Async Function TranslateAsync(ByVal files As String(), ByVal [option] As FileTranslationOption) As Task(Of String)
            Dim url As String = Me.baseUrl & "/file-translate"
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(New RozettaApiUser With {
                .AccessKey = [option].AccessKey,
                .SecretKey = [option].SecretKey
            }, url)
            Dim body As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
                {"fieldId", [option].FieldId.ToString()},
                {"targetLangs", Newtonsoft.Json.JsonConvert.SerializeObject([option].Langs)}
            }
            If [option].ContractId IsNot Nothing Then
                body.Add("contractId", [option].ContractId)
            End If
            Dim fileDict As Dictionary(Of String, String) = New Dictionary(Of String, String)()

            For Each file As String In files
                fileDict.Add("files", file)
            Next

            Dim content = Await HttpUtils.PostFileAsync(url, headers, body, fileDict)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .translateId = ""
                }
            }
            Dim serverResp = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("upload file failed")
            Return serverResp.data.translateId
        End Function

        Public Function GetAllHistories(ByVal rozetaApiUser As RozettaApiUser) As Translate()
            Dim ret As Translate() = {}
            Return ret
        End Function

        Public Async Function GetOneHistoryAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal translateId As String) As Task(Of Translate)
            Dim url As String = Me.baseUrl & "/translate-result/" & translateId
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(rozettaApiUser, url)
            Dim content = Await HttpUtils.SendAsync(Net.Http.HttpMethod.[Get], url, headers, DirectCast(Nothing, Dictionary(Of String, Object)))
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New Translate
            }
            Dim serverResp = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(responseString, definition)

            Debug.WriteLine($"responseString:{responseString}")

            If Not Equals(serverResp.status, ResponseStatus.Success) Then
                Return DirectCast(Nothing, Translate)
            End If
            Return serverResp.data
        End Function

        Public Async Function DownloadZipAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal ids As String, ByVal savePath As String) As Task(Of Boolean)
            Dim url As String = Me.baseUrl & "/downloads?ids=" & Web.HttpUtility.UrlEncode(ids)
            Dim apiPath As String = Web.HttpUtility.UrlDecode(New Uri(CStr((url))).PathAndQuery)
            Dim nonce As String = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
            Dim signature As String = Utils.GetSignature(nonce, apiPath, rozettaApiUser.SecretKey)
            Dim headers As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
                {"accessKey", rozettaApiUser.AccessKey},
                {"nonce", nonce},
                {"signature", signature}
            }
            Dim content = Await HttpUtils.SendAsync(Net.Http.HttpMethod.[Get], url, headers, DirectCast(Nothing, Dictionary(Of String, Object)))
            Dim stream As IO.MemoryStream = New IO.MemoryStream()
            Await content.CopyToAsync(stream)
            Utils.UnzipStream(stream, savePath)
            Return True
        End Function
    End Class
End Namespace
