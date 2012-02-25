using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Samba.Infrastructure.Data;

namespace Samba.Domain.Models.Accounts
{
    public class AccountTransactionDocument : Entity
    {
        public AccountTransactionDocument()
        {
            _accountTransactions = new List<AccountTransaction>();
            Date = DateTime.Now;
        }

        public DateTime Date { get; set; }

        private IList<AccountTransaction> _accountTransactions;
        public virtual IList<AccountTransaction> AccountTransactions
        {
            get { return _accountTransactions; }
            set { _accountTransactions = value; }
        }

        public void CreateSingleTransaction(AccountTransactionTemplate template, int accountTemplateId, int accountId)
        {
            var transaction = AccountTransactions.SingleOrDefault(x => x.AccountTransactionTemplateId == template.Id);
            if (transaction == null)
            {
                transaction = AccountTransaction.Create(template);
                AccountTransactions.Add(transaction);
            }
            transaction.UpdateAccounts(accountTemplateId, accountId);
        }

        public AccountTransaction AddTransaction(AccountTransactionTemplate accountTransactionTemplate,
            decimal amount, Account targetAccount, int sourceAccountTemplateId, int sourceAccountId)
        {
            var transaction = AccountTransaction.Create(accountTransactionTemplate);
            transaction.Amount = amount;
            transaction.SetTargetAccount(targetAccount.AccountTemplateId, targetAccount.Id);
            transaction.SetSoruceAccount(sourceAccountTemplateId, sourceAccountId);
            AccountTransactions.Add(transaction);
            return transaction;
        }

        public void AddTransaction2(AccountTransactionTemplate accountTransactionTemplate, string name,
            int accountTemplateId, int accountId)
        {
            var transaction = AccountTransaction.Create(accountTransactionTemplate);
            transaction.Name = name;
            transaction.UpdateAccounts(accountTemplateId, accountId);
            AccountTransactions.Add(transaction);
        }
    }
}
