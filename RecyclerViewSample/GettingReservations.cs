//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using RestSharp;

namespace RecyclerViewSample
{
    public class GettingReservations
    {
        public static string content;
        public async Task<string> GetReservations(string token)
        {
            var client = new RestClient("http://api.xplorpal.com/experience/" + Tours_detail.current_experience_id);
            var request = new RestRequest("/reservations_list", Method.POST);
            request.AddQueryParameter("api_token", token);

            var response = await client.ExecuteTaskAsync(request);
            content = response.Content;
            if(content=="")
            {
                await GetReservations(Login.token);
            }
            return content;
        }
    }
}