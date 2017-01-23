Imports MongoDB.Driver

<System.AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=False)>
Public Class MongoActiveAttribute
    Inherits Attribute

    Public Property DatabaseName As String = "Default"
    Public Property CollectionName As String = ""
    Public Property UseBaseForCollectionName As Boolean = False
    Public Property ClientFactoryType As String = ""
    Public Property CleanTypeName As Boolean = True
End Class