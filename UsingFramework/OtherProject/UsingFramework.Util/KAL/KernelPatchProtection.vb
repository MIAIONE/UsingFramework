
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports UsingFramework.Util.ApplicationAbstractionLayer

Namespace KernelAbstractionLayer
    Public Module ExcpFunc
        <Extension>
        Public Function ToRequireToken(json As String) As RequireToken
            Return JsonConvert.DeserializeObject(Of RequireToken)(json)
        End Function
        <Extension>
        Public Function FromRequireToken(classt As RequireToken) As String
            Return JsonConvert.SerializeObject(classt)
        End Function
        <Extension>
        Public Function ToAppToken(json As String) As ApplicationPermissionSet
            Return JsonConvert.DeserializeObject(Of ApplicationPermissionSet)(json)
        End Function
        <Extension>
        Public Function FromAppToken(classt As ApplicationPermissionSet) As String
            Return JsonConvert.SerializeObject(classt)
        End Function
    End Module
    Public NotInheritable Class KernelSecurity

        Public NotInheritable Class PatchGuard
            Public Shared Function GetSafeValue(value As String) As String
                If String.IsNullOrEmpty(value) Then Return String.Empty
                For Each chars As Char In My.Resources.SpChars
                    value.Replace(chars.ToString, String.Empty)
                Next
                Return value
            End Function
            Public Shared Function GetMD5(path As String) As String
                If Not File.Exists(path) Then Return Nothing
                Dim fs As FileStream = New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim md5Provider As MD5CryptoServiceProvider = New MD5CryptoServiceProvider()
                Dim buffer As Byte() = md5Provider.ComputeHash(fs)
                Dim resule As String = BitConverter.ToString(buffer)
                resule = resule.Replace("-", "")
                md5Provider.Clear()
                fs.Close()
                Return resule.ToUpper
            End Function

            Public Shared Function LoadAssembly(ApplicationPath As String, perlist As List(Of IPermission), Optional opt As LoaderOptimization = LoaderOptimization.MultiDomain, Optional allowdownloaddll As Boolean = False) As AppProcTask
                Return KernelPatchProtection.KpCreate(ApplicationPath, perlist, opt, allowdownloaddll)
            End Function
            Public Shared Sub UnLoadAssembly(ApplicatonDomain As AppProcTask)
                KernelPatchProtection.DisposeAppDomain(ApplicatonDomain.AppDomainControl)
            End Sub
            Public Shared Sub AddPermission(<Out> per As PermissionSet, token As IPermission)
                KernelPatchProtection.AddPermission(per, token)
            End Sub
            Public Shared Sub AddPermissionGroup(<Out> per As PermissionSet, perlist As List(Of IPermission))
                For Each Token As IPermission In perlist
                    KernelPatchProtection.AddPermission(per, Token)
                Next
            End Sub
            Public Shared Sub RemovePermission(<Out> per As PermissionSet, token As IPermission)
                KernelPatchProtection.RemovePermission(per, token)
            End Sub
            Public Shared Function GetBasePermissionList(RT As RequireToken) As List(Of IPermission)

                Dim resultList As New List(Of IPermission)
                For Each basetoken As BasePermissionItem In RT.RequirePermission.BasePermission
                    resultList.Add(GetSelectPerSet(basetoken.Token, GetUserChecked(GetSelectPerSet(basetoken.Token, False), RT.AppName)))
                Next
                Return resultList
            End Function
            Public Shared Function GetSystemDefineToken(Aps As ApplicationPermissionSet) As List(Of IPermission)

                Dim resultList As New List(Of IPermission)
                For Each basetoken As PermissionSetItem In Aps.PermissionSet
                    resultList.Add(GetSelectPerSet(GetPermissionsByString(basetoken.TokenName), Boolean.Parse(basetoken.State)))
                Next
                Return resultList
            End Function
            Public Shared Function GetUserChecked(perset As IPermission, appname As String) As Boolean
                Dim result As MsgBoxResult = MsgBox("是否允许 『 " + appname + " 』 获取 " + GetPerSetByName(perset.ToXml.Attribute("class").Split(",")(0).Trim) + " 的权限", MsgBoxStyle.MsgBoxSetForeground + vbYesNo, "权限管理")
                If result = MsgBoxResult.Yes Then
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetUnrestrictedState(evtoken As IPermission) As Boolean
                Return Boolean.Parse(evtoken.ToXml.Attribute("Unrestricted") IsNot Nothing)
            End Function
            Public Shared Sub RevisePermission(RT As RequireToken)
                Dim userAllowPerset As List(Of IPermission) = GetBasePermissionList(RT)
                SavePermission(RT, userAllowPerset)
            End Sub
            Public Shared Sub SavePermission(RT As RequireToken, Token As List(Of IPermission))
                Dim appSettings As ApplicationPermissionSet = New ApplicationPermissionSet With {
                    .RequirePermission = RT.RequirePermission
                }
                Dim pstlist As New List(Of PermissionSetItem)
                For Each evtoken As IPermission In Token
                    Dim pst As New PermissionSetItem With {
                        .TokenName = evtoken.ToXml.Attribute("class").Split(",")(0).Trim,
                        .State = GetUnrestrictedState(evtoken).ToString
                    }
                    pstlist.Add(pst)
                Next
                appSettings.PermissionSet = pstlist
                appSettings.AppName = RT.AppName
                appSettings.AppCOMName = RT.AppCOMName
                File.WriteAllText("ApplicationPermission\" + RT.GUID + ".json", appSettings.FromAppToken, Encoding.UTF8)
            End Sub
            Public Shared Function GetPermissionsByString(tokenstr As String) As Permissions
                Return PermissionToPerSet.Item(tokenstr)
            End Function
            Public Shared ReadOnly PermissionToPerSet As New Dictionary(Of String, Permissions) From {
            {"System.Security.Permissions.DataProtectionPermission", Permissions.DataProtectionPermission},
             {"System.Security.Permissions.EnvironmentPermission", Permissions.EnvironmentPermission},
             {"System.Security.Permissions.FileDialogPermission", Permissions.FileDialogPermission},
             {"System.Security.Permissions.FileIOPermission", Permissions.FileIOPermission},
             {"System.Security.Permissions.GacIdentityPermission", Permissions.GacIdentityPermission},
             {"System.Security.Permissions.IsolatedStorageFilePermission", Permissions.IsolatedStorageFilePermission},
             {"System.Security.Permissions.KeyContainerPermission", Permissions.KeyContainerPermission},
             {"System.Security.Permissions.MediaPermission", Permissions.MediaPermission},
             {"System.Security.Permissions.PrincipalPermission", Permissions.PrincipalPermission},
             {"System.Security.Permissions.PublisherIdentityPermission", Permissions.PublisherIdentityPermission},
             {"System.Security.Permissions.ReflectionPermission", Permissions.ReflectionPermission},
             {"System.Security.Permissions.RegistryPermission", Permissions.RegistryPermission},
             {"System.Security.Permissions.SecurityPermission", Permissions.SecurityPermission},
             {"System.Security.Permissions.SiteIdentityPermission", Permissions.SiteIdentityPermission},
             {"System.Security.Permissions.StorePermission", Permissions.StorePermission},
             {"System.Security.Permissions.StrongNameIdentityPermission", Permissions.StrongNameIdentityPermission},
             {"System.Security.Permissions.TypeDescriptorPermission", Permissions.TypeDescriptorPermission},
             {"System.Security.Permissions.UIPermission", Permissions.UIPermission},
             {"System.Security.Permissions.UrlIdentityPermission", Permissions.UrlIdentityPermission},
             {"System.Security.Permissions.WebBrowserPermission", Permissions.WebBrowserPermission},
             {"System.Security.Permissions.ZoneIdentityPermission", Permissions.ZoneIdentityPermission}
            }
            Public Shared ReadOnly PermissionInfo As New Dictionary(Of String, String) From {
             {"System.Security.Permissions.DataProtectionPermission", "访问、修改 『 加密数据和内存 』 "},
             {"System.Security.Permissions.EnvironmentPermission", "访问、修改 『 系统和用户环境变量 』 "},
             {"System.Security.Permissions.FileDialogPermission", "通过 『 文件对话框 』 访问、修改 『 文件或文件夹 』 "},
             {"System.Security.Permissions.FileIOPermission", "访问、修改 『 文件和文件夹 』 "},
             {"System.Security.Permissions.GacIdentityPermission", "访问、修改 『 全局程序集缓存中产生的文件的标识 』 "},
             {"System.Security.Permissions.IsolatedStorageFilePermission", "访问、修改 『 私有虚拟文件系统允许的用法 』 "},
             {"System.Security.Permissions.KeyContainerPermission", "访问、修改 『 访问密钥容器 』 "},
             {"System.Security.Permissions.MediaPermission", "让 『 音频、图像和视频媒体 』 在 『 不完全可信的应用程序 』 中运行"},
             {"System.Security.Permissions.PrincipalPermission", "使用 『 安全操作定义的语言构造 』 对 『 活动主体 』 执行检查"},
             {"System.Security.Permissions.PublisherIdentityPermission", "操作 『 软件发布者的标识 』 "},
             {"System.Security.Permissions.ReflectionPermission", "使用 『 其他程序集和相关操作 』 "},
             {"System.Security.Permissions.RegistryPermission", "访问、修改 『 注册表 』 "},
             {"System.Security.Permissions.SecurityPermission", "访问、修改 『 代码的安全权限 』 "},
             {"System.Security.Permissions.SiteIdentityPermission", "访问、修改 『 代码所源自的网站标识 』 "},
             {"System.Security.Permissions.StorePermission", "修改 『 X.509证书的存储区的操作权限 』 "},
             {"System.Security.Permissions.StrongNameIdentityPermission", "修改 『 强名称的标识的操作权限 』 "},
             {"System.Security.Permissions.TypeDescriptorPermission", "修改 『 TypeDescriptor的部分信任访问权限 』 "},
             {"System.Security.Permissions.UIPermission", "访问 『 用户界面和剪贴板 』 "},
             {"System.Security.Permissions.UrlIdentityPermission", "修改 『 代码来源URL标识 』 "},
             {"System.Security.Permissions.WebBrowserPermission", "修改 『 对象创建系统WebBrowser控件 』 "},
             {"System.Security.Permissions.ZoneIdentityPermission", "修改 『 代码来源区域的标识 』 "}
            }

            Public Shared Function GetPerSetByName(name As String) As String
                Return PermissionInfo.Item(name)
            End Function
            Public Shared Function GetSelectPerSet(Perset As Permissions, bool As Boolean) As IPermission
                Dim bools As PermissionState = GetPerSetState(bool)
                Select Case Perset
                    Case Permissions.DataProtectionPermission
                        Return New DataProtectionPermission(bools)
                    Case Permissions.EnvironmentPermission
                        Return New EnvironmentPermission(bools)
                    Case Permissions.FileDialogPermission
                        Return New FileDialogPermission(bools)
                    Case Permissions.FileIOPermission
                        Return New FileIOPermission(bools)
                    Case Permissions.GacIdentityPermission
                        Return New GacIdentityPermission(bools)
                    Case Permissions.IsolatedStorageFilePermission
                        Return New IsolatedStorageFilePermission(bools)
                    Case Permissions.KeyContainerPermission
                        Return New KeyContainerPermission(bools)
                    Case Permissions.MediaPermission
                        Return New MediaPermission(bools)
                    Case Permissions.PrincipalPermission
                        Return New PrincipalPermission(bools)
                    Case Permissions.PublisherIdentityPermission
                        Return New PublisherIdentityPermission(bools)
                    Case Permissions.ReflectionPermission
                        Return New ReflectionPermission(bools)
                    Case Permissions.RegistryPermission
                        Return New RegistryPermission(bools)
                    Case Permissions.SecurityPermission
                        Return New SecurityPermission(bools)
                    Case Permissions.SiteIdentityPermission
                        Return New SiteIdentityPermission(bools)
                    Case Permissions.StorePermission
                        Return New StorePermission(bools)
                    Case Permissions.StrongNameIdentityPermission
                        Return New StrongNameIdentityPermission(bools)
                    Case Permissions.TypeDescriptorPermission
                        Return New TypeDescriptorPermission(bools)
                    Case Permissions.UIPermission
                        Return New UIPermission(bools)
                    Case Permissions.UrlIdentityPermission
                        Return New UrlIdentityPermission(bools)
                    Case Permissions.WebBrowserPermission
                        Return New WebBrowserPermission(bools)
                    Case Permissions.ZoneIdentityPermission
                        Return New ZoneIdentityPermission(bools)
                    Case Else
                        Return New SecurityPermission(SecurityPermissionFlag.Execution)
                End Select
            End Function
            Public Shared Function GetPerSetState(bool As Boolean) As PermissionState
                If bool Then
                    Return PermissionState.Unrestricted
                Else
                    Return PermissionState.None
                End If
            End Function
        End Class
        Private NotInheritable Class KernelPatchProtection
            Public Shared Sub DisposeAppDomain(<Out> appdomains As AppDomain)
                If appdomains.IsFinalizingForUnload = False Then
                    AppDomain.Unload(appdomains)
                End If
            End Sub
            Public Shared Function GetNewPermission(perstate As Boolean) As PermissionSet
                If perstate Then
                    Return New PermissionSet(PermissionState.Unrestricted)
                Else
                    Return New PermissionSet(PermissionState.None)
                End If
            End Function
            Public Shared Sub AddPermission(<Out> per As PermissionSet, token As IPermission)
                per.AddPermission(token)
            End Sub
            Public Shared Sub RemovePermission(<Out> per As PermissionSet, token As IPermission)
                per.RemovePermission(token.GetType)
            End Sub

            Public Shared Function GetNewEvidence() As Evidence
                Dim EvidenceNew As New Evidence()
                SecurityManager.GetStandardSandbox(EvidenceNew)
                Return EvidenceNew
            End Function
            Public Shared Function GetNewDomain(Optional opt As LoaderOptimization = LoaderOptimization.MultiDomain, Optional allowdownloaddll As Boolean = False) As AppDomainSetup
                Dim Appdomain As New AppDomainSetup With {
                    .ApplicationBase = Application.StartupPath,
                    .LoaderOptimization = opt,
                    .DisallowCodeDownload = allowdownloaddll
                }
                Return Appdomain
            End Function
            Public Shared Function CreateAppDomain(name As String, ev As Evidence, appsetup As AppDomainSetup, ps As PermissionSet) As AppDomain
                Return AppDomain.CreateDomain(name, ev, appsetup, ps)
            End Function
            Public Shared Function KpCreate(ApplicationPath As String, perlist As List(Of IPermission), Optional opt As LoaderOptimization = LoaderOptimization.MultiDomain, Optional allowdownloaddll As Boolean = False) As AppProcTask
                Dim PerSet As PermissionSet = GetNewPermission(False)
                AddPermission(PerSet, New SecurityPermission(SecurityPermissionFlag.AllFlags))
                AddPermission(PerSet, New UIPermission(UIPermissionWindow.AllWindows, UIPermissionClipboard.AllClipboard))
                AddPermission(PerSet, New FileIOPermission(FileIOPermissionAccess.AllAccess, New String() {Path.GetDirectoryName(ApplicationPath), Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + "\ApplicationPermission")}))

                For Each Token As IPermission In perlist
                    AddPermission(PerSet, Token)
                Next
                Dim appsetup As AppDomainSetup = GetNewDomain(opt, allowdownloaddll)
                Dim Evidences As Evidence = GetNewEvidence()
                Dim AssemblyBinaryDomain As AppDomain = CreateAppDomain(ApplicationPath, Evidences, appsetup, PerSet)
                Dim forbi As Boolean = True
                Dim Expcx As SecurityException = Nothing
                Dim AssemblyLoader As AssemblyDynamicLoader = New AssemblyDynamicLoader(AssemblyBinaryDomain)
                Try
                    AssemblyLoader.LoadAssembly(ApplicationPath)
                Catch Exp As SecurityException
                    forbi = False
                    Expcx = Exp
                End Try
                Return New AppProcTask(AssemblyBinaryDomain, PerSet, forbi, Expcx, AssemblyLoader)
            End Function

        End Class
        Public Class AssemblyDynamicLoader
            Private appDomain As AppDomain
            Private LremoteLoader As RemoteLoader

            Public Sub New(appDomainx As AppDomain)
                appDomain = appDomainx
                Dim name As String = Assembly.GetExecutingAssembly().GetName().FullName
                LremoteLoader = CType(appDomain.CreateInstanceAndUnwrap(name, GetType(RemoteLoader).FullName), RemoteLoader)
            End Sub

            Public Sub LoadAssembly(assemblyFile As String)
                LremoteLoader.LoadAssembly(assemblyFile)
            End Sub

            Public Function GetInstance(Of T As Class)(typeName As String) As T
                If LremoteLoader Is Nothing Then Return Nothing
                Return LremoteLoader.GetInstance(Of T)(typeName)
            End Function

            Public Function Run(args As String) As Object
                Return LremoteLoader.Init(args)
            End Function

            Public Sub Unload()
                Try
                    If appDomain Is Nothing Then Return
                    AppDomain.Unload(appDomain)
                    appDomain = Nothing
                    LremoteLoader = Nothing
                Catch ex As CannotUnloadAppDomainException
                End Try
            End Sub

            Public Function GetAssemblies() As Assembly()
                Return appDomain.GetAssemblies()
            End Function
            Public Class RemoteLoader
                Inherits MarshalByRefObject

                Private _assembly As Assembly

                Public Sub LoadAssembly(assemblyFile As String)
                    _assembly = Assembly.LoadFrom(assemblyFile)
                End Sub

                Public Function GetInstance(Of T As Class)(typeName As String) As T
                    If _assembly Is Nothing Then Return Nothing
                    Dim type = _assembly.[GetType](typeName)
                    If type Is Nothing Then Return Nothing
                    Return Activator.CreateInstance(type)
                End Function

                Public Function Init(args As String) As Object
                    If _assembly Is Nothing Then Return Nothing
                    Dim wormMain = _assembly.GetTypes().FirstOrDefault(Function(m) m.GetInterface(GetType(IPlugin).Name) IsNot Nothing)
                    Dim tmpObj = CType(Activator.CreateInstance(wormMain), IPlugin)
                    Return tmpObj.Init(args)
                End Function
            End Class
        End Class
        Public Class AppProcTask
            Public ReadOnly Property AppDomainControl As AppDomain
            Public ReadOnly Property PermissionControl As PermissionSet
            Public ReadOnly Property SecurityAccessState As Boolean
            Public ReadOnly Property RequirePermission As SecurityException
            Public ReadOnly Property AssemblyLoader As AssemblyDynamicLoader
            Public Sub New(domain As AppDomain, perset As PermissionSet, securityas As Boolean, Expcx As SecurityException, ADL As AssemblyDynamicLoader)
                AppDomainControl = domain
                PermissionControl = perset
                SecurityAccessState = securityas
                RequirePermission = Expcx
                AssemblyLoader = ADL
            End Sub
        End Class
    End Class

End Namespace