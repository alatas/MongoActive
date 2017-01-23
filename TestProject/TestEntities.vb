Imports alatas.MongoActive

<MongoActive()>
Public Class SimpleEntity
    Inherits MongoActiveBaseGeneric(Of SimpleEntity)

    Public Property TestKey As String
    Public Property SortKey As Long = Now.Ticks
End Class

<MongoActive()>
Public Class InheritedEntity
    Inherits SimpleEntity
    Public Property InheritKey As String = Now.ToString

End Class

<MongoActive()>
Public Class ReInheritedEntity
    Inherits InheritedEntity

    Public Property Inherit2ndKey As String = Now.ToString
End Class
