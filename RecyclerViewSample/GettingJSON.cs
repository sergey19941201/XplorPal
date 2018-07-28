using Newtonsoft.Json.Linq;
using RecyclerViewSample;
using RestSharp;
using System.Threading.Tasks;

namespace StarWars.Api.Repository
{
    public class GettingJSON
    {
        public static string content;
        public async Task<string> VictorSologoob(string token, string lat, string lon)
        {
            if (lat.Contains(","))
            {
                lat = lat.Replace(',', '.');
            }
            if (lon.Contains(","))
            {
                lon = lon.Replace(',', '.');
            }
            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/near_me", Method.POST);
            request.AddQueryParameter("api_token", token);
            request.AddQueryParameter("lat", lat.ToString());
            request.AddQueryParameter("lng", lon.ToString());

            var response = await client.ExecuteTaskAsync(request);
            content = response.Content;

            return content;
        }
        public async Task<string> VictorSologoob(string lat, string lon)
        {
            if (lat.Contains(","))
            {
                lat = lat.Replace(',', '.');
            }
            if (lon.Contains(","))
            {
                lon = lon.Replace(',', '.');
            }
            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/near_me", Method.POST);
            request.AddQueryParameter("lat", lat.ToString());
            request.AddQueryParameter("lng", lon.ToString());

            var response = await client.ExecuteTaskAsync(request);
            content = response.Content;

            return content;
        }
    }
}
