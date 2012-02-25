using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCalc;
using Samba.Infrastructure.Data;

namespace Samba.Domain.Models.Accounts
{
    public class AccountTransaction : Entity
    {
        private decimal _amount;
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                if (SourceTransactionValue != null)
                    SourceTransactionValue.Credit = value;
                if (TargetTransactionValue != null)
                    TargetTransactionValue.Debit = value;
            }
        }

        public int AccountTransactionTemplateId { get; set; }
        public int AccountTransactionDocumentId { get; set; }
        public virtual AccountTransactionValue SourceTransactionValue { get; set; }
        public virtual AccountTransactionValue TargetTransactionValue { get; set; }

        private static AccountTransaction _null;
        public static AccountTransaction Null
        {
            get
            {
                return _null ?? (_null = new AccountTransaction
                                             {
                                                 SourceTransactionValue = new AccountTransactionValue(),
                                                 TargetTransactionValue = new AccountTransactionValue()
                                             });
            }
        }

        public static AccountTransaction Create(AccountTransactionTemplate template)
        {
            var result = new AccountTransaction
                             {
                                 Name = template.Name,
                                 AccountTransactionTemplateId = template.Id,
                                 SourceTransactionValue = new AccountTransactionValue(),
                                 TargetTransactionValue = new AccountTransactionValue()
                             };

            result.SourceTransactionValue.Name = template.Name;
            result.SourceTransactionValue.AccountTemplateId = template.SourceAccountTemplateId;
            result.SourceTransactionValue.AccountId = template.DefaultSourceAccountId;

            result.TargetTransactionValue.Name = template.Name;
            result.TargetTransactionValue.AccountId = template.DefaultTargetAccountId;
            result.TargetTransactionValue.AccountTemplateId = template.TargetAccountTemplateId;

            return result;
        }

        public void SetSoruceAccount(int accountTemplateId, int accountId)
        {
            SourceTransactionValue.AccountTemplateId = accountTemplateId;
            SourceTransactionValue.AccountId = accountId;
        }

        public void SetTargetAccount(int accountTemplateId, int accountId)
        {
            TargetTransactionValue.AccountTemplateId = accountTemplateId;
            TargetTransactionValue.AccountId = accountId;
        }

        public void UpdateAccounts(int accountTemplateId, int accountId)
        {
            if (SourceTransactionValue.AccountTemplateId == accountTemplateId)
                SetSoruceAccount(accountTemplateId, accountId);
            if (TargetTransactionValue.AccountTemplateId == accountTemplateId)
                SetTargetAccount(accountTemplateId, accountId);
        }
    }
}
