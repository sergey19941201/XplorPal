using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;

namespace StarWars.Api.Repository
{
    public class MoviesRepository
    {
        public async Task<Films> GetAllFilms(string data)
        {
            /*var httpClient = GetHttpClient();

            var response = await httpClient.GetAsync(ServiceEndPoints.StartWarsApiBaseUri).ConfigureAwait(false);*/

            if (data != null)
            {
                //var content = data;

                string jsonString = data;//await content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<Films>(jsonString);
            }
            return new Films();
        }

        /*private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient(new NativeMessageHandler())
            {
                BaseAddress = new Uri(ServiceEndPoints.StartWarsApiBaseUri)
            };

            

            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }*/
    }


    /*public class ServiceEndPoints
    {
        
        
        public static readonly string StartWarsApiBaseUri = "http://api.xplorpal.com/";

    }*/
}
