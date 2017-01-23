Imports alatas.MongoActive
Imports MongoDB.Bson
Imports MongoDB.Driver
Imports ObjectPrinter.ObjectExtensions

<TestClass()> Public Class CRUDTests

#Region "Event Handlers"

    Sub BeforeDeletes(t As Type, args() As Object)
        Console.Out.WriteLine($"--- Before Delete {t.Name} - {args.DumpToString}")
    End Sub

    Sub AfterDeletes(ret As DeleteResult)
        Console.Out.WriteLine($"+++ After Delete {ret.DumpToString }")
    End Sub

    Sub BeforeUpdates(t As Type, args() As Object)
        Console.Out.WriteLine($"--- Before Update {t.Name} - {args.DumpToString }")
    End Sub

    Sub AfterUpdates(ret As UpdateResult)
        Console.Out.WriteLine($"+++ After Update {ret.DumpToString }")
    End Sub

    Sub BeforeReplaces(t As Type, args() As Object)
        Console.Out.WriteLine($"--- Before Replace {t.Name} - {args.DumpToString }")
    End Sub

    Sub AfterReplaces(ret As ReplaceOneResult)
        Console.Out.WriteLine($"+++ After Replace {ret.DumpToString }")
    End Sub

    Sub BeforeInserts(t As Type, args() As Object)
        Console.Out.WriteLine($"--- Before Insert {t.Name} - {args.DumpToString }")
    End Sub

    Sub AfterInserts()
        Console.Out.WriteLine($"+++ After Insert")
    End Sub

    Sub InitHandlers()
        AddHandler MongoActive.BeforeDeleteOne, AddressOf BeforeDeletes
        AddHandler MongoActive.BeforeDeleteMany, AddressOf BeforeDeletes
        AddHandler MongoActive.BeforeDeleteAll, AddressOf BeforeDeletes
        AddHandler MongoActive.AfterDeleteOne, AddressOf AfterDeletes
        AddHandler MongoActive.AfterDeleteMany, AddressOf AfterDeletes
        AddHandler MongoActive.AfterDeleteAll, AddressOf AfterDeletes

        AddHandler MongoActive.BeforeUpdateOne, AddressOf BeforeUpdates
        AddHandler MongoActive.BeforeUpdateMany, AddressOf BeforeUpdates
        AddHandler MongoActive.BeforeUpdateAll, AddressOf BeforeUpdates
        AddHandler MongoActive.AfterUpdateOne, AddressOf AfterUpdates
        AddHandler MongoActive.AfterUpdateMany, AddressOf AfterUpdates
        AddHandler MongoActive.AfterUpdateAll, AddressOf AfterUpdates

        AddHandler MongoActive.BeforeInsertOne, AddressOf BeforeInserts
        AddHandler MongoActive.BeforeInsertMany, AddressOf BeforeInserts
        AddHandler MongoActive.BeforeReplaceOne, AddressOf BeforeReplaces
        AddHandler MongoActive.AfterInsertOne, AddressOf AfterInserts
        AddHandler MongoActive.AfterInsertMany, AddressOf AfterInserts
        AddHandler MongoActive.AfterReplaceOne, AddressOf AfterReplaces
    End Sub
