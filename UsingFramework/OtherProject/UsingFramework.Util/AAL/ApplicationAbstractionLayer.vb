Imports System.Runtime.CompilerServices

Namespace ApplicationAbstractionLayer

    Public NotInheritable Class API
        Public Shared Property OriginForeColor As ConsoleColor = ConsoleColor.Black
        Public Shared Property OriginBackColor As ConsoleColor = ConsoleColor.White
        Public Shared Sub InitConsole(Optional forecolor As ConsoleColor = ConsoleColor.Black, Optional backcolor As ConsoleColor = ConsoleColor.White)
            OriginForeColor = forecolor
            OriginBackColor = backcolor
            ReSetColor()
            Clear()
        End Sub
        Public Shared Sub ReSetColor()
            Console.ForegroundColor = OriginForeColor
            Console.BackgroundColor = OriginBackColor
        End Sub
        Public Shared Sub Clear()
            Console.Clear()
        End Sub
        Public Shared Sub WriteLine(t As String, Optional forecolor As ConsoleColor = ConsoleColor.Black, Optional backcolor As ConsoleColor = ConsoleColor.White)
            Console.ForegroundColor = forecolor
            Console.BackgroundColor = backcolor
            Console.WriteLine(t)
            ReSetColor()
        End Sub
        Public Shared Sub Write(t As String, Optional forecolor As ConsoleColor = ConsoleColor.Black, Optional backcolor As ConsoleColor = ConsoleColor.White)
            Console.ForegroundColor = forecolor
            Console.BackgroundColor = backcolor
            Console.Write(t)
            ReSetColor()
        End Sub

    End Class
    Public Module ExcpFunc
        <Extension>
        Public Function ToRequireToken(json As String) As RequireToken
            Return JsonConvert.DeserializeObject(Of RequireToken)(json)
        End Function
        <Extension>
        Public Function FromRequireToken(classt As RequireToken) As String
            Return JsonConvert.SerializeObject(classt)
        End Function

    End Module
End Namespace