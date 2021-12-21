Imports Newtonsoft.Json
Imports System.IO

Namespace common
    Public Class UserInfo
        Private Shared _ROZETTA_API_ACCESS_KEY As String
        Private Shared _ROZETTA_API_SECRET_KEY As String
        Private Shared _FILE_CONTRACT_ID As String
        Private Shared _TEXT_CONTRACT_ID As String

        Public Shared Sub init(ByVal filePath As String)
            Dim text As String = File.ReadAllText(filePath)
            Dim definition = New With {
                .ROZETTA_API_ACCESS_KEY = "",
                .ROZETTA_API_SECRET_KEY = "",
                .FILE_CONTRACT_ID = "",
                .TEXT_CONTRACT_ID = ""
            }
            Dim obj = JsonConvert.DeserializeAnonymousType(text, definition)
            ROZETTA_API_ACCESS_KEY = obj.ROZETTA_API_ACCESS_KEY
            ROZETTA_API_SECRET_KEY = obj.ROZETTA_API_SECRET_KEY
            FILE_CONTRACT_ID = obj.FILE_CONTRACT_ID
            TEXT_CONTRACT_ID = obj.TEXT_CONTRACT_ID
        End Sub

        Public Shared Property ROZETTA_API_ACCESS_KEY As String
            Get
                Return _ROZETTA_API_ACCESS_KEY
            End Get
            Private Set(ByVal value As String)
                _ROZETTA_API_ACCESS_KEY = value
            End Set
        End Property

        Public Shared Property ROZETTA_API_SECRET_KEY As String
            Get
                Return _ROZETTA_API_SECRET_KEY
            End Get
            Private Set(ByVal value As String)
                _ROZETTA_API_SECRET_KEY = value
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
