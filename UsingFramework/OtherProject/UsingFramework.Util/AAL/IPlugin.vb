Namespace ApplicationAbstractionLayer
    ''' <summary>
    ''' 开放接口
    ''' </summary>
    Public Interface IPlugin
        ''' <summary>
        ''' 应用程序入口点
        ''' </summary>
        ''' <param name="args">参数</param>
        ''' <returns>有无异常、正常退出</returns>
        Function OpenEntryPoint(args As String) As Boolean
    End Interface
End Namespace
