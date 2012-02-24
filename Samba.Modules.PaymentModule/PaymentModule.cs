using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using Samba.Domain.Models.Accounts;
using Samba.Domain.Models.Tickets;
using Samba.Presentation.Common;
using Samba.Services;
using Samba.Services.Common;

namespace Samba.Modules.PaymentModule
{
    [ModuleExport(typeof(PaymentModule))]
    class PaymentModule : VisibleModuleBase
    {
        private readonly IRegionManager _regionManager;
        private readonly PaymentEditorView _paymentEditorView;
        private readonly ITicketService _ticketService;

        [ImportingConstructor]
        public PaymentModule(IRegionManager regionManager, PaymentEditorView paymentEditorView, ITicketService ticketService)
            : base(regionManager, AppScreens.Payment)
        {
            _regionManager = regionManager;
            _paymentEditorView = paymentEditorView;
            _ticketService = ticketService;
        }

        public override object GetVisibleView()
        {
            return _paymentEditorView;
        }

        protected override void OnInitialization()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(PaymentEditorView));

            EventServiceFactory.EventService.GetEvent<GenericEvent<Ticket>>().Subscribe(
             x =>
             {
                 if (x.Topic == EventTopicNames.MakePayment)
                 {
                     ((PaymentEditorViewModel)_paymentEditorView.DataContext).Prepare(x.Value);
                     Activate();
                 }
             });

            EventServiceFactory.EventService.GetEvent<GenericEvent<Account>>().Subscribe(
             x =>
             {
                 if (x.Topic == EventTopicNames.MakePayment)
                 {
                     var tickets = _ticketService.LoadTickets(y => y.AccountId == x.Value.Id && !y.IsPaid).ToArray();
                     ((PaymentEditorViewModel)_paymentEditorView.DataContext).Prepare(tickets);
                     Activate();
                 }
             });
        }
    }
}
