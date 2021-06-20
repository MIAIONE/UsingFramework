
Public Class TestMain
    Implements IPlugin
    Public Function OpenEntryPoint(args As String) As Boolean Implements IPlugin.OpenEntryPoint
        API.TryGetPermission("Test")
        API.WriteLine("Loading Program ")
        '这里我测试了MMD的第三方NUGET，不过貌似运行不了PMM文件，但是能够运行dll
        Dim project = New Project().Load(args + "sample.pmm")
        Dim vv As New Components.View
        vv.FpsLimit = 30
        project.View = vv
        project.Save("0.mp4")
        Return True
    End Function
End Class
