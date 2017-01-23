Imports MongoDB.Bson
Imports MongoDB.Bson.Serialization.Attributes

<Serializable>
Public MustInherit Class MongoActiveBase
    <BsonElement("_id")>
    Public Property ID As ObjectId = ObjectId.GenerateNewId

End Class
