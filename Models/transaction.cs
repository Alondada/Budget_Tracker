using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Models
{
    public class Transaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }
        
        public string Description { get; set; }
    }
}
