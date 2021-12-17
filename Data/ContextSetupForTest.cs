
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
/*
 * This is a class to setup the inMemory sqllite db for the pair programming exam
 */

namespace Data
{
    public static class ContextSetupForTest
    {


        public static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Data Source=./SeedData/testing.db;");
            connection.Open();
            

            return connection;
        }
        public static string SeedDatabase(AdventureWorks2019Context context)
        {
            var returnString = "";
            try
            {

                context.Database.EnsureCreated();
       
                returnString += ":" + SeedEntity<Location>(context);
                returnString += ":" + SeedEntity<Product>(context);

                returnString += ":" + SeedEntity<ProductInventory>(context);
                returnString += ":" + SeedEntity<SalesOrderDetail>(context);

                return returnString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        private static string SeedEntity<T>(AdventureWorks2019Context ctx) where T : class
        {

            if (!ctx.Set<T>().Any())
            {
                var insertString = File.ReadAllText($@"./SeedData/{typeof(T).Name}.json");
                var entity = JsonSerializer.Deserialize<List<T>>(insertString);
                ctx.AddRange(entity);
                ctx.SaveChanges();
                return $@"{typeof(T).Name}.json completed";
            }

            return $@"{typeof(T).Name} nothing added";


        }
        
      
  
    }
    
}