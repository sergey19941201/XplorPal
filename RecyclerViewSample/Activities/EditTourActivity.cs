using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using Android.Graphics;
using System.IO;
using RestSharp;
using Newtonsoft.Json.Linq;
using Android.Util;
using Android.Content.PM;
using Com.Bumptech.Glide;

namespace RecyclerViewSampl
{
    [Activity(Label = "EditTourActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class EditTourActivity : Activity
    {
        private static string category;
        private static int idOfChosenCountry, IdOfChosenCategory;
        private static string response_code, response_img_code, promptMessage;
        RecyclerViewSample.GetMyExperiences getMyExperiences = new RecyclerViewSample.GetMyExperiences();
        RecyclerViewSample.GettingCountry gettingCountry = new RecyclerViewSample.GettingCountry();
        private ProgressBar activityIndicator;
        private Android.App.FragmentManager fragmentManager;
        // RecyclerViewSample.GettingCategories gc = new RecyclerViewSample.GettingCategories();
        private RecyclerViewSample.Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment_EditTour loadingMapFragment;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(RecyclerViewSample.Resource.Layout.Edit);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            /*//clearing listCategoriesRoot 
            RecyclerViewSample.GettingCategories.categoriesList.Clear();
            //getting categories method
            gc.GetCategories();*/
            var chooseLocationMapBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.chooseLocationMapBn);
            try
            {
                activityIndicator = FindViewById<ProgressBar>(RecyclerViewSample.Resource.Id.activityIndicator);
                var image = FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                var title = FindViewById<EditText>(RecyclerViewSample.Resource.Id.title);
                var location = FindViewById<EditText>(RecyclerViewSample.Resource.Id.location);
                var description = FindViewById<EditText>(RecyclerViewSample.Resource.Id.description);
                var price = FindViewById<EditText>(RecyclerViewSample.Resource.Id.price);
                var min_capacity = FindViewById<EditText>(RecyclerViewSample.Resource.Id.min_capacity);
                var max_capacity = FindViewById<EditText>(RecyclerViewSample.Resource.Id.max_capacity);
                var duration = FindViewById<EditText>(RecyclerViewSample.Resource.Id.duration);
                var meet_place_address = FindViewById<EditText>(RecyclerViewSample.Resource.Id.meet_place_address);
                var meet_place_city = FindViewById<EditText>(RecyclerViewSample.Resource.Id.meet_place_city);
                Spinner categories_spinner = FindViewById<Spinner>(RecyclerViewSample.Resource.Id.categories_spinner);
                var applyChangesBn = FindViewById<Button>(RecyclerViewSample.Resource.Id.applyChangesBn);

                title.Typeface = tf;
                location.Typeface = tf;
                description.Typeface = tf;
                price.Typeface = tf;
                min_capacity.Typeface = tf;
                max_capacity.Typeface = tf;
                duration.Typeface = tf;
                meet_place_address.Typeface = tf;
                meet_place_city.Typeface = tf;


                //adapter
                // ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, RecyclerViewSample.GettingCategories.categoriesList);
                var adapter = ArrayAdapter.CreateFromResource(this, RecyclerViewSample.Resource.Array.planets_array, Android.Resource.Layout.SimpleListItem1);
                categories_spinner.Adapter = adapter;
                categories_spinner.ItemSelected += Categories_spinner_ItemSelected;

                await getMyExperiences.GettingMyExperiences(RecyclerViewSample.Login.token);

                var responseData = JsonConvert.DeserializeObject<RecyclerViewSample.RootObjectMyExperiences>(RecyclerViewSample.GetMyExperiences.content);

                foreach (var item in responseData.experiences)
                {
                    if (item.id == Convert.ToInt32(RecyclerViewSample.Activities.MyExperiencesDetailActivity.id_of_my_current_experience))
                    {
                        //setting them to null to prevent situation that one tour will pick another tours coords
                        RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = null;
                        RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = null;
                        title.Text = item.title;
                        location.Text = item.location;
                        description.Text = item.description;
                        price.Text = item.price;
                        min_capacity.Text = item.min_capacity;
                        max_capacity.Text = item.max_capacity;
                        duration.Text = item.duration;
                        meet_place_address.Text = item.meet_place_address;
                        meet_place_city.Text = item.meet_place_city;
                        RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = item.lat;
                        RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = item.lng;
                        if (item.cover_image != null)
                        {
                            Glide.With(Application.Context)
                         .Load(item.cover_image.url)
                         .Into(image);
                        }
                        //category=item.
                        break;
                    }
                }


                //!!!!!CHECK HERE EVERYTHING!!!!
                //categories_spinner.SetSelection(0);
                image.Click += Image_Click;
                applyChangesBn.Click += async delegate
                {
                    if (String.IsNullOrWhiteSpace(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp) &&
                    String.IsNullOrWhiteSpace(RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp))
                    {
                        Toast.MakeText(this, "Choose location of the experience", ToastLength.Short).Show();
                    }
                    else
                    {
                        //setting prompt message empty
                        promptMessage = "";
                        //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE
                        image.BuildDrawingCache(true);
                        Bitmap bitmap = image.GetDrawingCache(true);
                        image.SetImageBitmap(bitmap);
                        Bitmap b = Bitmap.CreateBitmap(image.GetDrawingCache(true));
                        MemoryStream memStream = new MemoryStream();
                        b.Compress(Bitmap.CompressFormat.Png, 0, memStream);
                        //memStream.ToArray();
                        //WE NEED ALL OF THIS TO WRITE IMAGE TO DATABASE ENDED

                        byte[] ba = memStream.ToArray();
                        string bal = Base64.EncodeToString(ba, Base64.Default);
                        var client = new RestClient("http://api.xplorpal.com/experience/"
                            + RecyclerViewSample.Activities.MyExperiencesDetailActivity.id_of_my_current_experience);
                        var request = new RestRequest("/update", Method.POST);
                        var requestImage = new RestRequest("/uploadPhotos", Method.POST);

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

                        if (IdOfChosenCategory == 0)
                        {
                            promptMessage += " Choose category.\n";
                        }


                        //CHECKING THE CORRECTNESS OF THE USER'S INTRODUCTION TO ALL FIELDS ENDED

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
                        requestImage.AddParameter("api_token", RecyclerViewSample.Login.token);
                        requestImage.AddParameter("photos[]", "data:image/png;base64," + bal);   //"data:image/png;base64,/9j/"+
                        if (promptMessage == "")
                        {
                            try
                            {
                                activityIndicator.Visibility = Android.Views.ViewStates.Visible;
                                applyChangesBn.Visibility = Android.Views.ViewStates.Gone;
                                chooseLocationMapBn.Visibility = Android.Views.ViewStates.Gone;
                                IRestResponse response = await client.ExecuteTaskAsync(request);
                                IRestResponse responseImage = await client.ExecuteTaskAsync(requestImage);
                                var content = response.Content;
                                var contentImage = responseImage.Content;
                                var myContent = JObject.Parse(content);
                                response_code = myContent["response_code"].ToString();
                                var myContentImage = JObject.Parse(contentImage);
                                response_img_code = myContentImage["response_code"].ToString();
                                Console.WriteLine(response_img_code);
                                //now we need to get a new content to set a new cover image
                                if(response_img_code=="200")
                                {
                                    await getMyExperiences.GettingMyExperiences(RecyclerViewSample.Login.token);
                                    var responseDataSetCoverImage = JsonConvert.DeserializeObject<RecyclerViewSample.RootObjectMyExperiences>(RecyclerViewSample.GetMyExperiences.content);

                                    foreach (var item in responseDataSetCoverImage.experiences)
                                    {
                                            try
                                            {
                                                if (item.id.ToString() == RecyclerViewSample.Activities.MyExperiencesDetailActivity.id_of_my_current_experience)
                                                {

                                                    //!!!!!!!!!!!!!!!!!!NEED TO CHANGE THIS VALUE FOR THE LAST ADDED IMAGE_ID!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                    int image_id = item.cover_image.id;










                                                    var request_set_cover = new RestRequest("/setCover", Method.POST);
                                                    request_set_cover.AddQueryParameter("api_token", RecyclerViewSample.Login.token);
                                                    request_set_cover.AddQueryParameter("image_id", image_id.ToString());
                                                    var response_set_cover = await client.ExecuteTaskAsync(request_set_cover);
                                                    string set_cover_content, set_cover_response;
                                                    set_cover_content = response_set_cover.Content;
                                                    var set_c_content = JObject.Parse(set_cover_content);
                                                    set_cover_response = set_c_content["response_code"].ToString();
                                                }
                                            }catch{}
                                      }
                                }

                                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                                applyChangesBn.Visibility = Android.Views.ViewStates.Visible;
                                chooseLocationMapBn.Visibility = Android.Views.ViewStates.Visible;
                            }
                            catch { }
                            if (response_code == "200")
                            {
                                Toast.MakeText(this, "Tour changed successfully", ToastLength.Short).Show();
                                activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                                applyChangesBn.Visibility = Android.Views.ViewStates.Visible;
                                chooseLocationMapBn.Visibility = Android.Views.ViewStates.Visible;
                                RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = null;
                                RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = null;
                                StartActivity(typeof(RecyclerViewSample.MainActivity));
                            }
                        }
                        else if (promptMessage != "")
                        {
                            activityIndicator.Visibility = Android.Views.ViewStates.Gone;
                            applyChangesBn.Visibility = Android.Views.ViewStates.Visible;
                            chooseLocationMapBn.Visibility = Android.Views.ViewStates.Visible;
                            Toast.MakeText(this, promptMessage, ToastLength.Long).Show();
                        }
                    }
                };

            }
            catch { }

            fragmentManager = this.FragmentManager;
            loadingMapFragment = new RecyclerViewSample.Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment_EditTour();
            chooseLocationMapBn.Click += delegate
            {
                RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.addOrEditTourIndicator = "edit";
                loadingMapFragment.Show(fragmentManager, "fragmentManager");
            };
        }

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

        private void Image_Click(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                var imageView = FindViewById<ImageView>(RecyclerViewSample.Resource.Id.myImageView);
                //setting BackgroundResource to 0 to clear the previous image
                imageView.SetBackgroundResource(0);
                imageView.SetImageURI(data.Data);

                /*string path = data.Data.Path;
                //string name = data.Data.
                //media / external / images / media / 406
                //string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/filetemp.txt");
                var file = new FileInfo(path);// "/external/images/media/406").Length;
                //long length = file.Length;
                //storage/emulated/0/P70407-175308
                */

                //WE NEED ALL OF THIS TO CHECK SIZE OF IMAGEVIEW 
                imageView.BuildDrawingCache(true);
                Bitmap bitmap = imageView.GetDrawingCache(true);
                imageView.SetImageBitmap(bitmap);
                Bitmap b = Bitmap.CreateBitmap(imageView.GetDrawingCache(true));
                MemoryStream memStream = new MemoryStream();
                b.Compress(Bitmap.CompressFormat.Png, 0, memStream);
                //checking size of bytearray
                byte[] a = memStream.ToArray();
                long length = a.Length;
                double MB = ConvertBytesToMegabytes(length);
                //checking size of bytearrayENDED

                //checking size of base64format
                string bal = Base64.EncodeToString(a, Base64.Default);
                long LengthBase64 = bal.Length;
                double base64long = ConvertBytesToMegabytes(LengthBase64);
                //checking size of base64format ENDED
                //WE NEED ALL OF THIS TO CHECK SIZE OF IMAGEVIEW ENDED
            }
        }
        //function that converts bytes to megabytes
        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}