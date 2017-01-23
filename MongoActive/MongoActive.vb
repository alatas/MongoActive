Imports System.Configuration
Imports MongoDB.Bson
Imports MongoDB.Driver

Public NotInheritable Class MongoActive


#Region "Initializers"
    Public Shared typeCollectionList As IDictionary = Hashtable.Synchronized(New Hashtable())

    Private Shared Function GetCollectionSettings(instance As Object) As MongoActiveAttribute
        Return GetCollectionSettings(instance.GetType)
    End Function

    Private Shared Function GetCollectionSettings(type As Type) As MongoActiveAttribute
        Return CType(type.GetCustomAttributes(GetType(MongoActiveAttribute), True).FirstOrDefault, MongoActiveAttribute)
    End Function

    Private Shared Function GetCollectionName(type As Type) As String
        Dim settings = GetCollectionSettings(type)

        If settings Is Nothing Then Return type.Name

        If settings.UseBaseForCollectionName AndAlso Not (type.BaseType = GetType(MongoActive) Or type.BaseType = GetType(MongoActiveBaseGeneric(Of ))) Then
            Return type.BaseType.Name
        End If

        If settings.CollectionName <> "" Then
            Return settings.CollectionName
        End If

        Return type.Name
    End Function

    Public Shared Function GetCollection(Of TOut)() As IMongoCollection(Of TOut)
        Dim type = GetType(TOut)
        Dim ret As IMongoCollection(Of TOut) = typeCollectionList(type)

        If ret Is Nothing Then
            ret = GetDB(type).GetCollection(Of TOut)(GetCollectionName(type))
            typeCollectionList(type) = ret
        End If

        Return ret
    End Function

    Public Shared Function GetDB(type As Type) As IMongoDatabase
        Dim settings = GetCollectionSettings(type)
        Dim databaseName As String = If(settings?.DatabaseName Is Nothing, "Default", settings?.DatabaseName)

        Dim client As MongoClient = Nothing

        If settings?.ClientFactoryType <> "" Then
            Dim factory = CType(Activator.CreateInstance(Type.GetType(settings.ClientFactoryType)), IMongoClientFactory)
            client = factory.GetMongoClient
        Else
            Dim connectionString As String = GetMongoConnectionString()
            If connectionString IsNot Nothing Then
                Dim url As New MongoUrl(connectionString)
                If url.DatabaseName IsNot Nothing Then databaseName = url.DatabaseName
                client = New MongoClient(New MongoUrl(connectionString))
            Else
                client = New MongoClient("mongodb://localhost")
            End If
        End If

        Return client.GetDatabase(databaseName)
    End Function

    Private Shared Function GetMongoConnectionString() As String
        Dim connstr = (From cs As ConnectionStringSettings In ConfigurationManager.ConnectionStrings
                       Where cs.ProviderName = "alatas.MongoActive"
                       Select cs).FirstOrDefault
        If connstr Is Nothing Then Return Nothing
        Return connstr.ConnectionString
    End Function

    Private Shared Function GetBaseInstance(item As Object) As MongoActiveBase
        Return DirectCast(item, MongoActiveBase)
    End Function
#End Region

#Region "Param Factories"
    Public Shared Function Filter(Of TOut)() As FilterDefinitionBuilder(Of TOut)
        Return Builders(Of TOut).Filter
    End Function

    Public Shared Function Update(Of TOut)() As UpdateDefinitionBuilder(Of TOut)
        Return Builders(Of TOut).Update
    End Function

    Public Shared Function Sort(Of TOut)() As SortDefinitionBuilder(Of TOut)
        Return Builders(Of TOut).Sort
    End Function
#End Region

