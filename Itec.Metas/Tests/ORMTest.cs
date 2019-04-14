using Itec.ORMs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Tests
{
    public class ORMTest
    {
        public ORMTest()
        {
            this.DbSettings = new DbSettings()
            {
                TablePrefix = "test_",
                ConnectionString = "Data Source=./sqlitedb.db"
            };
            this.DbTrait = new DbTrait();
        }
        public DbSettings DbSettings { get; set; }
        public DbTrait DbTrait { get; set; }
        public void CreateTable()
        {
            var db = new Database(this.DbSettings, this.DbTrait,null);
            db.DropTableIfExists<Article>();
            db.CreateTable<Article>();
            var dbSet = db.DbSet<Article>();
            var dbset = dbSet.MembersString("Id,Name,Age");
            var article = new Article()
            {
                Id = 1,
                Name = "Yiy",
                Description = "Yiy Desc",
                Age = 20
            };

            var ok = dbset.Insert(article);
        }
    }
}
