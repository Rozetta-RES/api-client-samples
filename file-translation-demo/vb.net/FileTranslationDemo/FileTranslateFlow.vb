Imports common.common

Namespace FileTranslationDemo
    Public Class FileTranslateFlow
        Private client As FileTranslateClient
        Private aTimer As Timers.Timer = DirectCast(Nothing, Timers.Timer)
        Private rozettaApiUser As RozettaApiUser
        Private translateId As String
        Private done As Boolean = False
        Private Const Interval As Integer = 10000

        Public Sub New(ByVal baseUrl As String)
            Me.client = New FileTranslateClient(baseUrl)
        End Sub

        Public Async Function RozettaApiFlowAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal files As String(), ByVal langs As String(), ByVal fieldId As Integer) As Task(Of Boolean)
            Me.rozettaApiUser = rozettaApiUser
            Dim timeStamp As Long = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            Dim [option] As FileTranslationOption = New FileTranslationOption With {
                .Langs = langs,
                .AccessKey = rozettaApiUser.AccessKey,
                .SecretKey = rozettaApiUser.SecretKey,
                .ContractId = rozettaApiUser.FileContractId,
                .Nonce = timeStamp.ToString(),
                .FieldId = fieldId
            }
            Me.translateId = Await Me.client.TranslateAsync(files, [option])
            If Me.aTimer IsNot DirectCast(Nothing, Object) Then Me.StopTimer(Me.aTimer)
            Me.aTimer = New Timers.Timer()
            Me.aTimer.Interval = Interval
            AddHandler Me.aTimer.Elapsed, AddressOf Me.OnTimedEvent
            Me.aTimer.AutoReset = True
            Me.aTimer.Enabled = True

            While Not Me.done
                Threading.Thread.Sleep(1000)
            End While

            Return True
        End Function

        Private Sub OnTimedEvent(ByVal source As Object, ByVal e As Timers.ElapsedEventArgs)
            ' waiting for all translation done
            Dim task As Task(Of Translate) = Me.client.GetOneHistoryAsync(Me.rozettaApiUser, Me.translateId)
            task.Wait(Interval)
            Dim translate As Translate = task.Result
            If translate Is DirectCast(Nothing, Object) Then Return

            For Each item As TranslateItem In translate.items

                If Not item.done Then
                    Return
                End If
            Next

            Dim ids As List(Of String) = New List(Of String)()

            For Each item As TranslateItem In translate.items
                ids.Add(item.translateItemId)
            Next

            Dim strIds As String = Newtonsoft.Json.JsonConvert.SerializeObject(ids)
            Dim downloadTask As Task(Of Boolean) = Me.client.DownloadZipAsync(Me.rozettaApiUser, strIds, "C:\mydocuments")
            downloadTask.Wait(Interval)
            Me.done = downloadTask.Result
            If Me.done Then Me.StopTimer(Me.aTimer)
        End Sub

        Private Sub StopTimer(ByVal timer As Timers.Timer)
            Me.aTimer.Enabled = False
            Me.aTimer.[Stop]()
            Me.aTimer.Dispose()
        End Sub
    End Class
End Namespace
