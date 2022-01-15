using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneybox.App.Features.Exceptions
{
    public class InvalidAccountException : ApplicationException
    {
        public InvalidAccountException(string message) : base(message)
        {

        }

        public InvalidAccountException(string message, Exception exception) : base(message, exception)
        {

        }
    }
}
