using System.Collections.Generic;
using Samba.Domain.Models.Tickets;

namespace Samba.Modules.PaymentModule
{
    public interface ITicketGroupViewModel
    {
        List<Ticket> Tickets { get; set; }
        bool ContainsItems { get; }
        bool Locked { get; }
        decimal TotalAmount { get; }
        void AddCalculation(CalculationTemplate calculationTemplate, decimal amount);
        decimal GetRemainingAmount();
        int GetPrintCount(int id);
        void ClearPaidItems();
        void Clear();
        void UpdateTickets(params Ticket[] tickets);
        void Recalculate();
        decimal GetPostTaxServicesTotal();
        decimal GetSum();
        IEnumerable<Order> GetCalulatingOrders();
        IEnumerable<PaidItem> GetPaidItems();
        void AddPaidItems(PaidItem paidItem);
        void AddPayment(PaymentTemplate paymentTemplate, decimal tenderedAmount);
        void SaveTickets();
    }
}