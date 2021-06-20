Public Class TestMain
    Implements IPlugin
    Public Function Init(args As String) As Boolean Implements IPlugin.Init
        API.TryGetPermission("Test")
        API.WriteLine("Loading Program " + args)

        Application.Run(New Form)
        Return True
    End Function
End Class
