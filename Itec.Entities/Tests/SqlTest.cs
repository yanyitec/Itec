using Itec.Entities.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities.Tests
{
    public class SqlTest
    {
        public SqlTest() {
            this.DbSettings = new DB.DbSettings() {
                TablePrefix = "test_",
                ConnectionString = "Data Source=./sqlitedb.db"
            };
            this.Trait = new DB.DbTrait();
        }
        public DB.DbSettings DbSettings { get; set; }
        public DB.DbTrait Trait { get; set; }
        public void CreateTable() {
            var db = new Database(this.DbSettings,this.Trait);
            db.DropTableIfExists<Article>();
            db.CreateTable<Article>();
        }
    }
}
