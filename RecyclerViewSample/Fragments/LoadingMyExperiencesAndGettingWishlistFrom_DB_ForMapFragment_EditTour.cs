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
using Android.Graphics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;
using System.IO;
namespace RecyclerViewSample.Fragments
{
    class LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment_EditTour : DialogFragment
    {
        //Database declaration
        ORM.DBRepository dbr = new ORM.DBRepository();
        GetMyExperiences getMyExperiences = new GetMyExperiences();
        ProgressBar activityIndicator;
        TextView Getting_your_exp_TV;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment, container, false);

            Getting_your_exp_TV = rootView.FindViewById<TextView>(Resource.Id.Getting_your_exp_TV);
            activityIndicator = rootView.FindViewById<ProgressBar>(Resource.Id.activityIndicator);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(rootView.Context.Assets, path);
            Getting_your_exp_TV.Typeface = tf;

            activityIndicator.Visibility = ViewStates.Visible;

            //clearing lists
            RecyclerViewSample.Activities.Wishlist.wishlistClassForRecycler.Clear();
            RecyclerViewSample.Activities.MyBookings.myExpListClassForRecycler.Clear();

            addingToWishListFromDB_for_map();
            return rootView;
        }
        private async Task<string> addingToWishListFromDB_for_map()
        {
            //declaring path for RETRIEVING DATA
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var wishlist_table = db.Table<ORM.Wishlist>();
            //creating wishlist table
            dbr.CreateWishlistTable();
            //checking if the place of interest exists
            foreach (var item in wishlist_table)
            {
                if (!String.IsNullOrWhiteSpace(item.lat) && !String.IsNullOrWhiteSpace(item.lng))
                {
                    RecyclerViewSample.Activities.Wishlist.wishlistClassForRecycler.Add(new WishlistClassForRecycler
                    {
                        _name = item.name,
                        _id_public = item.id_public,
                        _price = item.price,
                        picData = item.picData,
                        _description = item.description,
                        _location = item.location,
                        _duration = item.duration,
                        _min_capacity = item.min_capacity,
                        _max_capacity = item.max_capacity,
                        _lat = item.lat,
                        _lng = item.lng
                    });
                }
            }
            await addingMyExperiencesToListForMap();
            return "";
        }

        private async Task<string> addingMyExperiencesToListForMap()
        {
            await getMyExperiences.GettingMyExperiences(Login.token);
            try
            {
                var responseData = JsonConvert.DeserializeObject<RootObjectMyExperiences>(GetMyExperiences.content);

                foreach (var item in responseData.experiences)
                {
                    if (!String.IsNullOrWhiteSpace(item.lat) && !String.IsNullOrWhiteSpace(item.lng))
                    {
                        RecyclerViewSample.Activities.MyBookings.myExpListClassForRecycler.Add(
                        new RecyclerViewSample.Activities.MyExperiencesClassForRecycler
                        {
                            _id = item.id.ToString(),
                            _name = item.title,
                            _price = item.price,
                            _description = item.description,
                            _location = item.location,
                            _duration = item.duration,
                            _min_capacity = item.min_capacity,
                            _max_capacity = item.max_capacity,
                            _lat = item.lat,
                            _lng = item.lng
                        });
                    }
                }

            }
            catch { }
            activityIndicator.Visibility = ViewStates.Gone;
            Getting_your_exp_TV.Visibility = ViewStates.Gone;
            this.Dismiss();
            StartActivity(new Intent(this.Activity, typeof(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity)));
            return "";
        }
    }
}