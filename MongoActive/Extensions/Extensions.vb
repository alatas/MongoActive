Imports System.Runtime.CompilerServices
Imports MongoDB.Bson
Imports MongoDB.Driver

Namespace Extensions

    Public Module Extensions

#Region "Param Factories"
        <Extension> Public Function FilterDef(Of T)(item As T) As FilterDefinitionBuilder(Of T)
            Return Builders(Of T).Filter
        End Function

        <Extension> Public Function UpdateDef(Of T)(item As T) As UpdateDefinitionBuilder(Of T)
            Return Builders(Of T).Update
        End Function

        <Extension> Public Function SortDef(Of T)(item As T) As SortDefinitionBuilder(Of T)
            Return Builders(Of T).Sort
        End Function
#End Region

#Region "Get"
        <Extension> Public Function GetOne(Of TOut, TField)(IDFieldName As String, IDFieldValue As TField) As TOut
            Return MongoActive.GetOne(Of TOut, TField)(IDFieldName, IDFieldValue)
        End Function

        <Extension> Public Function GetOne(Of TOut)(filterdef As FilterDefinition(Of TOut)) As TOut
            Return MongoActive.GetOne(Of TOut)(filterdef)
        End Function

        <Extension> Public Function GetOneByID(Of TOut)(ID As ObjectId) As TOut
            Return MongoActive.GetOneByID(Of TOut)(ID)
        End Function

        <Extension> Public Function GetAll(Of TOut)(item As TOut) As IFindFluent(Of TOut, TOut)
            Return MongoActive.GetAll(Of TOut)
        End Function

        <Extension> Public Function GetAll(Of TOut)(sortdef As SortDefinition(Of TOut)) As IFindFluent(Of TOut, TOut)
            Return MongoActive.GetAll(Of TOut)(sortdef)
        End Function

        <Extension> Public Function GetMany(Of TOut, TField)(FieldName As String, FieldValue As TField) As IFindFluent(Of TOut, TOut)
            Return MongoActive.GetMany(Of TOut, TField)(FieldName, FieldValue)
        End Function

        <Extension> Public Function GetMany(Of TOut)(filterdef As FilterDefinition(Of TOut)) As IFindFluent(Of TOut, TOut)
            Return MongoActive.GetMany(Of TOut)(filterdef)
        End Function

        <Extension> Public Function GetMany(Of TOut)(filterdef As FilterDefinition(Of TOut), sortdef As SortDefinition(Of TOut)) As IFindFluent(Of TOut, TOut)
            Return MongoActive.GetMany(Of TOut)(filterdef, sortdef)
        End Function

        <Extension> Public Function GetCount(Of TOut)(item As TOut) As Integer
            Return MongoActive.GetCount(Of TOut)()
        End Function

        <Extension> Public Function GetCount(Of TOut)(filterdef As FilterDefinition(Of TOut)) As Integer
            Return MongoActive.GetCount(Of TOut)(filterdef)
        End Function

        <Extension> Public Function GetCountLong(Of TOut)(item As TOut) As Long
            Return MongoActive.GetCountLong(Of TOut)()
        End Function

        <Extension> Public Function GetCountLong(Of TOut)(filterdef As FilterDefinition(Of TOut)) As Long
            Return MongoActive.GetCountLong(Of TOut)(filterdef)
        End Function
#End Region

#Region "Insert"

        <Extension> Public Sub InsertThis(Of T)(item As T)
            MongoActive.InsertOne(Of T)(item)
        End Sub

        <Extension> Public Sub InsertThese(Of T)(items As IEnumerable(Of T))
            MongoActive.InsertMany(Of T)(items)
        End Sub

#End Region

#Region "Replace"
        <Extension> Public Function ReplaceThis(Of T)(item As T) As ReplaceOneResult
            Return MongoActive.ReplaceOne(Of T)(item)
        End Function

#End Region

#Region "Update"
        <Extension> Public Function UpdateThis(Of T)(item As T, updatedef As UpdateDefinition(Of T)) As UpdateResult
            Return MongoActive.UpdateOne(Of T)(item, updatedef)
        End Function

        <Extension> Public Function UpdateMany(Of T)(items As IEnumerable(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
            Return MongoActive.UpdateMany(Of T)(items, updatedef)
        End Function

        <Extension> Public Function UpdateMany(Of T)(item As T, filterdef As FilterDefinition(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
            Return MongoActive.UpdateMany(Of T)(filterdef, updatedef)
        End Function

        <Extension>
        Public Function UpdateMany(Of T)(f As IFindFluent(Of T, T), updatedef As UpdateDefinition(Of T)) As UpdateResult
            Return MongoActive.UpdateMany(Of T)(f.ToEnumerable, updatedef)
        End Function

        <Extension> Public Function UpdateAll(Of T)(item As T, updatedef As UpdateDefinition(Of T)) As UpdateResult
            Return MongoActive.UpdateAll(Of T)(updatedef)
        End Function
#End Region

#Region "Delete"
        <Extension> Public Function DeleteThis(Of T)(item As T) As DeleteResult
            Return MongoActive.DeleteOne(Of T)(item)
        End Function

        <Extension> Public Function DeleteMany(Of T)(items As IEnumerable(Of T)) As DeleteResult
            Return MongoActive.DeleteMany(Of T)(items)
        End Function

        <Extension> Public Function DeleteMany(Of T)(item As T, filterdef As FilterDefinition(Of T)) As DeleteResult
            Return MongoActive.DeleteMany(Of T)(filterdef)
        End Function

        <Extension>
        Public Function DeleteMany(Of T)(f As IFindFluent(Of T, T)) As DeleteResult
            Return MongoActive.DeleteMany(Of T)(f.ToEnumerable)
        End Function

        <Extension> Public Sub DeleteAll(Of T)(item As T)
            MongoActive.DeleteAll(Of T)()
        End Sub

#End Region

    End Module

End Namespace





