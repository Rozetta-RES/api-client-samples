Imports common.common

Namespace FileTranslationDemo
    Public Class FileTranslateClient
        Private baseUrl As String

        Public Sub New(ByVal baseUrl As String)
            Me.baseUrl = baseUrl
        End Sub

        Public Async Function GetAuthCodeAsync(ByVal t4ooUser As T4ooUser) As Task(Of String)
            Dim t4ooUserDict As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
                {"orgId", t4ooUser.orgId},
                {"userId", t4ooUser.userId}
            }
            Dim content = Await HttpUtils.SendAsync(Net.Http.HttpMethod.Post, Me.baseUrl & "/auth-code", DirectCast(Nothing, Global.System.Collections.Generic.Dictionary(Of System.String, System.Object)), t4ooUserDict)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .id = ""
                }
            }
            Dim serverResp = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then
                Return DirectCast(Nothing, System.String)
            End If
            Return serverResp.data.id
        End Function

        Public Async Function AuthenticateAsync(ByVal t4ooUser As T4ooUser, ByVal authCode As String) As Task(Of ClassiiiUser)
            Dim encryptedPassword As String = New RC4(CStr((authCode))).Enc(t4ooUser.password)
            Dim t4ooUserDict As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
                {"orgId", t4ooUser.orgId},
                {"userId", t4ooUser.userId},
                {"password", encryptedPassword}
            }
            Dim content = Await HttpUtils.SendAsync(Net.Http.HttpMethod.Post, Me.baseUrl & "/authenticate", DirectCast(Nothing, Dictionary(Of String, Object)), t4ooUserDict)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .accessKey = "",
                    .secretKey = ""
                }
            }
            Dim serverResp = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then
                Return DirectCast(Nothing, ClassiiiUser)
            End If
            Return New ClassiiiUser With {
                .AccessKey = serverResp.data.accessKey,
                .SecretKey = serverResp.data.secretKey
            }
        End Function

        Public Async Function TranslateAsync(ByVal files As String(), ByVal [option] As FileTranslationOption) As Task(Of String)
            Dim url As String = Me.baseUrl & "/file-translate"
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(New ClassiiiUser With {
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

        Public Function GetAllHistories(ByVal classiiiUser As ClassiiiUser) As Translate()
            Dim ret As Translate() = {}
            Return ret
        End Function

        Public Async Function GetOneHistoryAsync(ByVal classiiiUser As ClassiiiUser, ByVal translateId As String) As Task(Of Translate)
            Dim url As String = Me.baseUrl & "/translate-result/" & translateId
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(classiiiUser, url)
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

        Public Async Function DownloadZipAsync(ByVal classiiiUser As ClassiiiUser, ByVal ids As String, ByVal savePath As String) As Task(Of Boolean)
            Dim url As String = Me.baseUrl & "/downloads?ids=" & Web.HttpUtility.UrlEncode(ids)
            Dim apiPath As String = Web.HttpUtility.UrlDecode(New Uri(CStr((url))).PathAndQuery)
            Dim nonce As String = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
            Dim signature As String = Utils.GetSignature(nonce, apiPath, classiiiUser.SecretKey)
            Dim headers As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
                {"accessKey", classiiiUser.AccessKey},
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
