using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProsperAPIWrapper
{
    public class ProsperApi
    {
        private readonly string _apiBaseUrl;
        private readonly AuthenticationHeaderValue _authenticationHeader;
        private const string ProductionUrl = "https://api.prosper.com/v1/";

        /// <summary>
        /// Create a new API Wrapper with the given username and password
        /// </summary>
        /// <param name="username">Prosper API Username</param>
        /// <param name="password">Prosper API Password</param>
        public ProsperApi(string username, string password) :
            this(username, password, ProductionUrl)
        {
        }

        /// <summary>
        /// Create a new API Wrapper with the given username, password and base URL.
        /// Mainly provided for testing purposes
        /// </summary>
        /// <param name="username">Prosper API Username</param>
        /// <param name="password">Prosper API Password</param>
        /// <param name="baseUrl">Base Url of Prosper API (e.g. https://api.prosper.com/v1/) </param>
        public ProsperApi(string username, string password, string baseUrl)
        {
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrEmpty(password))
                throw new ArgumentException("Username or Password cannot be null or empty");
            
            _apiBaseUrl = baseUrl;
        
            _authenticationHeader =
             new AuthenticationHeaderValue(
                 "Basic",
                 Convert.ToBase64String(
                     Encoding.UTF8.GetBytes(
                         string.Format("{0}:{1}", username, password))));
        }

        /// <summary>
        /// Authenticates the credentials
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                // The account call will fail out if credentials are incorrect
                await GetAccountAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a list of all notes associated with the account
        /// </summary>
        public Task<List<Note>> GetNotesAsync()
        {
            return GetProsperObjectAsync<List<Note>>("notes/");
        }

        /// <summary>
        /// Returns the account summary
        /// </summary>
        public Task<Account> GetAccountAsync()
        {
            return GetProsperObjectAsync<Account>("account/");
        }

        /// <summary>
        /// Returns current active listings
        /// </summary>
        public Task<List<Listing>> GetListingsAsync()
        {
            return GetProsperObjectAsync<List<Listing>>("Listings/");
        }

        /// <summary>
        /// Returns all pending investments (used to avoid double investing in the same loan)
        /// </summary>
        public Task<List<Investment>> GetPendingInvestmentsAsync()
        {
            return GetProsperObjectAsync<List<Investment>>("Investments?$filter=ListingStatus eq 2");
        }

        /// <summary>
        /// Return Prosper Object T from the URL
        /// Base for the other GetXXXAsync Methods, use this for custom ODATA Urls
        /// </summary>
        /// <typeparam name="T">Prosper Object Type to return</typeparam>
        /// <param name="url">API endpoint url - can handle ODATA Parameters</param>
        public async Task<T> GetProsperObjectAsync<T>(string url)
        {
            using (var client = HttpClientSetup())
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(response.StatusCode.ToString());

                var obj = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(obj);
            }
        }

        /// <summary>
        /// Invest [amount] in [listingId]
        /// </summary>
        /// <param name="listingId">ID of the Listing</param>
        /// <param name="amount">Amount to invest</param>
        /// <returns></returns>
        public async Task<InvestResponse> InvestAsync(string listingId, string amount)
        {
            using (var client = HttpClientSetup())
            {
                var investment = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("listingId", listingId),
                    new KeyValuePair<string, string>("amount", amount)
                };

                var content = new FormUrlEncodedContent(investment);
                
                var response = await client.PostAsync("Invest/", content).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(response.StatusCode.ToString());

                var obj = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<InvestResponse>(obj);
            }
        }

        private HttpClient HttpClientSetup()
        {
            var client = new HttpClient {BaseAddress = new Uri(_apiBaseUrl)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = _authenticationHeader;

            return client;
        }
    }
}
