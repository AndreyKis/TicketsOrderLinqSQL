using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Provider;
using System.Threading.Tasks;
using Common.Tr_Tick_DBDataSetTableAdapters;

namespace AccesToTicketsDB
{
    public partial class AccessToTicketsDB
    {
        Tr_Tick_DBDataSet ticketsDataSet;
        Provider.Provider provider = new Provider.Provider();
        PriceTableAdapter priceAllAdapters = new PriceTableAdapter();
        TypeTableAdapter typeAllAdapters = new TypeTableAdapter();
        RateTableAdapter rateAllAdapters = new RateTableAdapter();
        PersonTableAdapter personAllAdapters = new PersonTableAdapter();
        TicketTableAdapter ticketAllAdapters = new TicketTableAdapter();
        //Это созданный отдельно адаптер, гляди в БД
        //Price1TableAdapter priceOneAdapter = new Price1TableAdapter();

        public AccessToTicketsDB ()
        {
            this.ticketsDataSet = this.provider.GetAllData();
        }
    }
}
