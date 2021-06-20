Imports System.Runtime.CompilerServices
Imports UsingFramework.Util.KernelAbstractionLayer
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
        Public Shared Sub TryGetPermission(AppCOMName As String)
            'MsgBox(AppDomain.CurrentDomain.BaseDirectory + "\Data\" + KernelSecurity.PatchGuard.GetSafeValue(AppCOMName) + "\applicationpermission.json")
            Dim rt As RequireToken = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\Data\" + KernelSecurity.PatchGuard.GetSafeValue(AppCOMName) + "\applicationpermission.json", Encoding.UTF8).ToRequireToken
            rt.AppName = KernelSecurity.PatchGuard.GetSafeValue(rt.AppName)
            KernelSecurity.PatchGuard.RevisePermission(rt)
        End Sub
    End Class

End Namespace