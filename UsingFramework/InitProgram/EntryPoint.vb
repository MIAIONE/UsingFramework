''' <summary>
''' 系统内核入口点
''' </summary>
Public Class EntryPoint
    ''' <summary>
    ''' 主函数
    ''' </summary> 
    <MTAThread>
    Public Shared Sub Main()
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler Application.ThreadException, New ThreadExceptionEventHandler(AddressOf KeBugCheckThread)
        AddHandler AppDomain.CurrentDomain.UnhandledException, New System.UnhandledExceptionEventHandler(AddressOf KeBugCheckGlobal)
        Application.VisualStyleState = VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        API.InitConsole()


        API.WriteLine("Boot Kernel...")
        If Directory.Exists("Data") = False Then
            Directory.CreateDirectory("Data")
        End If
        If Directory.Exists("ApplicationPermission") = False Then
            Directory.CreateDirectory("ApplicationPermission")
        End If
        Dim pid As UInteger = 0
        Dim TaskListGrp As New KernelTask.TaskList
        Dim appPathDir As String = Application.StartupPath + "\Data"
        Dim folder = Directory.GetDirectories(appPathDir)
        For Each everyFolder As String In folder
            For Each files As String In Directory.GetFiles(everyFolder)
                If Path.GetFileName(files).ToLower = "applicationpermission.json" Then
                    pid += 1
                    Dim filebasePath As String = Path.GetDirectoryName(files)
                    Dim RT As RequireToken = File.ReadAllText(files).ToRequireToken
                    RT.AppName = KernelSecurity.PatchGuard.GetSafeValue(RT.AppName)
                    Dim appFilePath As String = filebasePath + "\" + RT.AppFiles
                    Dim tokenR As List(Of IPermission)
                    If File.Exists("ApplicationPermission\" + RT.GUID + ".json") Then
                        Dim appPerset As ApplicationPermissionSet = File.ReadAllText("ApplicationPermission\" + RT.GUID + ".json").ToAppToken
                        tokenR = KernelSecurity.PatchGuard.GetSystemDefineToken(appPerset)
                    Else
                        tokenR = KernelSecurity.PatchGuard.GetBasePermissionList(RT)
                        KernelSecurity.PatchGuard.SavePermission(RT, tokenR)
                    End If


                    Dim tData = KernelTask.CreateKernelTask(appFilePath, tokenR)
                        tData.Wait()
                        tData.Result.AssemblyLoader.Run("----------")
                        Dim TaskQueryT As New KernelTask.TaskQuery(pid, RT.AppName)
                        TaskListGrp.Add(TaskQueryT, tData)
                    End If
            Next
        Next


        'KernelSecurity.PatchGuard.LoadAssembly("D:\OSX 2\UsingFramework\OtherProject\AppTest\bin\Release\AppTest.dll", n)

        'API.WriteLine(tData.Result.RequirePermission.PermissionType.ToString)
        Application.Run()
    End Sub
    Private Shared Sub KeBugCheckThread(sender As Object, e As ThreadExceptionEventArgs)
        Dim str As String = GetExceptionMsg(e.Exception, e.ToString())
        MessageBox.Show(str, "线程错误", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    End Sub

    Private Shared Sub KeBugCheckGlobal(sender As Object, e As System.UnhandledExceptionEventArgs)
        Dim str As String = GetExceptionMsg(TryCast(e.ExceptionObject, Exception), e.ToString())
        MessageBox.Show(str, "全局错误", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    End Sub
    Private Shared Function GetExceptionMsg(ex As Exception, backStr As String) As String
        Dim sb As StringBuilder = New StringBuilder()
        sb.AppendLine("****************************异常信息****************************")
        sb.AppendLine("【出现时间】：" & Date.Now.ToString())

        If ex IsNot Nothing Then
            sb.AppendLine("【异常类型】：" & ex.[GetType]().Name)
            sb.AppendLine("【异常信息】：" & ex.Message)
            sb.AppendLine("【堆栈调用】：" & ex.StackTrace)
        Else
            sb.AppendLine("【未处理异常】：" & backStr)
        End If

        sb.AppendLine("***************************************************************")
        Return sb.ToString()
    End Function
End Class
