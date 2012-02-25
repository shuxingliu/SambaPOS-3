using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Samba.Domain.Models.Tickets;
using Samba.Infrastructure.Settings;
using Samba.Localization.Properties;
using Samba.Presentation.Common;

namespace Samba.Presentation.ViewModels
{
    public class TicketTotalsViewModel : ObservableObject
    {
        public IEnumerable<Ticket> Model { get; set; }
        public TicketTotalsViewModel(params Ticket[] model)
        {
            Model = model;
        }

        private ObservableCollection<PaymentViewModel> _payments;
        public ObservableCollection<PaymentViewModel> Payments
        {
            get
            {
                return _payments ?? (_payments = new ObservableCollection<PaymentViewModel>(
                    Model.SelectMany(x => x.Payments).Select(x => new PaymentViewModel(x))));
            }
        }

        private ObservableCollection<ServiceViewModel> _preServices;
        public ObservableCollection<ServiceViewModel> PreServices
        {
            get { return _preServices ?? (_preServices = new ObservableCollection<ServiceViewModel>(Model.SelectMany(x => x.Calculations).Where(x => !x.IncludeTax).Select(x => new ServiceViewModel(x)))); }
        }

        private ObservableCollection<ServiceViewModel> _postServices;
        public ObservableCollection<ServiceViewModel> PostServices
        {
            get { return _postServices ?? (_postServices = new ObservableCollection<ServiceViewModel>(Model.SelectMany(x => x.Calculations).Where(x => x.IncludeTax).Select(x => new ServiceViewModel(x)))); }
        }

        public decimal TicketTotalValue { get { return Model.Sum(x => x.GetSum()); } }
        public decimal TicketTaxValue { get { return Model.Sum(x => x.CalculateTax(x.GetPlainSum(), x.GetPreTaxServicesTotal())); } }
        public decimal TicketSubTotalValue { get { return Model.Sum(x => x.GetPlainSum() + x.GetPreTaxServicesTotal()); } }
        public decimal TicketPaymentValue { get { return Model.Sum(x => x.GetPaymentAmount()); } }
        public decimal TicketRemainingValue { get { return Model.Sum(x => x.GetRemainingAmount()); } }
        public decimal TicketPlainTotalValue { get { return Model.Sum(x => x.GetPlainSum()); } }

        public bool IsTicketTotalVisible { get { return TicketPaymentValue > 0 && TicketTotalValue > 0; } }
        public bool IsTicketPaymentVisible { get { return TicketPaymentValue > 0; } }
        public bool IsTicketRemainingVisible { get { return TicketRemainingValue > 0; } }
        public bool IsTicketTaxTotalVisible { get { return TicketTaxValue > 0; } }
        public bool IsPlainTotalVisible { get { return PostServices.Count > 0 || PreServices.Count > 0 || IsTicketTaxTotalVisible; } }
        public bool IsTicketSubTotalVisible { get { return PostServices.Count > 0 && PreServices.Count > 0; } }

        public string TicketPlainTotalLabel
        {
            get { return TicketPlainTotalValue.ToString(LocalSettings.DefaultCurrencyFormat); }
        }

        public string TicketTotalLabel
        {
            get { return TicketTotalValue.ToString(LocalSettings.DefaultCurrencyFormat); }
        }

        public string TicketTaxLabel
        {
            get { return TicketTaxValue.ToString(LocalSettings.DefaultCurrencyFormat); }
        }

        public string TicketSubTotalLabel
        {
            get { return TicketSubTotalValue.ToString(LocalSettings.DefaultCurrencyFormat); }
        }

        public string TicketPaymentLabel
        {
            get { return TicketPaymentValue.ToString(LocalSettings.DefaultCurrencyFormat); }
        }

        public string TicketRemainingLabel
        {
            get { return TicketRemainingValue.ToString(LocalSettings.DefaultCurrencyFormat); }
        }

        public string Title
        {
            get
            {
                if (Model.Count() == 0) return "";

                string selectedTicketTitle;
                var m = Model.OrderByDescending(x => x.Date).First();

                if (!string.IsNullOrEmpty(m.LocationName) && m.Id == 0)
                    selectedTicketTitle = string.Format(Resources.Location_f, m.LocationName);
                else if (!string.IsNullOrEmpty(m.AccountName) && m.Id == 0)
                    selectedTicketTitle = string.Format(Resources.Account_f, m.AccountName);
                else if (string.IsNullOrEmpty(m.AccountName)) selectedTicketTitle = string.IsNullOrEmpty(m.LocationName)
                     ? string.Format("# {0}", m.TicketNumber)
                     : string.Format(Resources.TicketNumberAndLocation_f, m.TicketNumber, m.LocationName);
                else if (string.IsNullOrEmpty(m.LocationName)) selectedTicketTitle = string.IsNullOrEmpty(m.AccountName)
                     ? string.Format("# {0}", m.TicketNumber)
                     : string.Format(Resources.TicketNumberAndAccount_f, m.TicketNumber, m.AccountName);
                else selectedTicketTitle = string.Format(Resources.AccountNameAndLocationName_f, m.TicketNumber, m.AccountName, m.LocationName);

                return selectedTicketTitle;
            }
        }

        public void ResetCache()
        {
            _payments = null;
            _preServices = null;
            _postServices = null;
        }
    }
}
