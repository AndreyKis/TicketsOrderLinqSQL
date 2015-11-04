using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using Common.Tr_Tick_DBDataSetTableAdapters;
using System.Threading.Tasks;

namespace AccesToTicketsDB
{
    public partial class AccessToTicketsDB
    {
        public List<Order> GetAllOrders()
        {
            //DataRowCollection searchedRow = this.ticketsDataSet.Ticket.Rows;
            DataRow[] searchedRow = this.ticketsDataSet.Main.Select();
            return this.GetRestPartsOfOrders(searchedRow);
        }

        List<Order> GetRestPartsOfOrders(DataRowCollection searchedRows)
        {
            if (searchedRows == null || searchedRows.Count == null)
                return null;
            List<Order> orders = new List<Order>();
            foreach (Tr_Tick_DBDataSet.MainRow orderRow in searchedRows)
            {
                Order order = new Order();
                order.ID = orderRow.m_id;
                order.PersonName = orderRow.PersonRow.name;
                order.TicketName = orderRow.TicketRow.ticket_name;
                order.Date = orderRow.month;
                order.Amount = orderRow.amount;
                order.Pledge = orderRow.pledge;
                orders.Add(order);
            }
            return orders;
        }

        List<Order> GetRestPartsOfOrders(DataRow[] searchedRows)
        {
            if (searchedRows == null || searchedRows.Length == null)
                return null;
            List<Order> orders = new List<Order>();
            foreach (Tr_Tick_DBDataSet.MainRow orderRow in searchedRows)
            {
                Order order = new Order();
                order.ID = orderRow.m_id;
                order.PersonName = orderRow.PersonRow.name;
                order.TicketName = orderRow.TicketRow.ticket_name;
                int tprice_id = orderRow.TicketRow.tprice_id;
                order.Date = orderRow.month;
                order.Amount = orderRow.amount;
                order.Pledge = orderRow.pledge;

                QueriesTableAdapter adapter = new QueriesTableAdapter();
                int priceRes = (int) adapter.ScalarQuery(order.TicketName);
                order.Sum = priceRes * order.Amount;

                orders.Add(order);
            }
            return orders;
        }

        public List<Order> GetSearchedOrders(Order searchedOrder, DateTime edDate)
        {
            string filter = null;
            //string price = "";
            string personNameFilter = "Parent(FK_Main_Person).name Like '%" + searchedOrder.PersonName + "%'";
            string ticketNamefilter = "Parent(FK_Main_Ticket).ticket_name Like '%" + searchedOrder.TicketName + "%'";
            string dateFilter = "month ='" + searchedOrder.Date.ToString() + "'";
            if (searchedOrder.PersonName != "" && searchedOrder.TicketName != "" && searchedOrder.Date.ToString() != "01.01.0001 0:00:00")
                filter = personNameFilter + " AND " + ticketNamefilter + " AND " + dateFilter;
            else if (searchedOrder.PersonName != "" && searchedOrder.TicketName != "")
                filter = personNameFilter + " AND " + ticketNamefilter;
            else if (searchedOrder.PersonName != "" && searchedOrder.Date.ToString() != "01.01.0001 0:00:00")
                filter = personNameFilter + " AND " + dateFilter;
            else if (searchedOrder.TicketName != "" && searchedOrder.Date.ToString() != "01.01.0001 0:00:00")
                filter = ticketNamefilter + " AND " + dateFilter;
            else if (searchedOrder.PersonName != "")
                filter = personNameFilter;
            else if (searchedOrder.TicketName != "")
                filter = ticketNamefilter;
            else if (searchedOrder.Date.ToString() != "01.01.0001 0:00:00")
                filter = dateFilter;


            DataRow[] searchedOrders = ticketsDataSet.Main.Select(filter);

            if (searchedOrders == null)
                return null;

            List<Order> orders = new List<Order>();

            foreach (DataRow tmpSearchedOrder in searchedOrders)
            {
                if (edDate.ToString() != "01.01.0001 0:00:00")
                {
                    if (edDate < Convert.ToDateTime(tmpSearchedOrder.GetParentRow("FK_Main_Person")["date_end_ed"].ToString()) &&
                        edDate > Convert.ToDateTime(tmpSearchedOrder.GetParentRow("FK_Main_Person")["date_begin_ed"].ToString()))
                        orders.Add(new Order(Convert.ToInt32(tmpSearchedOrder["m_id"].ToString()),
                            tmpSearchedOrder.GetParentRow("FK_Main_Person")["name"].ToString(),
                            tmpSearchedOrder.GetParentRow("FK_Main_Ticket")["ticket_name"].ToString(),
                            Convert.ToDateTime(tmpSearchedOrder["month"].ToString()),
                            Convert.ToInt32(tmpSearchedOrder["amount"].ToString()),
                            Convert.ToInt32(tmpSearchedOrder["pledge"].ToString())));
                }
                else
                    orders.Add(new Order(Convert.ToInt32(tmpSearchedOrder["m_id"].ToString()),
                            tmpSearchedOrder.GetParentRow("FK_Main_Person")["name"].ToString(),
                            tmpSearchedOrder.GetParentRow("FK_Main_Ticket")["ticket_name"].ToString(),
                            Convert.ToDateTime(tmpSearchedOrder["month"].ToString()),
                            Convert.ToInt32(tmpSearchedOrder["amount"].ToString()),
                            Convert.ToInt32(tmpSearchedOrder["pledge"].ToString())));
            }
            return orders;
        }

