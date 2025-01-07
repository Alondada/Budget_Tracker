using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Models
{
    public class User
    {
        public required string username { get; set; }
        public required string password { get; set; }
        public required string mail { get; set; }

        public int cashInflow { get; set; } = 0;

        public int cashOutflow { get; set; } = 0;

        public int debt { get; set; } = 0;

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
