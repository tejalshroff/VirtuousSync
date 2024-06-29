using CsvHelper;
using Microsoft.Extensions.Configuration;
using Sync.ConfigurationOptions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sync
{
    internal class Program
    {
        private static IConfiguration _iconfig;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            _iconfig = builder.Build();
            
            Sync().GetAwaiter().GetResult();
        }

        private static async Task Sync()
        {
            var ConnectionString = _iconfig.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            var AuthorizationOption = _iconfig.GetSection("AuthorizationOption").Get<AuthorizationOption>();
            IVirtuousService _virtuousService = new VirtuousService(AuthorizationOption); ;
            IAbbreviatedContactDAL _contactDAL = new AbbreviatedContactDAL(ConnectionString.Default);
            
            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;
            string stateFilter = "AZ";

            try
            {
                Console.WriteLine("Fetching Contact records!");
                do
                {
                    var contacts = await _virtuousService.GetContactsAsync(skip, take, stateFilter:stateFilter);
                    skip += take;
                    if (contacts.Any())
                    {
                        Console.WriteLine($"Updating {contacts.Count} Conacts");
                        _contactDAL.UpdateAbbreviatedContactAsync(contacts);
                    }
                    hasMore = skip > maxContacts;
                }
                while (!hasMore);
                Console.WriteLine("Completed Fetching Contact records!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
