Imports common.common
Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text

Namespace TextTranslationDemo
    Public Class UserDictionaryClient
        Private baseUrl As String

        Public Sub New(ByVal baseUrl As String)
            Me.baseUrl = baseUrl
        End Sub

        Public Async Function GetUserDictionaryAsync(ByVal rozettaApiUser As RozettaApiUser) As Task(Of UserDictionaryItem())
            Dim url As String = baseUrl & "/dictionary"
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(rozettaApiUser, url)
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Get, url, headers, Nothing)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = "",
                .data = New With {
                    .entries = New UserDictionaryItem() {}
                }
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("get user dictionary failed")
            Return serverResp.data.entries
        End Function

        Public Async Function AddUserDictionaryItemAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal item As UserDictionaryItem) As Task(Of Boolean)
            Dim url As String = baseUrl & "/dictionary"
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(rozettaApiUser, url)
            Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)() From {
                {"fromLang", item.fromLang},
                {"fromText", item.fromText},
                {"toLang", item.toLang},
                {"toText", item.toText}
            }
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Post, url, headers, body)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = ""
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("add user dictionary item failed")
            Return True
        End Function

        Public Async Function DeleteUserDictionaryItemAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal id As Integer) As Task(Of Boolean)
            Dim url As String = baseUrl & "/dictionary/" & id
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(rozettaApiUser, url)
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Delete, url, headers, Nothing)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = ""
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("delete user dictionary item failed")
            Return True
        End Function

        Public Async Function UpdateUserDictionaryItemAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal id As Integer, ByVal item As UserDictionaryItem) As Task(Of Boolean)
            Dim url As String = baseUrl & "/dictionary/" & id
            Dim headers As Dictionary(Of String, Object) = HttpUtils.BuildHeaders(rozettaApiUser, url)
            Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)() From {
                {"fromLang", item.fromLang},
                {"fromText", item.fromText},
                {"toLang", item.toLang},
                {"toText", item.toText}
            }
            Dim content = Await HttpUtils.SendAsync(HttpMethod.Put, url, headers, body)
            Dim byteArray = Await content.ReadAsByteArrayAsync()
            Dim responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length)
            Dim definition = New With {
                .status = ""
            }
            Dim serverResp = JsonConvert.DeserializeAnonymousType(responseString, definition)
            If Not Equals(serverResp.status, ResponseStatus.Success) Then Throw New Exception("update user dictionary item failed")
            Return True
        End Function
    End Class
End Namespace
