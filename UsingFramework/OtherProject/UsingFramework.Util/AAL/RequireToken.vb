Public Class RequireToken
    Public Property RequirePermission As RequirePermission
    Public Property AppFiles As String
    Public Property AppName As String
    Public Property GUID As String
    Public Property AppCOMName As String
End Class
Public Class BasePermissionItem
    Public Property Token As Integer
End Class
Public Class RequirePermission
    Public Property BasePermission As List(Of BasePermissionItem)
End Class

Public Class PermissionSetItem
    Public Property TokenName As String
    Public Property State As String
End Class

Public Class ApplicationPermissionSet
    Public Property RequirePermission As RequirePermission
    Public Property AppName As String
    Public Property AppCOMName As String
    Public Property PermissionSet As List(Of PermissionSetItem)
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
