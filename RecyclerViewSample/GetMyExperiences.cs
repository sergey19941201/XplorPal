using RestSharp;
using System.Threading.Tasks;

namespace RecyclerViewSample
{
    public class GetMyExperiences
    {
        public static string content;
        public async Task<string> GettingMyExperiences(string token)
        {
            var client = new RestClient("http://api.xplorpal.com");
            var request = new RestRequest("/my_experiences", Method.POST);
            request.AddQueryParameter("api_token", token);

            var response = await client.ExecuteTaskAsync(request);
            content = response.Content;
            return content;//
        }
    }
}