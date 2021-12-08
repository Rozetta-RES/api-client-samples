Imports common.common
Imports FileTranslationDemo.FileTranslationDemo

Namespace TextTranslationDemo
    Module main

        Sub Main()
            MainAsync().GetAwaiter().GetResult()
        End Sub

        Public Async Function MainAsync() As Task
            UserInfo.init("config.json")
            Dim flow As FileTranslateFlow = New FileTranslateFlow("https://translate.rozetta-api.io/api/v1")
            Dim files As String() = {"C:\mydocuments\テスト.docx"}
            Dim langs As String() = {"ja"}
            Dim fieldId As Integer = 1
            Dim done As Boolean = False
            Dim rozettaApiUser As RozettaApiUser = New RozettaApiUser With {
                .AccessKey = UserInfo.ROZETTA_API_ACCESS_KEY,
                .SecretKey = UserInfo.ROZETTA_API_SECRET_KEY,
                .FileContractId = UserInfo.FILE_CONTRACT_ID
            }
            done = Await flow.RozettaApiFlowAsync(rozettaApiUser, files, langs, fieldId)
            Debug.Assert(done)
        End Function
    End Module
End Namespace
