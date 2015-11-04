using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Threading.Tasks;

namespace AccesToTicketsDB
{
    public partial class AccessToTicketsDB
    {
        //Вытягиваем по-сложному Тикеты
        public List<Ticket> GetAllTickets()
        {
            //DataRowCollection searchedRow = this.ticketsDataSet.Ticket.Rows;
            DataRow[] searchedRow = this.ticketsDataSet.Ticket.Select();
            return this.GetRestPartsOfTicket(searchedRow);
        }

        List<Ticket> GetRestPartsOfTicket(DataRowCollection searchedRows)
        {
            if (searchedRows == null || searchedRows.Count == null)
                return null;
            List<Ticket> tickets = new List<Ticket>();
            foreach (Tr_Tick_DBDataSet.TicketRow ticketRow in searchedRows)
            {
                Ticket ticket = new Ticket();
                    ticket.ID = ticketRow.tticket_id;
                    ticket.Name = ticketRow.ticket_name;
                    ticket.Price = ticketRow.PriceRow.price_name;
                    ticket.Rate = ticketRow.RateRow.rrate_name;
                    ticket.Type = ticketRow.TypeRow.ttype_name;
                tickets.Add(ticket);
            }
            return tickets;
        }

        List<Ticket> GetRestPartsOfTicket(DataRow[] searchedRows)
        {
            if (searchedRows == null || searchedRows.Length == 0)
                return null;
            List<Ticket> tickets = new List<Ticket>();
            foreach(Tr_Tick_DBDataSet.TicketRow ticketRow in searchedRows)
            {
                Ticket ticket = new Ticket();
                ticket.ID = ticketRow.tticket_id;
                ticket.Name = ticketRow.ticket_name;
                ticket.Price = ticketRow.PriceRow.price_name;
                ticket.Rate = ticketRow.RateRow.rrate_name;
                ticket.Type = ticketRow.TypeRow.ttype_name;
                tickets.Add(ticket);
            }
            return tickets;
        }

        /*Пробуем вытягивать тикеты по-простому
        public List<Ticket> GetAllTickets()
        {
            List<Ticket> list = new List<Ticket>();
            var resData = ticketAllAdapters.GetData();
            foreach (var data in resData)
            {
                list.Add(new Ticket(data.tticket_id, data.ticket_name, data.tprice_id, data.);
            }
            return list;
        }*/

        //Фильтруем красиво
        public List<Ticket> GetSearchedTickets(Ticket searchedTicket)
        {
            string filter = "";
            String price = "";
            if (searchedTicket.Price != 0)
                filter += "Parent(FK_Ticket_Price).price_name ='" + searchedTicket.Price.ToString() + "' AND ";

                filter += "Parent(FK_Ticket_Type).ttype_name Like '%" + searchedTicket.Type + "%' AND " +
                  
                    "Parent(FK_Ticket_Rate).rrate_name Like '%" + searchedTicket.Rate + "%'";


            DataRow[] searchedTickets = this.ticketsDataSet.Ticket.Select(filter);
            
            if (searchedTickets == null)
                return null;

            List<Ticket> tickets = new List<Ticket>();

            foreach (DataRow tmpSearchedTicket in searchedTickets)
            {   
                tickets.Add(
                    new Ticket(Int32.Parse(tmpSearchedTicket["tticket_id"].ToString()), 
                        tmpSearchedTicket["ticket_name"].ToString(),
                        Convert.ToInt32(tmpSearchedTicket.GetParentRow("FK_Ticket_Price")["price_name"]),
                        tmpSearchedTicket.GetParentRow("FK_Ticket_Rate")["rrate_name"].ToString(),
                        tmpSearchedTicket.GetParentRow("FK_Ticket_Type")["ttype_name"].ToString()));
            }

            return tickets;
        }

        public int AddTicket(Ticket ticket, int priceId, int rateId, int typeId)
        {
            int canAdd = IsUniqueTicket(ticket, priceId, rateId, typeId);
            if (canAdd == 0)
            {
                    ticketsDataSet.Ticket.AddTicketRow(ticket.Name, ticketsDataSet.Price.FindBypprice_id(priceId),
                    ticketsDataSet.Rate.FindByrrate_id(rateId), ticketsDataSet.Type.FindByttype_id(typeId));
                provider.UpdateAllData();
                return 0;
            }
            else if (canAdd == 1)
                return 1;
            return 2;
        }

        int IsUniqueTicket(Ticket ticket, int priceId, int rateId, int typeId)
        {
            string namesFilter = "[ticket_name] Like '%" + ticket.Name + "%'";
            string pricesFilter = "tprice_id ='" + priceId.ToString() + "'";
            string ratesFilter = "trate_id ='" + rateId.ToString() + "'";
            string typesFilter = "ttype_id ='" + typeId.ToString() + "'";
            string commonFilter = namesFilter + " AND " + pricesFilter + " AND " +
                ratesFilter + " AND " + typesFilter;
            DataRow[] namesTicketsRows = ticketsDataSet.Ticket.Select(namesFilter);
            DataRow[] pricesTicketsRows = ticketsDataSet.Ticket.Select(pricesFilter);
            DataRow[] ratesFilterRows = ticketsDataSet.Ticket.Select(ratesFilter);
            DataRow[] typesFilterRows = ticketsDataSet.Ticket.Select(typesFilter);
            DataRow[] commonTicketsRows = ticketsDataSet.Ticket.Select(commonFilter);
            if (commonTicketsRows != null && commonTicketsRows.Length > 0)
                return 1;
            else if (namesTicketsRows != null && namesTicketsRows.Length > 0)
                return 2;
            
                return 0;
        }

