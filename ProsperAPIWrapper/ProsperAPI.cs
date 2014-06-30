using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace ProsperAPIWrapper
{
    public class ProsperApi
    {
        private readonly string _apiBaseUrl;
        private readonly AuthenticationHeaderValue _authenticationHeader;

        public ProsperApi(string username, string password) :
            this(username, password, "https://api.prosper.com/v1/")
        {
        }

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

        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                // The account call will fail out if credentials are incorrect, thus
                // we won't spend time getting Notes. If Account information is right,
                // then we load the notes data at the same time, so we can use it 
                await GetAccountAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            return await GetProsperObjectAsync<List<Note>>("notes/").ConfigureAwait(false);
        }

        public async Task<Account> GetAccountAsync()
        {
            return await GetProsperObjectAsync<Account>("account/").ConfigureAwait(false);
        }

        public async Task<List<Listing>> GetListingsAsync()
        {
            return await GetProsperObjectAsync<List<Listing>>("Listings/").ConfigureAwait(false);
        }

        public async Task<List<Investment>> GetPendingInvestmentsAsync()
        {
            return await GetProsperObjectAsync<List<Investment>>("Investments/$filter=ListingStatus eq 2").ConfigureAwait(false);
        }

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

        private async Task<T> GetProsperObjectAsync<T>(string url)
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
