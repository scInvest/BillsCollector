using System;
using System.Collections.Generic;
using System.Text;

namespace Integrations.MBank
{
    public class BankTransaction
    {
        public DateTime OperationDate { get; set; }
        public string Description { get; set; }
        public string Account { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}
