using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Sync
{
    public interface IAbbreviatedContactDAL
    {
        void UpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts);
        void BulkUpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts);
    }

    public class AbbreviatedContactDAL : IAbbreviatedContactDAL
    {
        private readonly string _connectionString;
        public AbbreviatedContactDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void BulkUpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts)
        {
            try
            {
                DataTable dtAbbreviatedContact = ListToDataTable(abbreviatedContacts);
                using (SqlBulkCopy sqlbc = new SqlBulkCopy(_connectionString))
                {
                    sqlbc.DestinationTableName = "dbo.AbbreviatedContact";
                    sqlbc.ColumnMappings.Add("Id", "Id");
                    sqlbc.ColumnMappings.Add("Name", "Name");
                    sqlbc.ColumnMappings.Add("ContactType", "ContactType");
                    sqlbc.ColumnMappings.Add("ContactName", "ContactName");
                    sqlbc.ColumnMappings.Add("Address", "Address");
                    sqlbc.ColumnMappings.Add("Email", "Email");
                    sqlbc.ColumnMappings.Add("Phone", "Phone");

                    sqlbc.WriteToServer(dtAbbreviatedContact);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public void UpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts)
        {
            var cmdText = abbreviatedContacts.Aggregate(
                new StringBuilder(),
                (sb, abbreviatedcontacts) => sb.AppendLine($@"
                    INSERT into [dbo].[AbbreviatedContact] (Id, Name, ContactType, ContactName, Address, Email, Phone)
                    values('{abbreviatedcontacts.Id}', '{abbreviatedcontacts.Name}', 
                    '{abbreviatedcontacts.ContactType}', '{abbreviatedcontacts.ContactName}', 
                    '{abbreviatedcontacts.Address}', '{abbreviatedcontacts.Email}', '{abbreviatedcontacts.Phone}')"));

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(cmdText.ToString(), connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private DataTable ListToDataTable<T>(List<T> list)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
