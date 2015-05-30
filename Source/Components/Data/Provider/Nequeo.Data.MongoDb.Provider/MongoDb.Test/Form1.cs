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
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace MongoDb.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Nequeo.Data.MongoDb.Connection conn = new Nequeo.Data.MongoDb.Connection("nequeompcx64");
            Nequeo.Data.MongoDb.DataAccess access = new Nequeo.Data.MongoDb.DataAccess(conn);
            MongoDatabase database = access.GetDatabase("nequeo");
            MongoCollection<UserModel> coll = access.GetCollection<UserModel>(database, "User");
            MongoCollection<User> collUser = access.GetCollection<User>(database, "User");
            MongoCollection<BsonDocument> collb = access.GetCollection(database, "User");

            BsonDocument[] bsonData = access.FindAll(collb).ToArray();
            BsonDocument bsonData_1 = bsonData[0];
            IEnumerable<BsonValue> values = bsonData_1.Values;

            object[] users = access.CreateDynamicModel(bsonData, "User");
            System.Data.DataTable table = access.CreateDataTable(users, "User");

            UserModel[] data = access.FindAll(coll, limit: 1, skip: 1).ToArray();
            System.Data.DataTable tableUser = access.CreateDataTable(data, "User");

            User[] User = access.FindAll(collUser).ToArray();
            long number = access.Count(coll);

            dataGridView1.DataSource = table;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Nequeo.Data.MongoDb.Connection conn = new Nequeo.Data.MongoDb.Connection("nequeompc");
            Nequeo.Data.MongoDb.DataAccess access = new Nequeo.Data.MongoDb.DataAccess(conn);
            MongoDatabase database = access.GetDatabase("nequeo");
            MongoCollection<BsonDocument> collb = access.GetCollection(database, "User");
            BsonDocument document = access.FindAll(collb, limit: 1).ToArray().First();

            Nequeo.Data.MongoDb.CodeDom.BsonDocumentModel bson = new Nequeo.Data.MongoDb.CodeDom.BsonDocumentModel();
            Nequeo.Data.MongoDb.CodeDom.BsonModelContainer model = new Nequeo.Data.MongoDb.CodeDom.BsonModelContainer();
            model.ClassName = "User";
            model.Namespace = "Nequeo.MongoDb";
            model.BsonDocument = document;
            model.AssignProperties();

            System.CodeDom.CodeCompileUnit unit = bson.Generate(model);
            bson.CreateCodeFile(@"C:\Temp\BsonDocModel.cs", unit);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Nequeo.Data.MongoDb.Connection conn = new Nequeo.Data.MongoDb.Connection("nequeompc");
            Nequeo.Data.MongoDb.DataAccess access = new Nequeo.Data.MongoDb.DataAccess(conn);
            MongoDatabase database = access.GetDatabase("nequeo");

            Nequeo.Data.MongoDb.CodeDom.BsonDocumentModel bson = new Nequeo.Data.MongoDb.CodeDom.BsonDocumentModel();
            int ret = bson.Generate(database, @"c:\temp\mongodb\", "Nequeo.MongoDb.Database");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Nequeo.Data.MongoDb.Connection conn = new Nequeo.Data.MongoDb.Connection("nequeompc");
            Nequeo.Data.MongoDb.DataAccess access = new Nequeo.Data.MongoDb.DataAccess(conn);
            MongoDatabase database = access.GetDatabase("nequeo");
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TestEnum mm = new TestEnum(56776.344m);
            TestEnum nn = new TestEnum() { { "gfgf", "sd", "dsdsds" }, { "gfgf", "sd", "dsdsds" } };
            Nequeo.Convertible.Number num = new Nequeo.Convertible.Number(6);
          

            Nequeo.Collections.NumberCollection numb = new Nequeo.Collections.NumberCollection() 
            { 
                { 56.8, 2, 5.8m, 123L }, 
                { 86.8, 34, 59.8m, 3123L } 
            };
        }
    }

    public class TestEnum : System.Collections.IEnumerable
    {
        public TestEnum()
        {
        }

        public TestEnum(Nequeo.Convertible.Number number)
        {
            Nequeo.Convertible.Number _number = number;
            string nbnb = _number.ToString();
        }

        public void Add(string data, string next, string pr)
        {

        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserModel
    {
        public ObjectId Id { get; set; }
        public string user { get; set; }
    }

    /// <summary>
    /// The user data object class.
    /// </summary>
    public partial class User
    {

        private MongoDB.Bson.ObjectId _Id;

        private string _user;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Gets sets, the id property for the object.
        /// </summary>
        public MongoDB.Bson.ObjectId Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this._Id = value;
            }
        }

        /// <summary>
        /// Gets sets, the user property for the object.
        /// </summary>
        public string user
        {
            get
            {
                return this._user;
            }
            set
            {
                this._user = value;
            }
        }
    }
    
    /// <summary>
    /// The system.indexes data object class.
    /// </summary>
    public partial class systemindexes
    {
        
        private int _v;
        
        private string _name;
        
        private System.Collections.Generic.Dictionary<string, object> _key;
        
        private string _ns;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public systemindexes()
        {
        }
        
        /// <summary>
        /// Gets sets, the v property for the object.
        /// </summary>
        public int v
        {
            get
            {
                return this._v;
            }
            set
            {
                this._v = value;
            }
        }
        
        /// <summary>
        /// Gets sets, the name property for the object.
        /// </summary>
        public string name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        
        /// <summary>
        /// Gets sets, the key property for the object.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }
        
        /// <summary>
        /// Gets sets, the ns property for the object.
        /// </summary>
        public string ns
        {
            get
            {
                return this._ns;
            }
            set
            {
                this._ns = value;
            }
        }
    }
}
