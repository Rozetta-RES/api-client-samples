Imports common.common
Imports System.Threading
Imports System.Timers

Namespace TextTranslationDemo
    Public Class TranslateTextByAsyncModeFlow
        Private client As TextTranslationClient
        Private aTimer As Timers.Timer = Nothing
        Private rozettaApiUser As RozettaApiUser
        Private queueId As String
        Private done As Boolean = False
        Private Const Interval As Integer = 10000
        Private result As TextTranslationResult()

        Public Sub New(ByVal baseUrl As String)
            client = New TextTranslationClient(baseUrl)
        End Sub

        Public Async Function TranslateFlowAsync(ByVal rozettaApiUser As RozettaApiUser, ByVal [option] As TextTranslationOption, ByVal text As String()) As Task(Of TextTranslationResult())
            Me.rozettaApiUser = rozettaApiUser
            queueId = Await client.TranslateTextByAsyncModeAsync(rozettaApiUser, [option], text)
            If aTimer IsNot Nothing Then StopTimer(aTimer)
            aTimer = New Timers.Timer()
            aTimer.Interval = Interval
            AddHandler aTimer.Elapsed, AddressOf OnTimedEvent
            aTimer.AutoReset = True
            aTimer.Enabled = True

            While Not done
                Thread.Sleep(1000)
            End While

            Return result
        End Function

        Private Sub OnTimedEvent(ByVal source As Object, ByVal e As ElapsedEventArgs)
            ' waiting translation done
            Dim task As Task(Of TextTranslationResult()) = client.GetTranslationResultAsync(rozettaApiUser, queueId)
            task.Wait(Interval)

            If task.Result.Length > 0 Then

                For Each result As TextTranslationResult In task.Result
                    Console.WriteLine(String.Format("source:{0},translated:{1}", result.sourceText, result.translatedText))
                Next

                result = task.Result
                done = True
                StopTimer(aTimer)
            End If
        End Sub

        Private Sub StopTimer(ByVal timer As Timers.Timer)
            aTimer.Enabled = False
            aTimer.Stop()
            aTimer.Dispose()
        End Sub
    End Class
End Namespace
