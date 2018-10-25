using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Tables
{
    public class Student : TableEntity
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Country { get; set; }
    }
}