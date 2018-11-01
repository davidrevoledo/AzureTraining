using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Tables
{
    class Program
    {
        // Retrieve the storage account from the connection string.
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(@"DefaultEndpointsProtocol=https;AccountName=storageaccount1234david;AccountKey=KXu5bWOiCTh7d/PWluFOgDjjoJZHrrL2lLQXiGAT1vAHKsVx/sl/B31vJpXmA+N2PnqCmX9cCVvtxn0pBTVdlw==;EndpointSuffix=core.windows.net");

        static void Main(string[] args)
        {
            MainAsync()
                .GetAwaiter()
                .GetResult();

            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            //await CreateTable();

            //List<Task> tasks = new List<Task>();

            //for (int i = 0; i < 10000; i++)
            //{
            //    tasks.Add(InsertEntities());
            //}

            //await Task.WhenAll(tasks);

            //await ExecuteBatch();

            await GetEntities();

            //await GetEntity();

            //await ReplaceEntity();

            //await InsertOrReplace();

            // await DeleteTable();
        }

        private static async Task DeleteTable()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            await table.DeleteIfExistsAsync();
        }

        private static async Task InsertOrReplace()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            Student student = new Student
            {
                Name = "Jack",
                Surname = "Stuart",
                Country = "USA",
                RowKey = "213123",
                PartitionKey = "Sistemas"
            };

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(student);
            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        private static async Task ReplaceEntity()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            // Create a retrieve operation that takes a entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<Student>("Sistemas", "35003399");

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult != null)
            {
                var student = (Student)retrievedResult.Result;

                Console.WriteLine($"Name : {student.Name}");


                student.Name = "Paul";

                // Create the Replace TableOperation.
                TableOperation updateOperation = TableOperation.Replace(student);

                // Execute the operation.
                await table.ExecuteAsync(updateOperation);
            }
        }

        private static async Task GetEntity()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            // Create a retrieve operation that takes a entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<Student>("Sistemas", "dsadasdas");

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                Console.WriteLine(((Student)retrievedResult.Result).Name);
            }
            else
            {
                Console.WriteLine("No results.");
            }
        }

        private static async Task GetEntities()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<Student> query = new TableQuery<Student>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Sistemas"));


            int i = 1;

            await ExecuteQueryAsync(table, query, CancellationToken.None, list =>
            {
                foreach (var student in list)
                {
                    i++;
                    Console.WriteLine($"{student.Name} {student.Surname} {student.Country} number : {i}");
                }
            });
        }

        public static async Task<IList<T>> ExecuteQueryAsync<T>(
            CloudTable table,
            TableQuery<T> query,
            CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null)
            where T : ITableEntity, new()
        {
            var runningQuery = new TableQuery<T>
            {
                FilterString = query.FilterString,
                SelectColumns = query.SelectColumns
            };

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                runningQuery.TakeCount = query.TakeCount - items.Count;

                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync(runningQuery, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                onProgress?.Invoke(items);

            } while (token != null && !ct.IsCancellationRequested && (query.TakeCount == null || items.Count < query.TakeCount.Value));

            return items;
        }

        private static async Task ExecuteBatch()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            Student student = new Student
            {
                Name = "David",
                Surname = "Revoledo",
                Country = "Argentina",
                RowKey = "35449987",
                PartitionKey = "Sistemas"
            };

            Student student2 = new Student
            {
                Name = "Miguel",
                Surname = "Suarez",
                Country = "Argentina",
                RowKey = "35003399",
                PartitionKey = "Sistemas"
            };

            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(student);
            batchOperation.Delete(student2);

            // Execute the batch operation.
            await table.ExecuteBatchAsync(batchOperation);
        }

        private static async Task InsertEntities()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            Student student = new Student
            {
                Name = "David",
                Surname = "Revoledo",
                Country = "Argentina",
                RowKey = Guid.NewGuid().ToString(),
                PartitionKey = "Sistemas"
            };

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(student);

            // Execute the insert operation.
            await table.ExecuteAsync(insertOperation);
        }

        private static async Task CreateTable()
        {
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("students");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
        }
    }
}