#End Region

    <TestInitialize> Public Sub Init()
        InitHandlers()
    End Sub

    <TestMethod()> Public Sub SimpleEntityCRUD()

        'DeleteAll
        SimpleEntity.DeleteAll()

        'GetCountLong
        Assert.AreEqual(0L, SimpleEntity.GetCountLong)

        Dim testKey As String = Guid.NewGuid.ToString

        'InsertOne
        SimpleEntity.InsertOne(New SimpleEntity With {.TestKey = testKey})

        'GetAll
        Assert.AreEqual(1L, SimpleEntity.GetAll.Count)
        Assert.AreEqual(testKey, SimpleEntity.GetAll.FirstOrDefault.TestKey)

        'GetOne
        Dim testobj = SimpleEntity.GetOne(Of String)("TestKey", testKey)
        Assert.AreEqual(testKey, testobj.TestKey)

        'GetOneWithID
        Assert.AreEqual(testKey, SimpleEntity.GetOneByID(testobj.ID).TestKey)

        'InsertMany
        SimpleEntity.InsertMany({
                               New SimpleEntity With {.TestKey = Guid.NewGuid.ToString},
                               New SimpleEntity With {.TestKey = Guid.NewGuid.ToString}
                               })

        'GetCount(Filter)
        Assert.AreEqual(3, SimpleEntity.GetCount(SimpleEntity.Filter.Gt(Of Long)("SortKey", 0)))

        'GetAll(Sort)
        Assert.AreEqual(testKey, SimpleEntity.GetAll(SimpleEntity.Sort.Ascending("SortKey")).FirstOrDefault.TestKey)
        Assert.AreEqual(testKey, SimpleEntity.GetAll(SimpleEntity.Sort.Descending("SortKey")).ToList.LastOrDefault.TestKey)


        'InsertThis
        Dim testobj2 As New SimpleEntity With {.TestKey = Guid.NewGuid.ToString}
        testobj2.InsertThis()

        'GetCount(Filter)
        Assert.AreEqual(1, SimpleEntity.GetCount(SimpleEntity.Filter.Eq(Of String)("TestKey", testobj2.TestKey)))

        'GetCountLong(Filter)
        Assert.AreEqual(4L, SimpleEntity.GetCountLong())

        'GetMany(Filter)
        Assert.AreEqual(3L, SimpleEntity.GetMany(SimpleEntity.Filter.Ne(Of Long)("SortKey", testobj2.SortKey)).Count)

        'GetMany(Filter,Sort)
        Assert.AreEqual(testKey, SimpleEntity.GetMany(SimpleEntity.Filter.Ne(Of Long)("SortKey", testobj2.SortKey), SimpleEntity.Sort.Ascending("SortKey")).FirstOrDefault.TestKey)
        Assert.AreEqual(testKey, SimpleEntity.GetMany(SimpleEntity.Filter.Ne(Of ObjectId)("_id", testobj2.ID), SimpleEntity.Sort.Descending("SortKey")).ToList.LastOrDefault.TestKey)

        testobj2.TestKey = Guid.NewGuid.ToString
        Assert.AreEqual(1L, testobj2.ReplaceThis.ModifiedCount)

        'GetCount(Filter)
        Assert.AreEqual(1, SimpleEntity.GetCount(SimpleEntity.Filter.Eq(Of String)("TestKey", testobj2.TestKey)))

        'UpdateThis
        Assert.AreEqual(1L, testobj2.UpdateThis(SimpleEntity.Update.Set(Of Long)("SortKey", Now.Ticks)).ModifiedCount)

        'GetThis
        Assert.AreNotEqual(testobj2.SortKey, testobj2.GetThis.SortKey)
        testobj2 = testobj2.GetThis

        'UpdateAll
        Assert.AreEqual(4L, SimpleEntity.UpdateAll(SimpleEntity.Update.Inc(Of Long)("SortKey", testobj.SortKey * -1)).ModifiedCount)

        'GetMany(Filter)
        Assert.AreEqual(4L, SimpleEntity.GetMany(SimpleEntity.Filter.Ne(Of Long)("SortKey", testobj.SortKey)).Count)

        'UpdateMany
        Assert.AreEqual(3L, SimpleEntity.UpdateMany(SimpleEntity.Filter.Ne(Of Long)("SortKey", 0), SimpleEntity.Update.Mul(Of Integer)("SortKey", -1)).ModifiedCount)

        'GetMany(Filter)
        Dim testobjs = SimpleEntity.GetMany(SimpleEntity.Filter.Lt(Of Long)("SortKey", 0))
        Assert.AreEqual(3L, testobjs.Count)

        'UpdateThese
        Assert.AreEqual(3L, testobjs.UpdateThese(SimpleEntity.Update.Mul(Of Integer)("SortKey", -1)).ModifiedCount)

        'GetMany(Filter)
        testobjs = SimpleEntity.GetMany(SimpleEntity.Filter.Gt(Of Long)("SortKey", 0))
        Assert.AreEqual(3L, testobjs.Count)

        'DeleteThese
        Assert.AreEqual(3L, testobjs.DeleteThese().DeletedCount)

        'GetCount
        Assert.AreEqual(1, SimpleEntity.GetCount)

        'DeleteThis
        testobj.DeleteThis()

        'GetCountLong
        Assert.AreEqual(0L, SimpleEntity.GetCountLong)
    End Sub

    <TestMethod()> Public Sub InheritedEntityCRUD()
        'DeleteAll
        InheritedEntity.DeleteAll()

        'GetCountLong
        Assert.AreEqual(0L, InheritedEntity.GetCountLong)

        Dim testKey As String = Guid.NewGuid.ToString

        'InsertOne
        InheritedEntity.InsertOne(New InheritedEntity With {.TestKey = testKey})

        'GetAll
        Assert.AreEqual(1L, InheritedEntity.GetAll.Count)
        Assert.AreEqual(testKey, InheritedEntity.GetAll.FirstOrDefault.TestKey)

        'GetOne
        Dim testobj = InheritedEntity.GetOne(Of String)("TestKey", testKey)
        Assert.AreEqual(testKey, testobj.TestKey)

        'GetOneWithID
        Assert.AreEqual(testKey, InheritedEntity.GetOneByID(testobj.ID).TestKey)

        'InsertMany
        InheritedEntity.InsertMany({
                               New InheritedEntity With {.TestKey = Guid.NewGuid.ToString},
                               New InheritedEntity With {.TestKey = Guid.NewGuid.ToString}
                               })

        'GetCount(Filter)
        Assert.AreEqual(3, InheritedEntity.GetCount(InheritedEntity.Filter.Gt(Of Long)("SortKey", 0)))

        'GetAll(Sort)
        Assert.AreEqual(testKey, InheritedEntity.GetAll(InheritedEntity.Sort.Ascending("SortKey")).FirstOrDefault.TestKey)
        Assert.AreEqual(testKey, InheritedEntity.GetAll(InheritedEntity.Sort.Descending("SortKey")).ToList.LastOrDefault.TestKey)


        'InsertThis
        Dim testobj2 As New InheritedEntity With {.TestKey = Guid.NewGuid.ToString}
        testobj2.InsertThis()

        'GetCount(Filter)
        Assert.AreEqual(1, InheritedEntity.GetCount(InheritedEntity.Filter.Eq(Of String)("TestKey", testobj2.TestKey)))

        'GetCountLong(Filter)
        Assert.AreEqual(4L, InheritedEntity.GetCountLong())

        'GetMany(Filter)
        Assert.AreEqual(3L, InheritedEntity.GetMany(InheritedEntity.Filter.Ne(Of Long)("SortKey", testobj2.SortKey)).Count)

        'GetMany(Filter,Sort)
        Assert.AreEqual(testKey, InheritedEntity.GetMany(InheritedEntity.Filter.Ne(Of Long)("SortKey", testobj2.SortKey), InheritedEntity.Sort.Ascending("SortKey")).FirstOrDefault.TestKey)
        Assert.AreEqual(testKey, InheritedEntity.GetMany(InheritedEntity.Filter.Ne(Of ObjectId)("_id", testobj2.ID), InheritedEntity.Sort.Descending("SortKey")).ToList.LastOrDefault.TestKey)

        testobj2.TestKey = Guid.NewGuid.ToString
        Assert.AreEqual(1L, testobj2.ReplaceThis.ModifiedCount)

        'GetCount(Filter)
        Assert.AreEqual(1, InheritedEntity.GetCount(InheritedEntity.Filter.Eq(Of String)("TestKey", testobj2.TestKey)))

        'UpdateThis
        Assert.AreEqual(1L, testobj2.UpdateThis(InheritedEntity.Update.Set(Of Long)("SortKey", Now.Ticks)).ModifiedCount)

        'GetThis
        Assert.AreNotEqual(testobj2.SortKey, testobj2.GetThis.SortKey)
        testobj2 = testobj2.GetThis

        'UpdateAll
        Assert.AreEqual(4L, InheritedEntity.UpdateAll(InheritedEntity.Update.Inc(Of Long)("SortKey", testobj.SortKey * -1)).ModifiedCount)

        'GetMany(Filter)
        Assert.AreEqual(4L, InheritedEntity.GetMany(InheritedEntity.Filter.Ne(Of Long)("SortKey", testobj.SortKey)).Count)

        'UpdateMany
        Assert.AreEqual(3L, InheritedEntity.UpdateMany(InheritedEntity.Filter.Ne(Of Long)("SortKey", 0), InheritedEntity.Update.Mul(Of Integer)("SortKey", -1)).ModifiedCount)

        'GetMany(Filter)
        Dim testobjs = InheritedEntity.GetMany(InheritedEntity.Filter.Lt(Of Long)("SortKey", 0))
        Assert.AreEqual(3L, testobjs.Count)

        'UpdateThese
        Assert.AreEqual(3L, testobjs.UpdateThese(InheritedEntity.Update.Mul(Of Integer)("SortKey", -1)).ModifiedCount)

        'GetMany(Filter)
        testobjs = InheritedEntity.GetMany(InheritedEntity.Filter.Gt(Of Long)("SortKey", 0))
        Assert.AreEqual(3L, testobjs.Count)

        'DeleteThese
        Assert.AreEqual(3L, testobjs.DeleteThese().DeletedCount)

        'GetCount
        Assert.AreEqual(1, InheritedEntity.GetCount)

        'DeleteThis
        testobj.DeleteThis()

        'GetCountLong
        Assert.AreEqual(0L, InheritedEntity.GetCountLong)
    End Sub

    <TestMethod()> Public Sub ReInheritedEntityCRUD()
        Dim Filter = ReInheritedEntity.Filter
        Dim Sort = ReInheritedEntity.Sort
        Dim Update = ReInheritedEntity.Update

        'DeleteAll
        ReInheritedEntity.DeleteAll()

        'GetCountLong
        Assert.AreEqual(0L, ReInheritedEntity.GetCountLong)

        Dim testKey As String = Guid.NewGuid.ToString

        'InsertOne
        ReInheritedEntity.InsertOne(New ReInheritedEntity With {.TestKey = testKey})

        'GetAll
        Assert.AreEqual(1L, ReInheritedEntity.GetAll.Count)
        Assert.AreEqual(testKey, ReInheritedEntity.GetAll.FirstOrDefault.TestKey)

        'GetOne
        Dim testobj = ReInheritedEntity.GetOne(Of String)("TestKey", testKey)
        Assert.AreEqual(testKey, testobj.TestKey)

        'GetOneWithID
        Assert.AreEqual(testKey, ReInheritedEntity.GetOneByID(testobj.ID).TestKey)

        'InsertMany
        ReInheritedEntity.InsertMany({
                               New ReInheritedEntity With {.TestKey = Guid.NewGuid.ToString},
                               New ReInheritedEntity With {.TestKey = Guid.NewGuid.ToString}
                               })

        'GetCount(Filter)
        Assert.AreEqual(3, ReInheritedEntity.GetCount(Filter.Gt(Of Long)("SortKey", 0)))

        'GetAll(Sort)
        Assert.AreEqual(testKey, ReInheritedEntity.GetAll(Sort.Ascending("SortKey")).FirstOrDefault.TestKey)
        Assert.AreEqual(testKey, ReInheritedEntity.GetAll(Sort.Descending("SortKey")).ToList.LastOrDefault.TestKey)


        'InsertThis
        Dim testobj2 As New ReInheritedEntity With {.TestKey = Guid.NewGuid.ToString}
        testobj2.InsertThis()

        'GetCount(Filter)
        Assert.AreEqual(1, ReInheritedEntity.GetCount(Filter.Eq(Of String)("TestKey", testobj2.TestKey)))

        'GetCountLong(Filter)
        Assert.AreEqual(4L, ReInheritedEntity.GetCountLong())

        'GetMany(Filter)
        Assert.AreEqual(3L, ReInheritedEntity.GetMany(Filter.Ne(Of Long)("SortKey", testobj2.SortKey)).Count)

        'GetMany(Filter,Sort)
        Assert.AreEqual(testKey, ReInheritedEntity.GetMany(Filter.Ne(Of Long)("SortKey", testobj2.SortKey), Sort.Ascending("SortKey")).FirstOrDefault.TestKey)
        Assert.AreEqual(testKey, ReInheritedEntity.GetMany(Filter.Ne(Of ObjectId)("_id", testobj2.ID), Sort.Descending("SortKey")).ToList.LastOrDefault.TestKey)

        testobj2.TestKey = Guid.NewGuid.ToString
        Assert.AreEqual(1L, testobj2.ReplaceThis.ModifiedCount)

        'GetCount(Filter)
        Assert.AreEqual(1, ReInheritedEntity.GetCount(Filter.Eq(Of String)("TestKey", testobj2.TestKey)))

        'UpdateThis
        Assert.AreEqual(1L, testobj2.UpdateThis(ReInheritedEntity.Update.Set(Of Long)("SortKey", Now.Ticks)).ModifiedCount)

        'GetThis
        Assert.AreNotEqual(testobj2.SortKey, testobj2.GetThis.SortKey)
        testobj2 = testobj2.GetThis

        'UpdateAll
        Assert.AreEqual(4L, ReInheritedEntity.UpdateAll(Update.Inc(Of Long)("SortKey", testobj.SortKey * -1)).ModifiedCount)

        'GetMany(Filter)
        Assert.AreEqual(4L, ReInheritedEntity.GetMany(Filter.Ne(Of Long)("SortKey", testobj.SortKey)).Count)

        'UpdateMany
        Assert.AreEqual(3L, ReInheritedEntity.UpdateMany(Filter.Ne(Of Long)("SortKey", 0), Update.Mul(Of Integer)("SortKey", -1)).ModifiedCount)

        'GetMany(Filter)
        Dim testobjs = ReInheritedEntity.GetMany(Filter.Lt(Of Long)("SortKey", 0))
        Assert.AreEqual(3L, testobjs.Count)

        'UpdateThese
        Assert.AreEqual(3L, testobjs.UpdateThese(Update.Mul(Of Integer)("SortKey", -1)).ModifiedCount)

        'GetMany(Filter)
        testobjs = ReInheritedEntity.GetMany(Filter.Gt(Of Long)("SortKey", 0))
        Assert.AreEqual(3L, testobjs.Count)

        'DeleteThese
        Assert.AreEqual(3L, testobjs.DeleteThese().DeletedCount)

        'GetCount
        Assert.AreEqual(1, ReInheritedEntity.GetCount)

        'DeleteThis
        testobj.DeleteThis()

        'GetCountLong
        Assert.AreEqual(0L, ReInheritedEntity.GetCountLong)
    End Sub

End Class

