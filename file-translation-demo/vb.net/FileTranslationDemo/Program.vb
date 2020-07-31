Imports common.common
Imports FileTranslationDemo.FileTranslationDemo

Namespace TextTranslationDemo
    Module main

        Sub Main()
            MainAsync().GetAwaiter().GetResult()
        End Sub

        Public Async Function MainAsync() As Task
            UserInfo.init("config.json")
            Dim flow As FileTranslateFlow = New FileTranslateFlow("https://translate.classiii.info/api/v1")
            Dim t4ooUser As T4ooUser = New T4ooUser With {
                .orgId = UserInfo.T4OO_ORG_ID,
                .userId = UserInfo.T4OO_USER_ID,
                .password = UserInfo.T4OO_PASSWORD
            }
            Dim files As String() = {"C:\mydocuments\テスト.docx"}
            Dim langs As String() = {"ja"}
            Dim fieldId As Integer = 1
            Dim done As Boolean = False
            'done = Await flow.T4ooFlowAsync(t4ooUser, files, langs, fieldId)
            'Debug.Assert(done)
            Dim classiiiUser As ClassiiiUser = New ClassiiiUser With {
                .AccessKey = UserInfo.CLASSIII_ACCESS_KEY,
                .SecretKey = UserInfo.CLASSIII_SECRET_KEY,
                .FileContractId = UserInfo.FILE_CONTRACT_ID,
                .TextContractId = UserInfo.TEXT_CONTRACT_ID
            }
            done = Await flow.ClassiiiFlowAsync(classiiiUser, files, langs, fieldId)
            Debug.Assert(done)
        End Function
    End Module
End Namespace
