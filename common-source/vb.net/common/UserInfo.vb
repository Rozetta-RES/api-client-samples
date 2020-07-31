Imports Newtonsoft.Json
Imports System.IO

Namespace common
    Public Class UserInfo
        Private Shared _T4OO_ORG_ID As String
        Private Shared _T4OO_USER_ID As String
        Private Shared _T4OO_PASSWORD As String
        Private Shared _CLASSIII_ACCESS_KEY As String
        Private Shared _CLASSIII_SECRET_KEY As String
        Private Shared _FILE_CONTRACT_ID As String
        Private Shared _TEXT_CONTRACT_ID As String

        Public Shared Sub init(ByVal filePath As String)
            Dim text As String = File.ReadAllText(filePath)
            Dim definition = New With {
                .T4OO_ORG_ID = "",
                .T4OO_USER_ID = "",
                .T4OO_PASSWORD = "",
                .CLASSIII_ACCESS_KEY = "",
                .CLASSIII_SECRET_KEY = "",
                .FILE_CONTRACT_ID = "",
                .TEXT_CONTRACT_ID = ""
            }
            Dim obj = JsonConvert.DeserializeAnonymousType(text, definition)
            T4OO_ORG_ID = obj.T4OO_ORG_ID
            T4OO_USER_ID = obj.T4OO_USER_ID
            T4OO_PASSWORD = obj.T4OO_PASSWORD
            CLASSIII_ACCESS_KEY = obj.CLASSIII_ACCESS_KEY
            CLASSIII_SECRET_KEY = obj.CLASSIII_SECRET_KEY
            FILE_CONTRACT_ID = obj.FILE_CONTRACT_ID
            TEXT_CONTRACT_ID = obj.TEXT_CONTRACT_ID
        End Sub

        Public Shared Property T4OO_ORG_ID As String
            Get
                Return _T4OO_ORG_ID
            End Get
            Private Set(ByVal value As String)
                _T4OO_ORG_ID = value
            End Set
        End Property

        Public Shared Property T4OO_USER_ID As String
            Get
                Return _T4OO_USER_ID
            End Get
            Private Set(ByVal value As String)
                _T4OO_USER_ID = value
            End Set
        End Property

        Public Shared Property T4OO_PASSWORD As String
            Get
                Return _T4OO_PASSWORD
            End Get
            Private Set(ByVal value As String)
                _T4OO_PASSWORD = value
            End Set
        End Property

        Public Shared Property CLASSIII_ACCESS_KEY As String
            Get
                Return _CLASSIII_ACCESS_KEY
            End Get
            Private Set(ByVal value As String)
                _CLASSIII_ACCESS_KEY = value
            End Set
        End Property

        Public Shared Property CLASSIII_SECRET_KEY As String
            Get
                Return _CLASSIII_SECRET_KEY
            End Get
            Private Set(ByVal value As String)
                _CLASSIII_SECRET_KEY = value
            End Set
        End Property
        Public Shared Property FILE_CONTRACT_ID As String
            Get
                Return _FILE_CONTRACT_ID
            End Get
            Private Set(ByVal value As String)
                _FILE_CONTRACT_ID = value
            End Set
        End Property
        Public Shared Property TEXT_CONTRACT_ID As String
            Get
                Return _TEXT_CONTRACT_ID
            End Get
            Private Set(ByVal value As String)
                _TEXT_CONTRACT_ID = value
            End Set
        End Property
    End Class
End Namespace
