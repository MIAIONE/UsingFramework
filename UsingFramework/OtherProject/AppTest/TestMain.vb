﻿
Public Class TestMain
    Implements IPlugin
    Public Function OpenEntryPoint(args As String) As Boolean Implements IPlugin.OpenEntryPoint
        API.TryGetPermission("Test")
        API.WriteLine("Loading Program " + args)

        Return True
    End Function
End Class
