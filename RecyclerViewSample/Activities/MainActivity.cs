using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using StarWars.Api.Repository;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using RecyclerViewSample.Activities;
using System.Threading.Tasks;
using RecyclerViewSample.ORM;
using SQLite;
using Android.Content.PM;
using Android.Graphics;
using System;

namespace RecyclerViewSample
{
    [Activity(Label = "RecyclerViewSample", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity
    {
        //INDICATOR TO DETECT IF THE USER HAS BEEN LOGINED
        public static bool isLogined;

        private Android.Support.V4.App.ActionBarDrawerToggle mDrawerToggle;
        private ArrayAdapter mLeftAdapter;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private SupportToolbar mToolBar;
        private Android.App.FragmentManager fragmentManager;
        private Fragments.LoginOrRegistrationFragment loginOrRegFragment;
        private Fragments.SearchFragment searchFragment;
        private Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment loadingMapFragment;
        //we need this to change the background if there are no tours in MovieAdapter.cs
        public static LinearLayout mainLayout;
        DBRepository dbr = new DBRepository();

        //LISTVIEW
        List<Resources.LeftMenuFolder.LeftMenu> leftDrawerList = new List<Resources.LeftMenuFolder.LeftMenu>();
        //List<clases.citiesClass> citiesList = new List<clases.citiesClass>();
        protected override int LayoutResource
        {
            get { return Resource.Layout.main; }
        }

        private RecyclerView recyclerView;
        private ProgressBar activityIndicator;
        private RecyclerView.LayoutManager layoutManager;



        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //setToursTitle();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("fonts/HelveticaNeueLight")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());
            //declaring mainLayout
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            TextView experienceTitleTV = FindViewById<TextView>(Resource.Id.experienceTitleTV);

            activityIndicator.Visibility = Android.Views.ViewStates.Visible;

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            recyclerView.SetLayoutManager(layoutManager);

            Fragments.SearchFragment.searchByWordIndicator = false;

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            experienceTitleTV.Typeface = tf;

            var repository = new MoviesRepository();

            //here we create DB
            dbr.CreateDB();
            //here we create table
            dbr.CreateUsersTable();

            //declaring path for RETRIEVING DATA
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var user_table = db.Table<ORM.UsersDataTable>();

            isLogined = false;
            //clearing table
            foreach (var item in user_table)
            {
                isLogined = true;

                Login.name = item.name;
                Login.email_ = item.email;
                Login.avatar = item.avatar;
                Login.token = item.api_token;
                Login.birth_date = item.birth_date;
                Login.gender = item.gender;
                Login.phone_num = item.phone_num;
                Login.interests = item.biography;
                Login.user_id = item.user_id;
                Login.user_country_id = item.country_id;
                Login.password = item.password;
            }

            //left drawer
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(mToolBar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("Profile"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("Experiences"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("Map"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("Wishlist"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("Shopping Cart"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("My Experiences"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("Help & Contact"));
            leftDrawerList.Add(new Resources.LeftMenuFolder.LeftMenu("About"));

            ListView leftDrawerLV = FindViewById<ListView>(Resource.Id.left_drawer);
            leftDrawerLV.Adapter = new Resources.LeftMenuFolder.LeftMenuAdapter(this, leftDrawerList);

            leftDrawerLV.ItemClick += LeftDrawerLV_ItemClick;

            //button to open/close Left Drawer
            FindViewById<Button>(Resource.Id.leftDrawerBN).Click += delegate
            {
                if (mDrawerLayout.IsDrawerOpen(mLeftDrawer))
                {
                    mDrawerLayout.CloseDrawer(mLeftDrawer);
                }
                else
                {
                    mDrawerLayout.OpenDrawer(mLeftDrawer);
                }
            };
            //left drawer ENDED

            //button to show context menu
            FindViewById<Button>(Resource.Id.contextMenuBn).Click += MainActivity_Click;
            //button to show context menu ENDED

            var films = await repository.GetAllFilms(GettingJSON.content);

            //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE 
            List<StarWars.Api.Repository.Movie> tmpReverseList = new List<StarWars.Api.Repository.Movie>();
            for (int i = films.results.Count - 1; i >= 0; i--)
            {
                tmpReverseList.Add(new StarWars.Api.Repository.Movie
                {
                    id = films.results[i].id,
                    title = films.results[i].title,
                    description = films.results[i].description,
                    owner_id = films.results[i].owner_id,
                    price = films.results[i].price,
                    min_capacity = films.results[i].min_capacity,
                    max_capacity = films.results[i].max_capacity,
                    location = films.results[i].location,
                    created_at = films.results[i].created_at,
                    updated_at = films.results[i].updated_at,
                    price_rate = films.results[i].price_rate,
                    duration = films.results[i].duration,
                    duration_type = films.results[i].duration_type,
                    video_url = films.results[i].video_url,
                    alien_video_id = films.results[i].alien_video_id,
                    video_source = films.results[i].video_source,
                    has_cover = films.results[i].has_cover,
                    status = films.results[i].status,
                    publish_date = films.results[i].publish_date,
                    meet_place_address = films.results[i].meet_place_address,
                    meet_place_city = films.results[i].meet_place_city,
                    meet_place_country = films.results[i].meet_place_country,
                    nearby_landmarks = films.results[i].nearby_landmarks,
                    must_have = films.results[i].must_have,
                    instructions = films.results[i].instructions,
                    approved = films.results[i].approved,
                    approved_by = films.results[i].approved_by,
                    approve_date = films.results[i].approve_date,
                    lat = films.results[i].lat,
                    lng = films.results[i].lng,
                    cover_image = films.results[i].cover_image
                });
            }
            var moviesAdapter = new MovieAdapter(tmpReverseList, this);
            //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE ENDED

            if (films.results == null || films.results.Count == 0)
            {
                mainLayout.SetBackgroundResource(Resource.Drawable.NoTours);
            }
            else
            {
                recyclerView.SetAdapter(moviesAdapter);
            }

            activityIndicator.Visibility = Android.Views.ViewStates.Gone;
            //FindViewById<LinearLayout>(Resource.Id.mainLayout).SetBackgroundResource(Resource.Drawable.NoTours);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            //SupportActionBar.SetHomeButtonEnabled(false);

            fragmentManager = this.FragmentManager;
            loginOrRegFragment = new Fragments.LoginOrRegistrationFragment();
            searchFragment = new Fragments.SearchFragment();
            loadingMapFragment = new Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment();

            FindViewById<Button>(Resource.Id.searchBn).Click += delegate
            {
                searchFragment.Show(fragmentManager, "fragmentManager");
            };
        }


        private void MainActivity_Click(object sender, System.EventArgs e)
        {
            Android.Support.V7.Widget.PopupMenu popupMenu = new Android.Support.V7.Widget.PopupMenu(this, FindViewById<Button>(Resource.Id.contextMenuBn));

            popupMenu.Inflate(Resource.Menu.home);

            popupMenu.MenuItemClick += (s1, arg1) =>
            {
                if (arg1.Item.TitleFormatted.ToString() == "Change Destination")
                {
                    StartActivity(new Intent(this, typeof(ChangeDestination)));
                }
                if (arg1.Item.TitleFormatted.ToString() == "Filter")
                {
                    StartActivity(new Intent(this, typeof(FilterActivity)));
                }
                if (arg1.Item.TitleFormatted.ToString() == "Search")
                {
                    Toast.MakeText(this, "Search", ToastLength.Short).Show();
                    StartActivity(new Intent(this, typeof(FilterActivity)));
                }
                if (arg1.Item.TitleFormatted.ToString() == "Add Experience (for pals)")
                {
                    RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = null;
                    RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = null;
                    if (isLogined == false)
                    {
                        loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                    }
                    else
                    {
                        StartActivity(new Intent(this, typeof(RecyclerViewSampl.AddNewTourActivity)));
                    }
                }
            };
            try
            {
                popupMenu.Show();
            }
            catch
            {

            }
        }

        //item click handler of the left drawer
        private void LeftDrawerLV_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == 0)
            {
                if (isLogined == false)
                {
                    loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                }
                else
                {
                    StartActivity(typeof(Profile));
                }

            }
            if (e.Position == 1)
            {
                StartActivity(typeof(MainActivity));

            }
            if (e.Position == 2)
            {
                if (String.IsNullOrWhiteSpace(Login.token))
                {
                    StartActivity(typeof(Map));
                }
                else
                {
                    loadingMapFragment.Show(fragmentManager, "fragmentManager");
                }
            }
            if (e.Position == 3)
            {
                if (isLogined == false)
                {
                    loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                }
                else
                {
                    StartActivity(typeof(Activities.Wishlist));
                }
            }
            if (e.Position == 4)
            {
                if (isLogined == false)
                {
                    loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                }
                else
                {
                    StartActivity(typeof(Activities.ShoppingCart));
                }
            }
            if (e.Position == 5)
            {
                if (isLogined == false)
                {
                    loginOrRegFragment.Show(fragmentManager, "fragmentManager");
                }
                else
                {
                    StartActivity(typeof(Activities.MyBookings));
                }
            }
            if (e.Position == 6)
            {
                StartActivity(typeof(Help));
            }
            if (e.Position == 7)
            {
                StartActivity(typeof(AboutActivity));
            }
        }
        //item click handler of the left drawer ENDED

        /*public override bool OnCreateOptionsMenu(IMenu menu)
        {


            MenuInflater.Inflate(Resource.Menu.home, menu);
            return base.OnCreateOptionsMenu(menu);

        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            Change_Location();
            return base.OnOptionsItemSelected(item);
        }

        private void Change_Location()
        {
            StartActivity(typeof(FilterActivity));

        }

        private void Filter()
        {
            StartActivity(typeof(FilterActivity));
        }*/



        /*public override bool OnOptionsItemSelected(IMenuItem item)
        {
            mDrawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }*/

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }

    }
}