#Region "Get"
    Public Shared Function GetOne(Of TOut, TField)(IDFieldName As String, IDFieldValue As TField) As TOut
        Return GetCollection(Of TOut).Find(Filter(Of TOut).Eq(Of TField)(IDFieldName, IDFieldValue)).FirstOrDefault
    End Function

    Public Shared Function GetOne(Of TOut)(filterdef As FilterDefinition(Of TOut)) As TOut
        Return GetCollection(Of TOut).Find(filterdef).FirstOrDefault
    End Function

    Public Shared Function GetOneByID(Of TOut)(ID As ObjectId) As TOut
        Return GetCollection(Of TOut).Find(Filter(Of TOut).Eq(Of ObjectId)("_id", ID)).FirstOrDefault
    End Function

    Public Shared Function GetAll(Of TOut)() As IFindFluent(Of TOut, TOut)
        Return GetCollection(Of TOut).Find(Filter(Of TOut).Empty)
    End Function

    Public Shared Function GetAll(Of TOut)(sortdef As SortDefinition(Of TOut)) As IFindFluent(Of TOut, TOut)
        Return GetCollection(Of TOut).Find(Filter(Of TOut).Empty).Sort(sortdef)
    End Function

    Public Shared Function GetMany(Of TOut, TField)(FieldName As String, FieldValue As TField) As IFindFluent(Of TOut, TOut)
        Return GetCollection(Of TOut).Find(Filter(Of TOut).Eq(Of TField)(FieldName, FieldValue))
    End Function

    Public Shared Function GetMany(Of TOut)(filterdef As FilterDefinition(Of TOut)) As IFindFluent(Of TOut, TOut)
        Return GetCollection(Of TOut).Find(filterdef)
    End Function

    Public Shared Function GetMany(Of TOut)(filterdef As FilterDefinition(Of TOut), sortdef As SortDefinition(Of TOut)) As IFindFluent(Of TOut, TOut)
        Return GetCollection(Of TOut).Find(filterdef).Sort(sortdef)
    End Function

    Public Shared Function GetCount(Of TOut)() As Integer
        Return CInt(GetCollection(Of TOut).Count(Filter(Of TOut).Empty))
    End Function

    Public Shared Function GetCount(Of TOut)(filterdef As FilterDefinition(Of TOut)) As Integer
        Return CInt(GetCollection(Of TOut).Count(filterdef))
    End Function

    Public Shared Function GetCountLong(Of TOut)() As Long
        Return GetCollection(Of TOut).Count(Filter(Of TOut).Empty)
    End Function

    Public Shared Function GetCountLong(Of TOut)(filterdef As FilterDefinition(Of TOut)) As Long
        Return GetCollection(Of TOut).Count(filterdef)
    End Function
#End Region

#Region "Insert"

    Public Shared Sub InsertOne(Of T)(item As T)
        RaiseEvent BeforeInsertOne(GetType(T), {item})
        GetCollection(Of T).InsertOne(item)
        RaiseEvent AfterInsertOne()
    End Sub

    Public Shared Sub InsertMany(Of T)(items As IEnumerable(Of T))
        RaiseEvent BeforeInsertMany(GetType(T), {items})
        GetCollection(Of T).InsertMany(items)
        RaiseEvent AfterInsertMany()
    End Sub

#End Region

#Region "Replace"
    Public Shared Function ReplaceOne(Of T)(item As T) As ReplaceOneResult
        RaiseEvent BeforeReplaceOne(GetType(T), {item})
        Dim ret = GetCollection(Of T).ReplaceOne(Filter(Of T).Eq(Of ObjectId)("_id", GetBaseInstance(item).ID), item)
        RaiseEvent AfterReplaceOne(ret)
        Return ret
    End Function

    'ReplaceMany is not implemented by the offical driver, so are we.
    'Public Shared Function ReplaceMany(Of T)(item As T)

#End Region

