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
        public List<Person> GetAllPersons()
        {
            DataRowCollection searchedRow = this.ticketsDataSet.Person.Rows;
            return this.GetRestPartsOfPerson(searchedRow);
        }
        List<Person> GetRestPartsOfPerson(DataRowCollection searchedRows)
        {
            if (searchedRows == null || searchedRows.Count == null)
                return null;
            List<Person> Person = new List<Person>();
            foreach (Tr_Tick_DBDataSet.PersonRow personRow in searchedRows)
            {
                Person person = new Person();
                person.ID = personRow.ppers_id;
                person.Name = personRow.name;
                person.date_begin_ed = personRow.date_begin_ed;
                person.date_end_ed = personRow.date_end_ed;
                Person.Add(person);
            }
            return Person;
        }

        public int AddPerson(Person person)
        {
            int canAddPerson = IsUniquePerson(person);
            if (canAddPerson == 0)
            {
                ticketsDataSet.Person.AddPersonRow(person.Name, person.date_begin_ed, person.date_end_ed);
                provider.UpdateAllData();
                return 0;
            }
            else if (canAddPerson == 1)
                return 1;
            else
                return 2;
        }

        public int IsUniquePerson(Person person)
        {
            DataRow[] personRows = ticketsDataSet.Person.Select("[name] ='" + person.Name + "' AND" +
                "[date_begin_ed] ='" + person.date_begin_ed.ToString() + "' AND" + 
                "[date_end_ed] ='" + person.date_end_ed.ToString() + "'");
            DataRow[] personRowsNames = ticketsDataSet.Person.Select("[name] ='" + person.Name + "'");
            if (personRows.Length > 0)
                return 1;
            else if (personRowsNames.Length > 0)
                return 2;
            return 0;
        }

        public bool DeletePerson(Person person)
        {
            bool canDeletePerson = IsUsedPerson(person);
            if (canDeletePerson)
            {
                Tr_Tick_DBDataSet.PersonRow row = ticketsDataSet.Person.FindByppers_id(person.ID);
                row.Delete();
                //ticketsDataSet.Price.RemovePriceRow(row);
                provider.UpdateAllData();
                return true;
            }
            return false;
        }

        bool IsUsedPerson(Person person)
        {
            DataRow[] personsUsedInMainRows = ticketsDataSet.Main.Select("[pers_id] ='" + person.ID.ToString() + "'");
            if (personsUsedInMainRows.Length > 0)
                return false;
            return true;
        }
    }
}
