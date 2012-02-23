using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Samba.Domain.Models.Accounts;
using Samba.Domain.Models.Tickets;
using Samba.Presentation.Common;
using Samba.Services;

namespace Samba.Modules.PosModule
{
    [Export]
    public class AccountTicketsViewModel : ObservableObject
    {
        private readonly ITicketService _ticketService;

        [ImportingConstructor]
        public AccountTicketsViewModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                if (SelectedAccount != null)
                {
                    OpenTickets = _ticketService.GetOpenTickets(x => x.AccountId == SelectedAccount.Id).Select(x => new OpenTicketButtonViewModel(x, true));
                }
                RaisePropertyChanged(() => SelectedAccount);
            }
        }

        private IEnumerable<OpenTicketButtonViewModel> _openTickets;
        public IEnumerable<OpenTicketButtonViewModel> OpenTickets
        {
            get { return _openTickets; }
            set
            {
                _openTickets = value;
                RaisePropertyChanged(() => OpenTickets);
            }
        }
    }
}
