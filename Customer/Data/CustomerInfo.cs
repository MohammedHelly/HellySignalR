using System;
using System.Collections.Generic;

#nullable disable

namespace Customer.Data
{
    public partial class CustomerInfo
    {
        public int Id { get; set; }
        public string CusId { get; set; }
        public string CusName { get; set; }
        public bool Status { get; set; }
    }
}
