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
using Newtonsoft.Json;
using RestSharp;
using Newtonsoft.Json.Linq;
using Android.Content.PM;
using System.Text.RegularExpressions;

namespace RecyclerViewSampl
{
    [Activity(Label = "AddNewTourActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddNewTourActivity : Activity
    {
        //RecyclerViewSample.GettingCountry gettingCountry = new RecyclerViewSample.GettingCountry();
        private static int  IdOfChosenCategory;
        private static string status, promptMessage;
        private Android.App.FragmentManager fragmentManager;
        // RecyclerViewSample.GettingCategories gc = new RecyclerViewSample.GettingCategories();
        private RecyclerViewSample.Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment_EditTour loadingMapFragment;
        //private variables to store values from entries
        private static string _title, _location, _description, _pricePerPerson, _minPeople, _maxPeople, _duration, _city, _address;
        private static int _country, _category;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(RecyclerViewSample.Resource.Layout.AddNewTour);

            //RecyclerViewSample.GettingCategories gc = new RecyclerViewSample.GettingCategories();

            //clearing listCategoriesRoot 
            /*RecyclerViewSample.GettingCategories.categoriesList.Clear();
            //getting categories method
            gc.GetCategories();*/
            ProgressBar activityIndicator = FindViewById<ProgressBar>(RecyclerViewSample.Resource.Id.activityIndicator);
            Button add_tour_button = FindViewById<Button>(RecyclerViewSample.Resource.Id.add_tour_button);
            Spinner categories_spinner = FindViewById<Spinner>(RecyclerViewSample.Resource.Id.categories_spinner);
            //adapter
            var adapter = ArrayAdapter.CreateFromResource(this, RecyclerViewSample.Resource.Array.planets_array, Android.Resource.Layout.SimpleListItem1);
            //ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, RecyclerViewSample.GettingCategories.categoriesList);
            categories_spinner.Adapter = adapter;
            categories_spinner.ItemSelected += Categories_spinner_ItemSelected;
            //FindViewById<EditText>(Resource.Id.owner_id).Text = Login.user_id.ToString();

            var title = FindViewById<EditText>(RecyclerViewSample.Resource.Id.title);
            //var image = FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
            var location = FindViewById<EditText>(RecyclerViewSample.Resource.Id.location);
            var description = FindViewById<EditText>(RecyclerViewSample.Resource.Id.description);
            var price = FindViewById<EditText>(RecyclerViewSample.Resource.Id.price);
            var min_capacity = FindViewById<EditText>(RecyclerViewSample.Resource.Id.min_capacity);
            var max_capacity = FindViewById<EditText>(RecyclerViewSample.Resource.Id.max_capacity);
            var duration = FindViewById<EditText>(RecyclerViewSample.Resource.Id.duration);
            var meet_place_address = FindViewById<EditText>(RecyclerViewSample.Resource.Id.meet_place_address);
            var meet_place_city = FindViewById<EditText>(RecyclerViewSample.Resource.Id.meet_place_city);
            var chooseLocationMapBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.chooseLocationMapBn);
            /*var lat = FindViewById<EditText>(RecyclerViewSample.Resource.Id.lat);
            var lng = FindViewById<EditText>(RecyclerViewSample.Resource.Id.lng);*/

            if (IdOfChosenCategory < 3)
            {
                categories_spinner.SetSelection(IdOfChosenCategory);
            }
            else if (IdOfChosenCategory > 2)
            {
                categories_spinner.SetSelection(IdOfChosenCategory - 1);
            }

            if (_title != null)
            {
                title.Text = _title;
            }
            if (_location != null)
            {
                location.Text = _location;
            }
            if (_description != null)
            {
                description.Text = _description;
            }
            if (_pricePerPerson != null)
            {
                price.Text = _pricePerPerson;
            }
            if (_minPeople != null)
            {
                min_capacity.Text = _minPeople;
            }
            if (_maxPeople != null)
            {
                max_capacity.Text = _maxPeople;
            }
            if (_duration != null)
            {
                duration.Text = _duration;
            }
            if (_city != null)
            {
                meet_place_city.Text = _city;
            }
            if (_address != null)
            {
                meet_place_address.Text = _address;
            }

            /*try
            {
                categories_spinner.SetSelection(_category - 1);
            }
            catch { }*/
            /*try
            {
                countries.SetSelection(_country - 1);
            }
            catch { }*/

            //uploading image
            //image.Click += Image_Click;
            //uploading image ENDED
            fragmentManager = this.FragmentManager;
            loadingMapFragment = new RecyclerViewSample.Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment_EditTour();
            chooseLocationMapBn.Click += delegate
            {
                _title = title.Text;
                _location = location.Text;
                _description = description.Text;
                _pricePerPerson = price.Text;
                _minPeople = min_capacity.Text;
                _maxPeople = max_capacity.Text;
                _duration = duration.Text;
                _address = meet_place_address.Text;
                _city = meet_place_city.Text;
                _country = RecyclerViewSample.Login.user_country_id;
                _category = IdOfChosenCategory;
                RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.addOrEditTourIndicator = "add";
                RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = null;
                RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = null;
                loadingMapFragment.Show(fragmentManager, "fragmentManager");//StartActivity(typeof(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity));
            };

            add_tour_button.Click += async delegate
            {
                //setting prompt message empty
                promptMessage = "";
                var client = new RestClient("http://api.xplorpal.com");
                var request = new RestRequest("/experience", Method.POST);

                //CHECKING THE CORRECTNESS OF THE USER'S INTRODUCTION TO ALL FIELDS
                if (title.Text.Length < 3)
                {
                    promptMessage += " Title must have at least 3 symbols.\n";
                }
                if (location.Text.Length < 3)
                {
                    promptMessage += " Location must have at least 3 symbols.\n";
                }
                if (description.Text.Length < 30)
                {
                    promptMessage += " Description length must be at least 30 symbols.\n";
                }

                //checking if price, capacity, etc... are integer values
                int res;
                bool priceIsInt = false;
                priceIsInt = Int32.TryParse(price.Text, out res);

                if (priceIsInt == false)
                {
                    promptMessage += " Price must be an integer value.\n";
                }

                bool minCapacityIsInt = false;
                minCapacityIsInt = Int32.TryParse(min_capacity.Text, out res);
                if (minCapacityIsInt == false)
                {
                    promptMessage += " Minimum capacity must be an integer value.\n";
                }

                bool maxCapacityIsInt = false;
                maxCapacityIsInt = Int32.TryParse(max_capacity.Text, out res);
                if (maxCapacityIsInt == false)
                {
                    promptMessage += " Maximum capacity must be an integer value.\n";
                }

                bool durationIsInt = false;
                durationIsInt = Int32.TryParse(duration.Text, out res);
                if (durationIsInt == false)
                {
                    promptMessage += " Duration must be an integer value.\n";
                }
                //checking if price, capacity, etc... are integer values ENDED

                if (meet_place_address.Text.Length < 3)
                {
                    promptMessage += " Address of meeting place must have at least 3 symbols.\n";
                }

                if (meet_place_city.Text.Length < 3)
                {
                    promptMessage += " City of meeting place must have at least 3 symbols.\n";
                }

                if (String.IsNullOrWhiteSpace(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp))
                {
                    promptMessage += " Latitude is empty.\n";
                }
                if (String.IsNullOrWhiteSpace(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp))
                {
                    promptMessage += " Longitude is empty.\n";
                }
                /*string latFiltered = "", lngFiltered = "";
                /*foreach (char c in lat.Text)
                {
                    if (Char.IsNumber(c) || c == '+' || c == '-' || c == ',' || c == '.')
                    {
                        latFiltered += c;
                    }
                }
                foreach (char c in lng.Text)
                {
                    if (Char.IsNumber(c) || c == '+' || c == '-' || c == ',' || c == '.')
                    {
                        lngFiltered += c;
                    }
                }*/

                request.AddParameter("api_token", RecyclerViewSample.Login.token);
                request.AddParameter("title", title.Text);
                request.AddParameter("location", location.Text);
                request.AddParameter("description", description.Text);
                request.AddParameter("price", price.Text);
                request.AddParameter("owner_id", RecyclerViewSample.Login.user_id);
                request.AddParameter("min_capacity", min_capacity.Text);
                request.AddParameter("max_capacity", max_capacity.Text);
                request.AddParameter("duration", duration.Text);
                request.AddParameter("duration_type", 1);
                request.AddParameter("meet_place_address", meet_place_address.Text);
                request.AddParameter("meet_place_city", meet_place_city.Text);
                request.AddParameter("meet_place_country", RecyclerViewSample.Login.user_country_id);
                request.AddParameter("category_list[0]", IdOfChosenCategory);
                if (!String.IsNullOrWhiteSpace(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp) 
                && !String.IsNullOrWhiteSpace(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp))
                {
                    if (RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp.Contains(","))
                    {
                        request.AddParameter("lat", RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp.Replace(",", "."));
                    }
                    else
                    {
                        request.AddParameter("lat", RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp);
                    }
                    if (RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp.Contains(","))
                    {
                        request.AddParameter("lng", RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp.Replace(",", "."));
                    }
                    else
                    {
                        request.AddParameter("lng", RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp);
                    }
                }
                if (promptMessage == "")
                {
                    try
                    {
                        add_tour_button.Visibility = ViewStates.Gone;
                        chooseLocationMapBn.Visibility = ViewStates.Gone;
                        activityIndicator.Visibility = ViewStates.Visible;
                        IRestResponse response = await client.ExecuteTaskAsync(request);
                        activityIndicator.Visibility = ViewStates.Gone;
                        add_tour_button.Visibility = ViewStates.Visible;
                        chooseLocationMapBn.Visibility = ViewStates.Visible;
                        var content = response.Content;
                        var myContent = JObject.Parse(content);
                        status = myContent["status"].ToString();
                    }
                    catch { }

                    if (status == "success")
                    {
                        Toast.MakeText(this, "Tour added successfully", ToastLength.Short).Show();
                        //setting status variable to null to prevent issues with adding new places in future
                        status = null;
                        //setting variables to null
                        _title = null; _location = null; _description = null; _pricePerPerson = null;
                        _minPeople = null; _maxPeople = null; _duration = null; _city = null; _address = null;
                        //int variables to default
                        _category = 0; _country = 0;
                        IdOfChosenCategory = 0;
                        StartActivity(typeof(RecyclerViewSample.MainActivity));
                    }
                }
                else
                {
                    Toast.MakeText(this, promptMessage, ToastLength.Long).Show();
                }
            };
        }

        /*private void Image_Click(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }*/

        private void Categories_spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Choose type of experience")
            {
                IdOfChosenCategory = 0;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Sports")
            {
                IdOfChosenCategory = 1;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Music")
            {
                IdOfChosenCategory = 3;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Romance")
            {
                IdOfChosenCategory = 4;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Adventure")
            {
                IdOfChosenCategory = 5;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Food")
            {
                IdOfChosenCategory = 6;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Art/Architecture")
            {
                IdOfChosenCategory = 7;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Photography")
            {
                IdOfChosenCategory = 8;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Bar hopping")
            {
                IdOfChosenCategory = 9;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Sight-seeing")
            {
                IdOfChosenCategory = 10;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Wine tasting")
            {
                IdOfChosenCategory = 11;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Classes")
            {
                IdOfChosenCategory = 12;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Wedding planning")
            {
                IdOfChosenCategory = 13;
            }
            if ((spinner.GetItemAtPosition(e.Position)).ToString() == "Gardening")
            {
                IdOfChosenCategory = 14;
            }
        }


        /*protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                var imageView = FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                imageView.SetImageURI(data.Data);
            }
        }*/
    }
}