using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Shared;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization;

namespace MongoDb.Test.v1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MongoServerAddress serverAddress = new MongoServerAddress("nequeompcx64", 27017);
            MongoClientSettings clientSettings = new MongoClientSettings();

            // Assign the server.
            clientSettings.Server = serverAddress;

            // Assign the client.
            MongoClient client = new MongoClient(clientSettings);
            IMongoDatabase database = client.GetDatabase("nequeo");
            GetList(database);
        }

        private async void GetList(IMongoDatabase database)
        {
            IAsyncCursor<MongoDB.Bson.BsonDocument> collections = await database.ListCollectionsAsync();
            bool moved = await collections.MoveNextAsync();
            IEnumerable<BsonDocument> tt = collections.Current;
            int count = tt.Count();

            IAsyncCursor<MongoDB.Bson.BsonDocument> collectionsDB = await database.Client.ListDatabasesAsync();
            bool movedDB = await collectionsDB.MoveNextAsync();
            IEnumerable<BsonDocument> dbs = collectionsDB.Current;
            int countDB = dbs.Count();
            
            // For each collection in the database.
            foreach (BsonDocument document in tt)
            {
                try
                {
                    // Get the collection name.
                    BsonElement nameElement = document.GetElement("name");
                    BsonValue nameValue = nameElement.Value;
                    string collectionName = nameValue.AsString;

                    // Get the document collection.
                    IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);
                    

                    BsonDocument[] bsonDocuments = (await collection.Find(new BsonDocument()).Limit(1).ToListAsync()).ToArray();
                    int yuuyuy = bsonDocuments.Length;

                    Dictionary<string, object> row = bsonDocuments[0].ToDictionary();
                    int ggffgfg = row.Count;
                }
                catch { }
                
                
                
            }
        }
    }
}
