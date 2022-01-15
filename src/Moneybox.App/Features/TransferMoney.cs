using Microsoft.Extensions.Logging;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features.Exceptions;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;
        private ITransactionRepository _transactionRepository;
        private ILogger<TransferMoney> _logger;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService,
                               ITransactionRepository transactionRepository, ILogger<TransferMoney> logger)
        {
            this._accountRepository = accountRepository;
            this._notificationService = notificationService;
            this._transactionRepository = transactionRepository;
            this._logger = logger;
        }

        public Transaction Execute(Transaction transaction)
        {
            try
            {
                var lastTransaction = this._transactionRepository.GetLastTransactionByUserId(transaction.FromAccountId);
                if (lastTransaction != null)
                {
                    if (lastTransaction.CompletionTime == null)
                    {
                        throw new InvalidOperationException("Another transaction is ongoing");
                    }

                    if (lastTransaction.InitiatedTime.AddSeconds(Transaction.TransactionTimeSpacing) > DateTime.Now && transaction.Amount == lastTransaction.Amount)
                    {
                        throw new InvalidOperationException("Transaction not within allowed time spacing");
                    }
                }

                this._transactionRepository.Add(transaction);

                var to = this._accountRepository.GetAccountById(transaction.ToAccountId);
                var from = this._accountRepository.GetAccountById(transaction.FromAccountId);
                if (from == null)
                {
                    throw new InvalidAccountException("From Account does not exist");
                }
                if (to == null)
                {
                    throw new InvalidAccountException("To Account does not exist");
                }
                var fromBalance = from.Balance - transaction.Amount;
                if (fromBalance < 0m)
                {
                    throw new InvalidOperationException("Insufficient funds to make transfer");
                }

                if (fromBalance < Account.LowFund)
                {
                    this._notificationService.NotifyFundsLow(from.User.Email);
                }

                var paidIn = to.PaidIn + transaction.Amount;
                if (paidIn > Account.PayInLimit)
                {
                    throw new InvalidOperationException("Account pay in limit reached");
                }

                if (Account.PayInLimit - paidIn < Account.PayInLimitWarningThreshold)
                {
                    this._notificationService.NotifyApproachingPayInLimit(to.User.Email);
                }

                from.Balance = fromBalance;
                from.Withdrawn = from.Withdrawn - transaction.Amount;

                to.Balance = to.Balance + transaction.Amount;
                to.PaidIn = paidIn;

                this._accountRepository.Update(from);
                this._accountRepository.Update(to);

                transaction.Succeeded = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transfer Money Error");
                transaction.Succeeded = false;
                throw;
            }
            finally
            {
                transaction.CompletionTime = DateTime.Now;
                this._transactionRepository.Update(transaction);
            }

            return transaction;
        }
    }
}
