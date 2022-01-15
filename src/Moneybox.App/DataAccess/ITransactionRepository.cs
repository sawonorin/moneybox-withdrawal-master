using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moneybox.App.Domain;

namespace Moneybox.App.DataAccess
{
    public interface ITransactionRepository
    {
        Transaction GetLastTransactionByUserId(Guid accountId);
        void Add(Transaction transaction);
        void Update(Transaction transaction);
    }
}
