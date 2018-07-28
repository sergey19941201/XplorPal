using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;

namespace RecyclerViewSample
{
    public class GettingCountry
    {
        public static List<RootObjectCountries> listOfCountriesRoot = new List<RootObjectCountries>();
        public static List<string> countriesList = new List<string>();

        public class RootObjectCountries
        {
            [JsonProperty("id")]
            public int id { get; set; }

            [JsonProperty("nicename")]
            public string nicename { get; set; }

            public RootObjectCountries(int id, string nicename)
            {
                this.id = id;
                this.nicename = nicename;
            }
        }

        public int retrievingChoosenCountryId(string country)//used to retrieve id of the pressed country
        {
            foreach (var item in listOfCountriesRoot)//foreach loop to compare item title of the country with pressed title of the country
            {
                if (country == item.nicename)
                {
                    return item.id;//retrieving needed id of the country
                }
            };
            return 0;
        }
    }
}