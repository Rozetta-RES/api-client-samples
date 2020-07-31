Imports common.common
Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text

Namespace TextTranslationDemo
    Friend Class TextTranslationClient
        Private baseUrl As String

        Public Sub New(ByVal baseUrl As String)
            Me.baseUrl = baseUrl
        End Sub

        Public Async Function TranslateTextBySyncModeAsync(ByVal classiiiUser As ClassiiiUser, ByVal [option] As TextTranslationOption, ByVal text As String()) As Task(Of TextTranslationResult())
            Dim url As String = baseUrl & "/translate"
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(classiiiUser, url)
            Dim body As Dictionary(Of String, Object) = BuildBody([option], classiiiUser.TextContractId, text)
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .taskId = "",
                    .translationResult = New TextTranslationResult() {}
                }
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("text translation failed")
            Return serverResp.data.translationResult
        End Function

        Private Shared Function BuildBody(ByVal [option] As TextTranslationOption, contractId As String, ByVal text As String()) As Dictionary(Of String, Object)
            Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
            body.Add("fieldId", [option].FieldId.ToString())
            body.Add("targetLang", [option].TargetLang)
            body.Add("sourceLang", [option].SourceLang)
            body.Add("text", text)
            If Not String.IsNullOrEmpty(contractId) Then body.Add("contractId", contractId)
            If [option].AutoSplit IsNot Nothing Then body.Add("autoSplit", [option].AutoSplit)
            If [option].Type IsNot Nothing Then body.Add("type", [option].Type)
            If [option].RemoveFakeLineBreak IsNot Nothing Then body.Add("removeFakeLineBreak", [option].RemoveFakeLineBreak)
            Return body
        End Function

        Public Async Function TranslateTextByAsyncModeAsync(ByVal classiiiUser As ClassiiiUser, ByVal [option] As TextTranslationOption, ByVal text As String()) As Task(Of String)
            Dim url As String = baseUrl & "/translate/async"
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(classiiiUser, url)
            Dim body As Dictionary(Of String, Object) = BuildBody([option], classiiiUser.TextContractId, text)
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .queueId = ""
                }
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("text translation failed")
            Return serverResp.data.queueId
        End Function

        Public Async Function GetTranslationResultAsync(ByVal classiiiUser As ClassiiiUser, ByVal queueId As String) As Task(Of TextTranslationResult())
            Dim url As String = baseUrl & "/translate/async/" & queueId
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(classiiiUser, url)
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Get, url, headers, Nothing)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .taskId = "",
                    .translationResult = New TextTranslationResult() {}
                }
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("text translation failed")

            If Equals(serverResp.data.taskId, Nothing) Then
                Return New TextTranslationResult() {}
            End If

            Return serverResp.data.translationResult
        End Function
    End Class
End Namespace
