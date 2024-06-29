using RestSharp;
using Sync.ConfigurationOptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sync
{
    public interface IVirtuousService
    {
        Task<List<AbbreviatedContact>> GetContactsAsync(int skip, int take, string stateFilter = "");
    }

    /// <summary>
    /// API Docs found at https://docs.virtuoussoftware.com/
    /// </summary>
    internal class VirtuousService : IVirtuousService
    {
        private readonly RestClient _restClient;

        public VirtuousService(AuthorizationOption authorizationOption)
        {
            var options = new RestClientOptions(authorizationOption.VirtuousApiBaseUrl)
            {
                Authenticator = new RestSharp.Authenticators.OAuth2.OAuth2AuthorizationRequestHeaderAuthenticator(authorizationOption.VirtuousApiKey)
            };

            _restClient = new RestClient(options);
        }

        public async Task<List<AbbreviatedContact>> GetContactsAsync(int skip, int take, string stateFilter="")
        {
            List<AbbreviatedContact> resultContact = new List<AbbreviatedContact>();
            var request = new RestRequest("/api/Contact/Query", Method.Post);
            request.AddQueryParameter("Skip", skip);
            request.AddQueryParameter("Take", take);
            request.AddHeader("Accept", "application/json");

            var body = new ContactQueryRequest();
            request.AddJsonBody(body);

            var response = await _restClient.PostAsync<PagedResult<AbbreviatedContact>>(request);

            resultContact = response.List.Where(x => !string.IsNullOrWhiteSpace(x.Address) 
                                                && x.Address.Contains(stateFilter)).ToList();

            return resultContact;
        }
    }
}
