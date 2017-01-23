Imports System.Runtime.CompilerServices
Imports MongoDB.Driver

Public Module ExtensionsIFindFluent

    <Extension>
    Public Sub InsertThese(Of T)(items As IFindFluent(Of T, T))
        'this looks like a little nonsense, because these objects actually come from MongoDB.
        'but someone needs insert same data again, maybe?
        MongoActive.InsertMany(Of T)(items.ToEnumerable)
    End Sub

    <Extension>
    Public Function UpdateThese(Of T)(items As IFindFluent(Of T, T), updatedef As UpdateDefinition(Of T)) As UpdateResult
        Return MongoActive.UpdateMany(Of T)(items.ToEnumerable, updatedef)
    End Function

    <Extension>
    Public Function DeleteThese(Of T)(items As IFindFluent(Of T, T)) As DeleteResult
        Return MongoActive.DeleteMany(Of T)(items.ToEnumerable)
    End Function

End Module
