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
        public List<TransportType> GetAllTransportTypes()
        {
            DataRowCollection searchedRow = this.ticketsDataSet.Type.Rows;
            return this.GetRestPartsOfTransportType(searchedRow);
        }
        List<TransportType> GetRestPartsOfTransportType(DataRowCollection searchedRows)
        {
            if (searchedRows == null || searchedRows.Count == null)
                return null;
            List<TransportType> TransportType = new List<TransportType>();
            foreach (Tr_Tick_DBDataSet.TypeRow transportTypeRow in searchedRows)
            {
                TransportType transportType = new TransportType();
                transportType.ID = transportTypeRow.ttype_id;
                transportType.Name = transportTypeRow.ttype_name;
                TransportType.Add(transportType);
            }
            return TransportType;
        }

        public bool AddType(TransportType type)
        {
            bool canAddType = IsUniqueType(type);
            if (canAddType)
            {
                ticketsDataSet.Type.AddTypeRow(type.Name);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUniqueType(TransportType type)
        {
            DataRow[] typeRows = ticketsDataSet.Type.Select("[ttype_name] ='" + type.Name.ToString() + "'");
            if (typeRows.Length > 0)
                return false;
            return true;
        }

        public bool DeleteType(TransportType type)
        {
            bool canDeleteType = IsUsedType(type);
            if (canDeleteType)
            {
                Tr_Tick_DBDataSet.TypeRow row = ticketsDataSet.Type.FindByttype_id(type.ID);
                row.Delete();
                //ticketsDataSet.Price.RemovePriceRow(row);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUsedType(TransportType type)
        {
            DataRow[] typesUsedInTicketRows = ticketsDataSet.Ticket.Select("[ttype_id] ='" + type.ID.ToString() + "'");
            if (typesUsedInTicketRows.Length > 0)
                return false;
            return true;
        }
    }
}