#Region "Update"
    Public Shared Function UpdateOne(Of T)(item As T, updatedef As UpdateDefinition(Of T)) As UpdateResult
        RaiseEvent BeforeUpdateOne(GetType(T), {item, updatedef})
        Dim ret = GetCollection(Of T).UpdateOne(Filter(Of T).Eq(Of ObjectId)("_id", GetBaseInstance(item).ID), updatedef)
        RaiseEvent AfterUpdateOne(ret)
        Return ret
    End Function

    Public Shared Function UpdateMany(Of T)(items As IEnumerable(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
        RaiseEvent BeforeUpdateMany(GetType(T), {items, updatedef})
        Dim ret = UpdateMany(Of T)(Filter(Of T).In(Of ObjectId)("_id", (From i In items Let o = GetBaseInstance(i) Select o.ID).ToArray), updatedef)
        RaiseEvent AfterUpdateMany(ret)
        Return ret
    End Function

    Public Shared Function UpdateMany(Of T)(filterdef As FilterDefinition(Of T), updatedef As UpdateDefinition(Of T)) As UpdateResult
        RaiseEvent BeforeUpdateMany(GetType(T), {filterdef, updatedef})
        Dim ret = GetCollection(Of T).UpdateMany(filterdef, updatedef)
        RaiseEvent AfterUpdateMany(ret)
        Return ret
    End Function

    Public Shared Function UpdateAll(Of T)(updatedef As UpdateDefinition(Of T)) As UpdateResult
        RaiseEvent BeforeUpdateAll(GetType(T), {updatedef})
        Dim ret = GetCollection(Of T).UpdateMany(Filter(Of T).Empty, updatedef)
        RaiseEvent AfterUpdateAll(ret)
        Return ret
    End Function
#End Region

#Region "Delete"
    Public Shared Function DeleteOne(Of T)(item As T) As DeleteResult
        RaiseEvent BeforeDeleteOne(GetType(T), {item})
        Dim ret = GetCollection(Of T).DeleteOne(Filter(Of T).Eq(Of ObjectId)("_id", GetBaseInstance(item).ID))
        RaiseEvent AfterDeleteOne(ret)
        Return ret
    End Function

    Public Shared Function DeleteMany(Of T)(items As IEnumerable(Of T)) As DeleteResult
        RaiseEvent BeforeDeleteMany(GetType(T), {items})
        Dim ret = DeleteMany(Filter(Of T).In(Of ObjectId)("_id", (From i In items Let o = GetBaseInstance(i) Select o.ID).ToArray))
        RaiseEvent AfterDeleteMany(ret)
        Return ret
    End Function

    Public Shared Function DeleteMany(Of T)(filterdef As FilterDefinition(Of T)) As DeleteResult
        RaiseEvent BeforeDeleteMany(GetType(T), {filterdef})
        Dim ret = GetCollection(Of T).DeleteMany(filterdef)
        RaiseEvent AfterDeleteMany(ret)
        Return ret
    End Function

    Public Shared Sub DeleteAll(Of T)()
        RaiseEvent BeforeDeleteAll(GetType(T), {})
        GetDB(GetType(T)).DropCollection(GetCollectionName(GetType(T)))
        RaiseEvent AfterDeleteAll(Nothing)
    End Sub

#End Region

#Region "Events"
    Shared Event BeforeDeleteOne(t As Type, args() As Object)
    Shared Event BeforeDeleteAll(t As Type, args() As Object)
    Shared Event BeforeDeleteMany(t As Type, args() As Object)

    Shared Event AfterDeleteOne(ret As DeleteResult)
    Shared Event AfterDeleteAll(ret As DeleteResult)
    Shared Event AfterDeleteMany(ret As DeleteResult)


    Shared Event BeforeUpdateOne(t As Type, args() As Object)
    Shared Event BeforeUpdateAll(t As Type, args() As Object)
    Shared Event BeforeUpdateMany(t As Type, args() As Object)

    Shared Event AfterUpdateOne(ret As UpdateResult)
    Shared Event AfterUpdateAll(ret As UpdateResult)
    Shared Event AfterUpdateMany(ret As UpdateResult)


    Shared Event BeforeReplaceOne(t As Type, args() As Object)
    Shared Event AfterReplaceOne(ret As ReplaceOneResult)


    Shared Event BeforeInsertOne(t As Type, args() As Object)
    Shared Event BeforeInsertMany(t As Type, args() As Object)

    Shared Event AfterInsertOne()
    Shared Event AfterInsertMany()
#End Region

End Class