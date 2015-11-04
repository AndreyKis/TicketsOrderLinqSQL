using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Data;

namespace AccesToTicketsDB
{
    public partial class AccessToTicketsDB
    {
        public List<Rate> GetAllRates()
        {
            DataRowCollection searchedRow = this.ticketsDataSet.Rate.Rows;
            return this.GetRestPartsOfRate(searchedRow);
        }
        List<Rate> GetRestPartsOfRate(DataRowCollection searchedRows)
        {
            if (searchedRows == null || searchedRows.Count == 0)
                return null;
            List<Rate> Rate = new List<Rate>();
            foreach (Tr_Tick_DBDataSet.RateRow rateRow in searchedRows)
            {
                Rate rate = new Rate();
                rate.ID = rateRow.rrate_id;
                rate.Name = rateRow.rrate_name;
                Rate.Add(rate);
            }
            return Rate;
        }

        public bool AddRate(Rate rate)
        {
            bool canAdd = IsUniqueRate(rate);
            if (canAdd)
            {
                ticketsDataSet.Rate.AddRateRow(rate.Name);
                provider.UpdateAllData();
                /*List<Rate> rateList = new List<Rate>();
                DataRow[] rateRow = ticketsDataSet.Rate.Select("[rrate_name] ='" + rate.Name.ToString() + "'");
                foreach(DataRow tmpRate in rateRow)
                {
                    rateList.Add(new Rate(Int32.Parse(tmpRate["rrate_id"].ToString()), tmpRate["rrate_name"].ToString()));
                }
                rate.ID = rateList[0].ID;*/
                return true;
            }
            return false;
        }

        public bool IsUniqueRate(Rate rate)
        {
            DataRow[] rateRow = ticketsDataSet.Rate.Select("[rrate_name] ='" + rate.Name.ToString() + "'");
            if (rateRow.Length > 0)
                return false;
            return true;
        }

        public bool DeleteRate(Rate rate)
        {
            bool canDeleteRate = IsUsedRate(rate);
            if (canDeleteRate)
            {
                Tr_Tick_DBDataSet.RateRow row = ticketsDataSet.Rate.FindByrrate_id(rate.ID);
                row.Delete();
                //ticketsDataSet.Price.RemovePriceRow(row);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUsedRate(Rate rate)
        {
            DataRow[] ratesUsedInTicketRows = ticketsDataSet.Ticket.Select("[trate_id] ='" + rate.ID.ToString() + "'");
            if (ratesUsedInTicketRows.Length > 0)
                return false;
            return true;
        }
    }
}