        public bool AddOrder(Order order, int personId, int ticketId)
        {
            bool canAdd = IsUniqueOrder(order, personId, ticketId);
            if (canAdd == true)
            {
                Tr_Tick_DBDataSet.MainRow orderRow =
                    ticketsDataSet.Main.AddMainRow(ticketsDataSet.Person.FindByppers_id(personId),
                    ticketsDataSet.Ticket.FindBytticket_id(ticketId), order.Date, order.Amount, order.Pledge);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUniqueOrder(Order order, int personId, int ticketId)
        {
            string personFilter = "pers_id ='" + personId.ToString() + "'";
            string ticketFilter = "ticket_id ='" + ticketId.ToString() + "'";
            string commonFilter = personFilter + " AND " + ticketFilter;
            DataRow[] commonOrdersRows = ticketsDataSet.Main.Select(commonFilter);
            if (commonOrdersRows != null && commonOrdersRows.Length > 0)
                return false;
            return true;
        }

        public bool DeleteOrder(Order order, /*int personId, int ticketId,*/ DateTime givingDate)
        {
            string personNameFilter = "Parent(FK_Main_Person).name Like '%" + order.PersonName + "%'";
            string ticketNamefilter = "Parent(FK_Main_Ticket).ticket_name Like '%" + order.TicketName + "%'";
            string filter = personNameFilter + " AND " + ticketNamefilter + " AND " + "month ='" + givingDate.ToString() + "'";
            DataRow[] OrdersRows = ticketsDataSet.Main.Select(filter);
            Tr_Tick_DBDataSet.MainRow row = null;
            if (OrdersRows.Length == 0 || OrdersRows == null)
                return false;
            else
            {
                foreach (DataRow tmpSearchedOrder in
                    OrdersRows)
                {
                    row = ticketsDataSet.Main.FindBym_id(Convert.ToInt32(tmpSearchedOrder["m_id"].ToString()));
                    //order.ID = Convert.ToInt32(tmpSearchedOrder["m_id"].ToString());
                }
                //Tr_Tick_DBDataSet.MainRow row = ticketsDataSet.Main.FindBym_id(Convert.ToInt32(tmpSearchedOrder["m_id"].ToString()));
                row.Delete();
                provider.UpdateAllData();
                return true;
            }
            
        }
    }
}
