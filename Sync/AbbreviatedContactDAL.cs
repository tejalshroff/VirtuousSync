using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    public interface IAbbreviatedContactDAL
    {
        void UpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts);
        void BulkUpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts);
    }

    public class AbbreviatedContactDAL
    {
        public string connectionString;
        private readonly IConfiguration _iconfiguration;
        public AbbreviatedContactDAL(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            connectionString = _iconfiguration.GetValue("ConnectionString");
        }

        public void BulkUpdateAbbreviatedContactAsync(List<AbbreviatedContact> abbreviatedContacts)
        {
            try
            {
                DataTable dtAbbreviatedContact = _iconfiguration.ListToDataTable(abbreviatedContacts);
                using (SqlBulkCopy sqlbc = new SqlBulkCopy(connectionString))
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

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(cmdText.ToString(), connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
