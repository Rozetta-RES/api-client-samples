
Imports common.common

Namespace TextTranslationDemo
    Module main
        Sub Main()
            MainAsync().GetAwaiter().GetResult()
        End Sub
        Private Async Function MainAsync() As Task
            UserInfo.init("config.json")
            Dim baseUrl As String = "https://translate.classiii.info/api/v1"
            Dim classiiiUser As ClassiiiUser = New ClassiiiUser With {
                .AccessKey = UserInfo.CLASSIII_ACCESS_KEY,
                .SecretKey = UserInfo.CLASSIII_SECRET_KEY,
                .FileContractId = UserInfo.FILE_CONTRACT_ID,
                .TextContractId = UserInfo.TEXT_CONTRACT_ID
            }
            ' テキスト翻訳を試す
            Await TestTextTranslation(baseUrl, classiiiUser)
            ' ユーザー辞書を試す
            Await TestUserDictionary(baseUrl, classiiiUser)
        End Function

        Private Async Function TestTextTranslation(ByVal baseUrl As String, ByVal classiiiUser As ClassiiiUser) As Task
            Dim text As String() = New String() {"hello"}
            Dim [option] As TextTranslationOption = New TextTranslationOption With {
                .FieldId = 1,
                .TargetLang = "ja",
                .SourceLang = "en"
            }
            Dim textTranslationClient As TextTranslationClient = New TextTranslationClient(baseUrl)
            '  同期テキスト翻訳
            Dim ret As TextTranslationResult() = Await textTranslationClient.TranslateTextBySyncModeAsync(classiiiUser, [option], text)
            Debug.Assert(ret.Length = 1)
            Debug.Assert(Equals(ret(0).sourceText, "hello"))
            Debug.Assert(Equals(ret(0).translatedText, "こんにちは"))


            '  非同期テキスト翻訳
            Dim flow As TranslateTextByAsyncModeFlow = New TranslateTextByAsyncModeFlow(baseUrl)
            ret = Await flow.TranslateFlowAsync(classiiiUser, [option], text)
            Debug.Assert(ret.Length = 1)
            Debug.Assert(Equals(ret(0).sourceText, "hello"))
            Debug.Assert(Equals(ret(0).translatedText, "こんにちは"))
        End Function
        ' ※T4OO user can not use classiii dictionary function
        Private Async Function TestUserDictionary(ByVal baseUrl As String, ByVal classiiiUser As ClassiiiUser) As Task
            Dim userDictionaryClient As UserDictionaryClient = New UserDictionaryClient(baseUrl)
            Dim item As UserDictionaryItem = New UserDictionaryItem With {
                .fromLang = "en",
                .fromText = "hello",
                .toLang = "ja",
                .toText = "おはよう"
            }
            Dim bRet As Boolean
            Dim text As String() = New String() {"hello"}
            Dim [option] As TextTranslationOption = New TextTranslationOption With {
                .FieldId = 1,
                .SourceLang = "en",
                .TargetLang = "ja"
            }
            Dim textTranslationClient As TextTranslationClient = New TextTranslationClient(baseUrl)
            Dim textTranslationResults As TextTranslationResult()
            Dim userDictionaryItems As UserDictionaryItem()

            ' ユーザー辞書を登録
            bRet = Await userDictionaryClient.AddUserDictionaryItemAsync(classiiiUser, item)
            Debug.Assert(bRet)

            ' ユーザー辞書を取得
            userDictionaryItems = Await userDictionaryClient.GetUserDictionaryAsync(classiiiUser)
            Debug.Assert(userDictionaryItems.Length = 1)
            Debug.Assert(userDictionaryItems(0) = item)

            ' ユーザー辞書を確認 -----------------------------------------------------             
            textTranslationResults = Await textTranslationClient.TranslateTextBySyncModeAsync(classiiiUser, [option], text)
            Debug.Assert(textTranslationResults.Length = 1)
            Debug.Assert(Equals(textTranslationResults(0).sourceText, "hello"))
            Debug.Assert(Equals(textTranslationResults(0).translatedText, "おはよう"))
            ' ユーザー辞書を確認 ----------------------------------------------------- end

            ' ユーザー辞書を更新
            Dim itemNew As UserDictionaryItem = New UserDictionaryItem With {
                .fromLang = "en",
                .fromText = "hello",
                .toLang = "ja",
                .toText = "こんにちは"
            }
            bRet = Await userDictionaryClient.UpdateUserDictionaryItemAsync(classiiiUser, userDictionaryItems(0).id, itemNew)
            Debug.Assert(bRet)

            ' ユーザー辞書を確認 -----------------------------------------------------             
            textTranslationResults = Await textTranslationClient.TranslateTextBySyncModeAsync(classiiiUser, [option], text)
            Debug.Assert(textTranslationResults.Length = 1)
            Debug.Assert(Equals(textTranslationResults(0).sourceText, "hello"))
            Debug.Assert(Equals(textTranslationResults(0).translatedText, "こんにちは"))
            ' ユーザー辞書を確認 ----------------------------------------------------- end

            ' ユーザー辞書を削除
            bRet = Await userDictionaryClient.DeleteUserDictionaryItemAsync(classiiiUser, userDictionaryItems(0).id)
            Debug.Assert(bRet)

            ' ユーザー辞書数を確認
            userDictionaryItems = Await userDictionaryClient.GetUserDictionaryAsync(classiiiUser)
            Debug.Assert(userDictionaryItems.Length = 0)
        End Function
    End Module
End Namespace
