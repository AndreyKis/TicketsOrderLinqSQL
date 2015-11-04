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
        /*Вытягиваем из БД по-сложному, с помощью провайдера.
        public List<Price> GetAllPrices()
        {
            DataRowCollection searchedRow = this.ticketsDataSet.Price.Rows;
            return this.GetRestPartsOfPrice(searchedRow);
        }
        List<Price> GetRestPartsOfPrice(DataRowCollection searchedRows)
        {
            if (searchedRows == null || searchedRows.Count == null)
                return null;
            List<Price> Price = new List<Price>();
            foreach (Tr_Tick_DBDataSet.PriceRow priceRow in searchedRows)
            {
                Price price = new Price();
                price.ID = priceRow.pprice_id;
                price.Value = priceRow.price_name;
                price.enter_date = priceRow.price_enter_date;
                Price.Add(price);
            }
            return Price;
        }
        public List<Ticket> GetSearchedPerson(Ticket searchedTicket)
        {
            //Получаем наборы найденых строк из соответсвующих таблиц
            Tr_Tick_DBDataSet.TicketRow[] searchedTicketRows =
                (Tr_Tick_DBDataSet.TicketRow[])ticketsDataSet.Ticket.Select("???????");
            Tr_Tick_DBDataSet.PriceRow[] searchedPriceRows =
                (Tr_Tick_DBDataSet.PriceRow[])ticketsDataSet.Price.Select("???????");
            Tr_Tick_DBDataSet.RateRow[] searchedAmountRows =
                (Tr_Tick_DBDataSet.RateRow[])ticketsDataSet.Rate.Select("??????????");
            Tr_Tick_DBDataSet.TypeRow[] searchedTypeRows =
                (Tr_Tick_DBDataSet.TypeRow[])ticketsDataSet.Type.Select("??????????");
            if (searchedTicketRows == null || searchedPriceRows == null ||
                searchedAmountRows == null || searchedTypeRows == null)
                return new List<Ticket>(0);
        }


        Добавление строки. После неё нужно обновлять провайдера.
        public void fff()
        {
            ticketsDataSet.Rate.AddRateRow("row");
        }*/

        public List<Price> GetAllPrices()
        {
            List<Price> list = new List<Price>();
            var resData = priceAllAdapters.GetData();
            foreach (var data in resData)
            {
                list.Add(new Price(data.pprice_id, data.price_name));
            }
            return list;                      
        }

        public bool AddPrice(Price price)
        {
            bool canAddPrice = IsUniquePrice(price);
            if (canAddPrice)
            {
                ticketsDataSet.Price.AddPriceRow(price.Value);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUniquePrice(Price price)
        {

            DataRow[] priceRows = ticketsDataSet.Price.Select("[price_name] ='" + price.Value.ToString() + "'");
            if (priceRows.Length > 0)
                return false;
            return true;
        }

        public bool DeletePrice(Price price)
        {
            bool canDeletePrice = IsUsedPrice(price);
            if (canDeletePrice)
            {
                Tr_Tick_DBDataSet.PriceRow row = ticketsDataSet.Price.FindBypprice_id(price.ID);
                row.Delete();
                //ticketsDataSet.Price.RemovePriceRow(row);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUsedPrice (Price price)
        {
            DataRow[] pricesUsedInTicketRows = ticketsDataSet.Ticket.Select("[tprice_id] ='" + price.ID.ToString() + "'");
            if (pricesUsedInTicketRows.Length > 0)
                return false;
            return true;
        }

        /*Пример как пользовать свой собственный адаптер
        public Price getPriceByID(int id)
        {

            Price tmp = null;

            var res = priceOneAdapter.GetData(id);

            if (res == null) return tmp;

            foreach (var r in res)
            {

                tmp = new Price(r.price_name, r.price_enter_date);

            }

            return tmp;
        }*/

    }
}

