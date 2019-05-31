using Itec.ORMs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Tests
{
    [Fact]

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

        [Fact]
        public async Task CreateAndCheckTableAsync()
        {
            var db = new Database(this.DbSettings, this.DbTrait,null);
            //ifdb.CheckTableExists<Article>()
            await db.DropTableIfExistsAsync<Article>();
            await db.CreateTableAsync<Article>();
            var existed = await db.TableExistsAsync<Article>();
            Fact.True(existed, "创建了Article表，但未能检查到该表存在");


            
        }

        [Fact]
        public void CreateAndCheckTable()
        {
            var db = new Database(this.DbSettings, this.DbTrait, null);
            //ifdb.CheckTableExists<Article>()
            db.DropTableIfExists<Article>();
            db.CreateTable<Article>();
            var existed = db.TableExists<Article>();
            Fact.True(existed, "创建了Article表，但未能检查到该表存在");



        }

        [Fact]
        public async Task InsertAndGetByIdAsync() {
            var db = new Database(this.DbSettings, this.DbTrait, null);
            //ifdb.CheckTableExists<Article>()
            await db.DropTableIfExistsAsync<Article>();
            await db.CreateTableAsync<Article>();


            var repo = db.Repository<Article>();
            //var dbset = dbSet.MembersString("Id,Name,Age");
            var article1 = new Article()
            {
                Id = 1,
                Name = "Yiy",
                Description = "Yiy Desc",
                Age = 20
            };
            Fact.True(await repo.InsertAsync(article1),"未能插入数据");
            var article1_db = await repo.GetByIdAsync(1);
            Fact.NotNull(article1_db);
            Fact.Equal(article1_db.Id,1,"取出的Id不为1");
            Fact.Equal(article1_db.Name, article1.Name, "取出的Name与原先不一样");
            Fact.Equal(article1_db.Description, article1.Description, "取出的Description与原先不一样");
            Fact.True(article1_db.Age.HasValue, "未取出Age");
            Fact.Equal(article1_db.Age.Value,article1.Age.Value,"取出的Age与原先不一样");

            article1_db = await repo.GetByIdAsync(1,new RepoOptions() {AllowedFields="Name,Age" });

            Fact.NotNull(article1_db);
            Fact.Equal(article1_db.Id, 1, "取出的Id不为1");
            Fact.Equal(article1_db.Name, article1.Name, "取出的Name与原先不一样");
            Fact.Null(article1_db.Description, "取出的Description与原先不一样");
            Fact.True(article1_db.Age.HasValue, "未取出Age");
            Fact.Equal(article1_db.Age.Value, article1.Age.Value, "取出的Age与原先不一样");
            //var article2 = new Article()
            //{
            //    Id = 2,
            //    Name = "yy",
            //    Description = "yy Desc",
            //    Age = 30
            //};

            //repo.Insert(article2);

            //for (var i = 0; i < 7; i++)
            //{
            //    article2.Id++;
            //    article2.Name = "xxx" + i.ToString();
            //    repo.Insert(article2);
            //}

            //dbset = dbset.Query(p=>p.Name.StartsWith("xxx")).Page(2,2).Descending(p=>p.Id);
            //var count = dbset.Count();
            //Console.WriteLine("Count:" + count.ToString());
            //dbset.Load();
            //var len = dbset.Length;
            //Console.WriteLine("Loaded record{0}",len);
        }

        [Fact]
        public async Task ListAsync()
        {
            var db = new Database(this.DbSettings, this.DbTrait, null);
            //ifdb.CheckTableExists<Article>()
            await db.DropTableIfExistsAsync<Article>();
            await db.CreateTableAsync<Article>();


            var repo = db.Repository<Article>();

            var article2 = new Article()
            {
                Id = 1,
                Name = "yy01",
                Description = "yy Desc01",
                Age = 30
            };

            

            for (var i = 0; i < 12; i++)
            {
                article2.Id = i;
                article2.Name = "yy" + article2.Id.ToString("00");
                article2.Description = "yy Desc" + article2.Id.ToString("00");
                article2.Age = i+20;
                repo.Insert(article2,new RepoOptions() { AllowedFields="Name,Age"});
            }

            var pageable = new Pageable<Article>();
            pageable.AndAlso(p=>p.Age<30).Descending(p=>p.Age).Skip(3).Take(2);
            var list = await repo.ListAsync(pageable);

            Fact.Equal(2,list.Count);
            Fact.Equal(list[0].Id,6,"第一个实体.Id应该等于6");
            Fact.Equal(list[1].Id,5, "第一个实体.Id应该等于5");

            
        }
    }
}
