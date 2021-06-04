using System;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.EventArgs;

namespace TableChangeMonitor
{
    public partial class CustomerInfo
    {
        public int Id { get; set; }
        public string CusId { get; set; }
        public string CusName { get; set; }
        public bool Status { get; set; }
    }
    class Program
    {
        private static string _con = "data source=.; initial catalog=Customer; integrated security=True";
       // "Data Source=.;Initial Catalog=Customer;Integrated Security=True"
        static void Main(string[] args)
        {
            var mapper = new ModelToTableMapper<CustomerInfo>();
            mapper.AddMapping(c => c.CusName, "CusName");
            mapper.AddMapping(c => c.Status, "Status");

            // Here - as second parameter - we pass table name: 
            // this is necessary only if the model name is different from table name 
            // (in our case we have Customer vs Customers). 
            // If needed, you can also specifiy schema name.
            using (var dep = new SqlTableDependency<CustomerInfo>(_con, "CustomerInfo", mapper: mapper)) 
            {
                dep.OnChanged += Changed;
                dep.Start();

                Console.WriteLine("Press a key to exit");
                Console.ReadKey();

                dep.Stop();
            }
        }

        public static void Changed(object sender, RecordChangedEventArgs<CustomerInfo> e)
        {
            var changedEntity = e.Entity;

            Console.WriteLine("DML operation: " + e.ChangeType);
            Console.WriteLine("ID: " + changedEntity.Id);
            Console.WriteLine("Name: " + changedEntity.CusName);
            Console.WriteLine("Surname: " + changedEntity.Status);
        }
    }
}
