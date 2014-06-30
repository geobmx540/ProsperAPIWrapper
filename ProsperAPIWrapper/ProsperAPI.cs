using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProsperAPIWrapper;

namespace ProsperAPI
{
    public class ProsperApi
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _apiBaseUrl = "https://api.prosper.com/v1/";
        private readonly AuthenticationHeaderValue _authenticationHeader;

        #region Constructors
        
        public ProsperApi(string username, string password)
        {
            _username = username;
            _password = password;
            _authenticationHeader = 
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(
                            string.Format("{0}:{1}", _username, _password))));
        }

        public ProsperApi(string username, string password, string baseUrl) : this(username, password)
        {
            _apiBaseUrl = baseUrl;
        }

        #endregion

        public async Task<bool> Authenticate()
        {
            if (String.IsNullOrEmpty(_username))
                throw new ArgumentNullException("_username", "Credentials are not set");
            if (String.IsNullOrEmpty(_username))
                throw new ArgumentNullException("_password", "Credentials are not set");

            try
            {
                // The account call will fail out if credentials are incorrect, thus
                // we won't spend time getting Notes. If Account information is right,
                // then we load the notes data at the same time, so we can use it 
                await GetAccount().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Get all notes for the current account
        /// Assumes credentials are valid
        /// </summary>
        /// <returns></returns>
        public async Task<List<Note>> GetNotes()
        {
            return await Get<List<Note>>("notes/").ConfigureAwait(false);
        }

        /// <summary>
        /// Get Account for the current credentials
        /// Assumes credentials are valid
        /// </summary>
        /// <returns></returns>
        public async Task<Account> GetAccount()
        {
            return await Get<Account>("account/").ConfigureAwait(false);
        }

        public async Task<List<Listing>> GetListings()
        {
            return await Get<List<Listing>>("Listings/").ConfigureAwait(false);
        }

        public async Task<List<Investment>> GetPendingInvestments()
        {
            return await Get<List<Investment>>("Investments/$filter=ListingStatus eq 2").ConfigureAwait(false);
        }

        public async Task<InvestResponse> Invest(string listingId, string amount)
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
                    throw new CommunicationException();

                var obj = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<InvestResponse>(obj);
            }
        }

        private async Task<T> Get<T>(string url)
        {
            using (var client = HttpClientSetup())
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new CommunicationException();

                var obj = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(obj);
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
