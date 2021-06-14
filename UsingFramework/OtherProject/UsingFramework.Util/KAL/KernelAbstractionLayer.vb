'内核抽象层
Namespace KernelAbstractionLayer

    ''' <summary>
    ''' 内核任务调度
    ''' </summary>
    Public NotInheritable Class KernelTask

        ''' <summary>
        ''' 任务信息
        ''' </summary>
        Public NotInheritable Class TaskQuery
            ''' <summary>
            ''' 任务标识
            ''' </summary>
            ''' <returns>标识</returns>
            Public ReadOnly Property PID As UInteger
            ''' <summary>
            ''' 应用程序名称
            ''' </summary>
            ''' <returns>名称字符串</returns>
            Public ReadOnly Property AppName As String
            ''' <summary>
            ''' 任务列表
            ''' </summary>
            ''' <param name="PIDs">任务标识</param>
            ''' <param name="Appnames">应用程序名称</param>
            Sub New(PIDs As UInteger, Appnames As String)
                PID = PIDs
                AppName = Appnames
            End Sub
        End Class
        ''' <summary>
        ''' 任务列表
        ''' </summary>
        Public NotInheritable Class TaskList
            Inherits Dictionary(Of TaskQuery, Task(Of KernelSecurity.AppProcTask))
        End Class
        ''' <summary>
        ''' 创建新任务
        ''' </summary>
        ''' <param name="ApplicationPath">应用程序路径</param>
        ''' <returns>应用程序主函数实例</returns>
        Public Shared Function CreateKernelTask(ApplicationPath As String, perlist As List(Of IPermission), Optional opt As LoaderOptimization = LoaderOptimization.MultiDomain, Optional allowdownloaddll As Boolean = False) As Task(Of KernelSecurity.AppProcTask)
            Dim task As TaskFactory(Of KernelSecurity.AppProcTask) = New TaskFactory(Of KernelSecurity.AppProcTask)
            Dim TaskMain = task.StartNew(Function()
                                             Return KpCreateKernelTask(ApplicationPath, perlist, opt, allowdownloaddll)
                                         End Function)
            Return TaskMain
        End Function


        ''' <summary>
        ''' 内核函数：创建新任务
        ''' </summary>
        ''' <param name="ApplicationPath">应用程序路径</param> 
        Protected Friend Shared Function KpCreateKernelTask(ApplicationPath As String, perlist As List(Of IPermission), Optional opt As LoaderOptimization = LoaderOptimization.MultiDomain, Optional allowdownloaddll As Boolean = False) As KernelSecurity.AppProcTask
            Return KernelSecurity.PatchGuard.LoadAssembly(ApplicationPath, perlist, opt, allowdownloaddll)
        End Function
    End Class
    ''' <summary>
    ''' 内核服务控制
    ''' </summary>
    Public NotInheritable Class KernelService
        ''' <summary>
        ''' 安装服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <param name="serviceFilePath">服务路径</param>
        ''' <returns>是否成功</returns>
        Public Shared Function InstallService(serviceName As String, serviceFilePath As String) As Boolean
            Dim Result As Boolean
            If KpIsServiceExisted(serviceName) Then
                KpUninstallService(serviceName)
            End If
            Result = KpInstallService(serviceFilePath)
            Return Result
        End Function
        ''' <summary>
        ''' 启动服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <returns>是否成功</returns>
        Public Shared Function StartService(serviceName As String) As Boolean
            Dim Result As Boolean
            If KpIsServiceExisted(serviceName) Then
                Result = KpServiceStart(serviceName)
            End If
            Return Result
        End Function
        ''' <summary>
        ''' 停止服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <returns>是否成功</returns>
        Public Shared Function StopService(serviceName As String) As Boolean
            Dim Result As Boolean
            If KpIsServiceExisted(serviceName) Then
                Result = KpServiceStop(serviceName)
            End If
            Return Result
        End Function
        ''' <summary>
        ''' 卸载服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <param name="serviceFilePath">服务路径</param>
        ''' <returns>是否成功</returns>
        Public Shared Function UninstallService(serviceName As String, serviceFilePath As String) As Boolean
            Dim Result As Boolean
            If KpIsServiceExisted(serviceName) Then
                Result = KpServiceStop(serviceName) And KpUninstallService(serviceFilePath)
            End If
            Return Result
        End Function
        ''' <summary>
        ''' 内核函数：服务是否存在
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <returns>是否存在</returns>
        Protected Friend Shared Function KpIsServiceExisted(serviceName As String) As Boolean
            On Error Resume Next
            Dim services As ServiceController() = ServiceController.GetServices()

            For Each sc As ServiceController In services

                If sc.ServiceName.ToLower() = serviceName.ToLower() Then
                    Return True
                End If
            Next

            Return False
        End Function
        ''' <summary>
        ''' 内核函数：安装服务
        ''' </summary>
        ''' <param name="serviceFilePath">服务路径</param>
        ''' <returns>是否成功</returns>
        Protected Friend Shared Function KpInstallService(serviceFilePath As String) As Boolean
            Try
                Using installer As AssemblyInstaller = New AssemblyInstaller()
                    installer.UseNewContext = True
                    installer.Path = serviceFilePath
                    Dim savedState As IDictionary = New Hashtable()
                    installer.Install(savedState)
                    installer.Commit(savedState)
                End Using
                Return True
            Catch
                Return False
            End Try
        End Function
        ''' <summary>
        ''' 内核函数：卸载服务
        ''' </summary>
        ''' <param name="serviceFilePath">服务路径</param>
        ''' <returns>是否成功</returns>
        Protected Friend Shared Function KpUninstallService(serviceFilePath As String) As Boolean
            Try
                Using installer As AssemblyInstaller = New AssemblyInstaller()
                    installer.UseNewContext = True
                    installer.Path = serviceFilePath
                    installer.Uninstall(Nothing)
                End Using
                Return True
            Catch
                Return False
            End Try
        End Function
        ''' <summary>
        ''' 内核函数：启动服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <param name="Args">参数</param>
        ''' <returns>是否成功</returns>
        Protected Friend Shared Function KpServiceStart(serviceName As String, Optional Args As String() = Nothing) As Boolean
            Try
                Using control As ServiceController = New ServiceController(serviceName)
                    If (control.Status = (ServiceControllerStatus.Stopped Or ServiceControllerStatus.Paused)) Then
                        control.Start(Args)
                    End If
                End Using
                Return True
            Catch
                Return False
            End Try
        End Function
        ''' <summary>
        ''' 内核函数：暂停服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <returns>是否成功</returns>
        Protected Friend Shared Function KpServicePause(serviceName As String) As Boolean
            Try
                Using control As ServiceController = New ServiceController(serviceName)
                    If (control.Status = ServiceControllerStatus.Running) Then
                        control.Pause()
                    End If
                End Using
                Return True
            Catch
                Return False
            End Try
        End Function
        ''' <summary>
        ''' 内核函数：停止服务
        ''' </summary>
        ''' <param name="serviceName">服务名称</param>
        ''' <returns>是否成功</returns>
        Protected Friend Shared Function KpServiceStop(serviceName As String) As Boolean
            Try
                Using control As ServiceController = New ServiceController(serviceName)
                    If (control.Status <> (ServiceControllerStatus.Stopped Or ServiceControllerStatus.StopPending)) Then
                        control.[Stop]()
                    End If
                End Using
                Return True
            Catch
                Return False
            End Try
        End Function
    End Class
End Namespace