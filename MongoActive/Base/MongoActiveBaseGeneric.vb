Imports MongoDB.Bson
Imports MongoDB.Driver

Public MustInherit Class MongoActiveBaseGeneric(Of T)
    Inherits MongoActiveBase

#Region "Param Factories"
    Public Shared Shadows Filter As FilterDefinitionBuilder(Of T) = Builders(Of T).Filter
    Public Shared Shadows Update As UpdateDefinitionBuilder(Of T) = Builders(Of T).Update
    Public Shared Shadows Sort As SortDefinitionBuilder(Of T) = Builders(Of T).Sort
#End Region

#Region "Initializers"
    Private Function GetInstanceFromMe() As T
        Return DirectCast(DirectCast(Me, Object), T)
    End Function

    Private Shared Function GetBaseInstance(item As T) As MongoActiveBaseGeneric(Of T)
        Return DirectCast(DirectCast(item, Object), MongoActiveBaseGeneric(Of T))
    End Function
#End Region

#Region "Get"
    Public Overloads Shared Function GetOne(Of TField)(IDFieldName As String, IDFieldValue As TField) As T
        Return MongoActive.GetOne(Of T, TField)(IDFieldName, IDFieldValue)
    End Function

    Public Overloads Shared Function GetOneByID(ID As ObjectId) As T
        Return MongoActive.GetOneByID(Of T)(ID)
    End Function

    Public Overloads Shared Function GetAll() As IFindFluent(Of T, T)
        Return MongoActive.GetAll(Of T)
    End Function

    Public Overloads Shared Function GetAll(sortdef As SortDefinition(Of T)) As IFindFluent(Of T, T)
        Return MongoActive.GetAll(sortdef)
    End Function

    Public Overloads Shared Function GetMany(Of TField)(FieldName As String, FieldValue As TField) As IFindFluent(Of T, T)
        Return MongoActive.GetMany(Of T, TField)(FieldName, FieldValue)
    End Function

    Public Overloads Shared Function GetMany(filterdef As FilterDefinition(Of T)) As IFindFluent(Of T, T)
        Return MongoActive.GetMany(filterdef)
    End Function

    Public Overloads Shared Function GetMany(filterdef As FilterDefinition(Of T), sortdef As SortDefinition(Of T)) As IFindFluent(Of T, T)
        Return MongoActive.GetMany(filterdef, sortdef)
    End Function

    Public Overloads Shared Function GetCount() As Integer
        Return MongoActive.GetCount(Of T)
    End Function

    Public Overloads Shared Function GetCount(filterdef As FilterDefinition(Of T)) As Integer
        Return MongoActive.GetCount(filterdef)
    End Function

    Public Overloads Shared Function GetCountLong() As Long
        Return MongoActive.GetCountLong(Filter.Empty)
    End Function

    Public Overloads Shared Function GetCountLong(filterdef As FilterDefinition(Of T)) As Long
        Return MongoActive.GetCountLong(filterdef)
    End Function

    Public Overloads Function GetThis() As T
        Return GetOneByID(GetBaseInstance(GetInstanceFromMe).ID)
    End Function
#End Region

#Region "Insert"
    Public Overloads Shared Sub InsertOne(item As T)
        MongoActive.InsertOne(item)
    End Sub

    Public Overloads Shared Sub InsertMany(items As IEnumerable(Of T))
        MongoActive.InsertMany(items)
    End Sub

    Public Sub InsertThis()
        InsertOne(GetInstanceFromMe)
    End Sub

#End Region

#Region "Replace"
    Public Overloads Shared Function ReplaceOne(item As T) As ReplaceOneResult
        Return MongoActive.ReplaceOne(item)
    End Function

    Public Function ReplaceThis() As ReplaceOneResult
        Return ReplaceOne(GetInstanceFromMe)
    End Function
#End Region

#Region "Update"
    Public Overloads Shared Function UpdateOne(item As T, updatedef As UpdateDefinition(Of T)) As UpdateResult
        Return MongoActive.UpdateOne(item, updatedef)
    End Function

    Public Overloads Shared Function UpdateMany(items As IEnumerable(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
        Return MongoActive.UpdateMany(items, updatedef)
    End Function

    Public Overloads Shared Function UpdateMany(filterdef As FilterDefinition(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
        Return MongoActive.UpdateMany(filterdef, updatedef)
    End Function

    Public Overloads Shared Function UpdateAll(updatedef As UpdateDefinition(Of T)) As UpdateResult
        Return MongoActive.UpdateMany(Filter.Empty, updatedef)
    End Function

    Public Function UpdateThis(updatedef As UpdateDefinition(Of T), Optional refreshAfterUpdate As Boolean = True) As UpdateResult
        Dim ret = UpdateOne(GetInstanceFromMe, updatedef)
        If refreshAfterUpdate Then GetThis()
        Return ret
    End Function

#End Region

#Region "Delete"
    Public Overloads Shared Function DeleteOne(item As T) As DeleteResult
        Return MongoActive.DeleteOne(item)
    End Function

    Public Overloads Shared Function DeleteMany(items As IEnumerable(Of T)) As DeleteResult
        Return MongoActive.DeleteMany(items)
    End Function

    Public Overloads Shared Function DeleteMany(filterdef As FilterDefinition(Of T)) As DeleteResult
        Return MongoActive.DeleteMany(filterdef)
    End Function

    Public Overloads Shared Sub DeleteAll()
        MongoActive.DeleteAll(Of T)()
    End Sub

    Public Function DeleteThis() As DeleteResult
        Return DeleteOne(GetInstanceFromMe)
    End Function
#End Region

End Class