        public bool DeleteTicket(Ticket ticket)
        {
            bool canDeleteTicket = IsUsedTicket(ticket);
            if (canDeleteTicket)
            {
                Tr_Tick_DBDataSet.TicketRow row = ticketsDataSet.Ticket.FindBytticket_id(ticket.ID);
                row.Delete();
                //ticketsDataSet.Price.RemovePriceRow(row);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUsedTicket(Ticket ticket)
        {
            DataRow[] ticketsUsedInMainRows = ticketsDataSet.Main.Select("[ticket_id] ='" + ticket.ID.ToString() + "'");
            if (ticketsUsedInMainRows.Length > 0)
                return false;
            return true;
        }
        
        /*Эксперименты с классическим способом фильтрации
        public List<Ticket> GetSearchedTickets(Ticket searchedTicket)
        {
            //Получаем наборы найденых строк из соответсвующих таблиц
            /*Tr_Tick_DBDataSet.PriceRow[] searchedPriceRows =
                (Tr_Tick_DBDataSet.PriceRow[])ticketsDataSet.Price.Select(MakeFilteredQuery(searchedTicket.PriceFields));
            Tr_Tick_DBDataSet.RateRow[] searchedAmountRows =
                (Tr_Tick_DBDataSet.RateRow[])ticketsDataSet.Rate.Select(MakeFilteredQuery(searchedTicket.RateFields));
            Tr_Tick_DBDataSet.TypeRow[] searchedTypeRows =
                (Tr_Tick_DBDataSet.TypeRow[])ticketsDataSet.Type.Select(MakeFilteredQuery(searchedTicket.TypeFields));
            if (searchedPriceRows == null || searchedAmountRows == null || searchedTypeRows == null)
                return new List<Ticket>(0);
            //Создаём списки для набора строк для более гибкого манипулирования эл-тами массива
            List<Tr_Tick_DBDataSet.PriceRow> searchedPriceRowsList =
                searchedPriceRows.ToList<Tr_Tick_DBDataSet.PriceRow>();
            List<Tr_Tick_DBDataSet.RateRow> searchedAmountRowsList =
                searchedAmountRows.ToList<Tr_Tick_DBDataSet.RateRow>();
            List<Tr_Tick_DBDataSet.TypeRow> searchedTypeRowsList =
                searchedTypeRows.ToList<Tr_Tick_DBDataSet.TypeRow>();
            List<int> priceRowName = new List<int>();
            List<string> rateRowName = new List<string>();
            List<string> typeRowName = new List<string>();
            
            foreach(Tr_Tick_DBDataSet.PriceRow priceRow in searchedPriceRowsList)
            {
                if (priceRow.price_name != null)
                    priceRowName.Add(priceRow.price_name); 
            }
            //foreach (Tr_Tick_DBDataSet.RateRow rateRow in searchedAmountRowsList)
            //{
              //  if()
            //}
            //конечный набор искомых строк
            List<Tr_Tick_DBDataSet.TicketRow> realTicketsRow = new List<Tr_Tick_DBDataSet.TicketRow>();
            DataRow[] searchedPrices = 
                ticketsDataSet.Price.Select("SELECT *FROM Price WHERE price_name LIKE" + searchedTicket.Price);
            DataRow[] searchedRates =
                ticketsDataSet.Rate.Select("SELECT *FROM Rate WHERE rrate_name LIKE" + searchedTicket.Rate);
            DataRow[] searchedTypes =
                ticketsDataSet.Price.Select("SELECT *FROM Type WHERE ttype_name LIKE" + searchedTicket.Type);
            List<Ticket> tickets;
            List<Price> prices = new List<Price>();
            List<Rate> rates = new List<Rate>();
            List<TransportType> types = new List<TransportType>();
            foreach(Tr_Tick_DBDataSet.PriceRow priceRow in searchedPrices)
            {
                Price price = new Price();
                if (priceRow != null)
                    price.Value = priceRow.price_name;
                    prices.Add(price);
            }
            foreach (Tr_Tick_DBDataSet.RateRow rateRow in searchedRates)
            {
                Rate rate = new Rate();
                if (rateRow != null)
                    rate.Name = rateRow.rrate_name;
                rates.Add(rate);
            }
            foreach (Tr_Tick_DBDataSet.TypeRow typeRow in searchedTypes)
            {
                TransportType type = new TransportType();
                if (typeRow != null)
                    type.Name= typeRow.ttype_name;
                types.Add(type);
            }
            while (true)
            {
                tickets.get
            }
        }*/
    }
}
