using System.Collections.Generic;
using Samba.Infrastructure.Data;

namespace Samba.Domain.Models.Accounts
{
    public class AccountScreen : Entity, IOrderable
    {
        public int Order { get; set; }
        public int DisplayMode { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string LocationEmptyColor { get; set; }
        public string LocationFullColor { get; set; }
        public string LocationLockedColor { get; set; }
        public int PageCount { get; set; }
        public int ColumnCount { get; set; }
        public int ButtonHeight { get; set; }
        public int NumeratorHeight { get; set; }
        public string AlphaButtonValues { get; set; }

        private IList<AccountButton> _buttons;
        public virtual IList<AccountButton> Buttons
        {
            get { return _buttons; }
            set { _buttons = value; }
        }

        public string UserString
        {
            get { return Name; }
        }

        public bool IsBackgroundImageVisible { get { return !string.IsNullOrEmpty(BackgroundImage); } }

        public AccountScreen()
        {
            _buttons = new List<AccountButton>();
            LocationEmptyColor = "WhiteSmoke";
            LocationFullColor = "Orange";
            LocationLockedColor = "Brown";
            BackgroundColor = "Transparent";
            PageCount = 1;
            ButtonHeight = 0;
        }

        public int ItemCountPerPage
        {
            get
            {
                var itemCount = Buttons.Count / PageCount;
                if (Buttons.Count % PageCount > 0) itemCount++;
                return itemCount;
            }
        }

        public void AddScreenItem(AccountButton choosenValue)
        {
            if (!Buttons.Contains(choosenValue))
                Buttons.Add(choosenValue);
        }
    }
}
