using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moneybox.App.Features.Exceptions;
using Moq;

namespace Moneybox.Test.Features
{
    [TestClass]
    public class WithdrawMoneyTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidAccountException), "From Account does not exist")]
        public void ShouldThrowErrorIfFromAccountIsInvalid()
        {
            // Arrange

            var accountRepository = new Mock<IAccountRepository>();

            var notificationService = new Mock<INotificationService>();

            var logger = new Mock<ILogger<WithdrawMoney>>();

            var transactionRepository = new Mock<ITransactionRepository>();

            var newtransaction = new Transaction() { Amount = 100, FromAccountId = Guid.NewGuid(), ToAccountId = Guid.NewGuid() };

            // System under Test
            WithdrawMoney sut = new WithdrawMoney(accountRepository.Object, notificationService.Object,
                                                    transactionRepository.Object, logger.Object);

            // Act & Assert
            sut.Execute(newtransaction);
        }
    }
}
