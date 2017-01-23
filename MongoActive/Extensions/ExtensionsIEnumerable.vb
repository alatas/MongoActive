Imports System.Runtime.CompilerServices
Imports MongoDB.Driver

Public Module ExtensionsIEnumerable

    <Extension>
    Public Sub InsertThese(Of T)(items As IEnumerable(Of T))
        MongoActive.InsertMany(Of T)(items)
    End Sub

    <Extension> Public Function UpdateThese(Of T)(items As IEnumerable(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
        Return MongoActive.UpdateMany(Of T)(items, updatedef)
    End Function

    <Extension> Public Function DeleteThese(Of T)(items As IEnumerable(Of T)) As DeleteResult
        Return MongoActive.DeleteMany(Of T)(items)
    End Function
End Module
