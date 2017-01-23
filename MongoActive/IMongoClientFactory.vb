Imports MongoDB.Driver

Public Interface IMongoClientFactory
    Function GetMongoClient() As MongoClient
End Interface