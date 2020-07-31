Imports Newtonsoft.Json
Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text

Namespace common
    Public Class HttpUtils
        Private Shared ReadOnly client As HttpClient = New HttpClient()

        Public Shared Async Function SendAsync(ByVal method As HttpMethod, ByVal url As String, ByVal headers As Dictionary(Of String, Object), ByVal bodyDict As Dictionary(Of String, Object)) As Task(Of HttpContent)
            Dim request = New HttpRequestMessage(method, url)

            If headers IsNot Nothing AndAlso headers.Count > 0 Then

                For Each header As KeyValuePair(Of String, Object) In headers
                    request.Headers.Add(header.Key, header.Value.ToString())
                Next
            End If

            If bodyDict IsNot Nothing AndAlso bodyDict.Count > 0 Then
                Dim jsonStr = JsonConvert.SerializeObject(bodyDict, Formatting.Indented)
                Dim stringContent = New StringContent(jsonStr, Encoding.UTF8, "application/json")
                request.Content = stringContent
            End If

            Dim response As HttpResponseMessage = Await client.SendAsync(request)
            If Not response.IsSuccessStatusCode Then Throw New Exception(String.Format("Http failed {0}", response.StatusCode))
            Return response.Content
        End Function

        Public Shared Async Function PostFileAsync(ByVal url As String, ByVal headers As Dictionary(Of String, Object), ByVal bodyDict As Dictionary(Of String, String), ByVal files As Dictionary(Of String, String)) As Task(Of HttpContent)
            Dim request = New HttpRequestMessage(HttpMethod.Post, url)

            If headers IsNot Nothing AndAlso headers.Count > 0 Then

                For Each header As KeyValuePair(Of String, Object) In headers
                    request.Headers.Add(header.Key, header.Value.ToString())
                Next
            End If

            Dim content = New MultipartFormDataContent()

            If files IsNot Nothing AndAlso files.Count > 0 Then

                For Each filePair As KeyValuePair(Of String, String) In files
                    Dim fileNameOnly = Path.GetFileName(filePair.Value)
                    Dim fileContent = New StreamContent(File.OpenRead(filePair.Value))
                    Dim fileNameHeader As String = String.Format("form-data; name=""{0}"" ;filename=""{1}""", filePair.Key, fileNameOnly)
                    fileContent.Headers.Add("Content-Disposition", New String(Encoding.UTF8.GetBytes(fileNameHeader).[Select](Function(b) ChrW(b)).ToArray()))
                    fileContent.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")
                    content.Add(fileContent)
                Next
            End If

            If bodyDict IsNot Nothing AndAlso bodyDict.Count > 0 Then

                For Each pair As KeyValuePair(Of String, String) In bodyDict
                    Dim stringContent = New StringContent(String.Format("{0}", pair.Value))
                    stringContent.Headers.ContentDisposition = New ContentDispositionHeaderValue("form-data") With {
                        .Name = pair.Key
                    }
                    content.Add(stringContent, pair.Key)
                Next
            End If

            request.Content = content
            Dim response As HttpResponseMessage = Await client.SendAsync(request)
            If Not response.IsSuccessStatusCode Then Throw New Exception(String.Format("post file failed {0}", response.StatusCode))
            Return response.Content
        End Function

        Public Shared Function BuildHeaders(ByVal classiiiUser As ClassiiiUser, ByVal url As String) As Dictionary(Of String, Object)
            Dim apiPath As String = New Uri(url).PathAndQuery
            Dim nonce As String = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
            Dim signature As String = Utils.GetSignature(nonce, apiPath, classiiiUser.SecretKey)
            Return New Dictionary(Of String, Object) From {
                {"accessKey", classiiiUser.AccessKey},
                {"nonce", nonce},
                {"signature", signature}
            }
        End Function
    End Class
End Namespace
