Public Class RequireToken
    Public Property RequirePermission As RequirePermission
    Public Property AppFiles As String
    Public Property AppName As String

End Class
Public Class BasePermissionItem
    Public Property Token As Integer
End Class

Public Class OptionalPermissionItem
    Public Property Token As Integer
End Class

Public Class RequirePermission
    Public Property BasePermission As List(Of BasePermissionItem)
    Public Property OptionalPermission As List(Of OptionalPermissionItem)
End Class
Public Enum Permissions As Integer
    DataProtectionPermission
    EnvironmentPermission
    FileDialogPermission
    FileIOPermission
    GacIdentityPermission
    IsolatedStorageFilePermission
    KeyContainerPermission
    MediaPermission
    PrincipalPermission
    PublisherIdentityPermission
    ReflectionPermission
    RegistryPermission
    SecurityPermission
    SiteIdentityPermission
    StorePermission
    StrongNameIdentityPermission
    TypeDescriptorPermission
    UIPermission
    UrlIdentityPermission
    WebBrowserPermission
    ZoneIdentityPermission
End Enum
