using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybox.App.Domain
{
    public class Transaction
    {
        public const int TransactionTimeSpacing = 60;
        public const string WithdrawalAccountId = "";
        public Guid Id { get; set; } = new Guid();
        public Guid FromAccountId { get; set; }

        public Guid ToAccountId { get; set; }

        public decimal Amount { get; set; }

        public DateTime InitiatedTime { get; set; }

        public DateTime? CompletionTime { get; set; }
        public bool Succeeded { get; set; }
        public Guid User { get; set; }
    }
}
