using Itec.ORMs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public async Task CreateTableAsync()
        {
            var db = new Database(this.DbSettings, this.DbTrait,null);
            db.DropTableIfExists<Article>();
            await db.CreateTableAsync<Article>();
            var dbSet = db.DbSet<Article>();
            var dbset = dbSet.MembersString("Id,Name,Age");
            var article1 = new Article()
            {
                Id = 1,
                Name = "Yiy",
                Description = "Yiy Desc",
                Age = 20
            };
            var article2 = new Article()
            {
                Id = 2,
                Name = "Yi",
                Description = "Yi Desc",
                Age = 30
            };
            (await dbset.InsertAsync(article1)).Insert(article2);

            dbset = dbset.Query(p=>p.Name=="Yiy");

            dbset.Load();
            Console.WriteLine("Loaded record{0}",dbset.Count());
        }
    }
}
