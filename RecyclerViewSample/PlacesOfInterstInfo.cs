using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StarWars.Api.Repository;
using Newtonsoft.Json;
using RecyclerViewSample.ORM;
using System.IO;
using SQLite;
using Environment = System.Environment;

namespace RecyclerViewSample
{
    public class PlacesOfInterstInfo
    {
        //Database declaration
        DBRepository dbr = new DBRepository();
        private static int count_data_rows_in_poi_table;
        MoviesRepository repository = new MoviesRepository();
        public async void places_of_interest()
        {
            //creating table of places of interest
            dbr.CreatePlacesOfInterestTable();
            //declaring path for RETRIEVING DATA
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var places_of_interest_table = db.Table<PlacesOfInterestTable>();

            var places_of_interest = await repository.GetAllFilms(GettingJSON.content);

            //We need this count to know if our table is empty
            count_data_rows_in_poi_table = 0;
            foreach (var item in places_of_interest_table)
            {
                count_data_rows_in_poi_table = 1;
                break;
            }
            foreach (var place_of_interest in places_of_interest.results)
            {
                if (count_data_rows_in_poi_table == 0)
                {
                    dbr.InsertPlacesOfInterestRecord(place_of_interest.title, place_of_interest.price, place_of_interest.lat, place_of_interest.lng);
                }
                foreach(var place_of_interst_internal in places_of_interest.results)
                {
                    if(place_of_interst_internal.title!= place_of_interest.title)
                    {
                        dbr.InsertPlacesOfInterestRecord(place_of_interest.title, place_of_interest.price, place_of_interest.lat, place_of_interest.lng);
                    }
                }
            }
        }
    }
}