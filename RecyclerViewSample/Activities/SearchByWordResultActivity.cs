using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using UK.CO.Chrisjenx.Calligraphy;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using RecyclerViewSample.ORM;
using SQLite;
using Newtonsoft.Json;
using Android.Content.PM;
using Android.Graphics;
using System;

namespace RecyclerViewSample.Activities
{
    [Activity(Label = "SearchByWordResultActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SearchByWordResultActivity : BaseActivity
    {
        public static LinearLayout mainLayout;
        private Android.Support.V4.App.ActionBarDrawerToggle mDrawerToggle;
        private ArrayAdapter mLeftAdapter;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;
        private SupportToolbar mToolBar;
        private Android.App.FragmentManager fragmentManager;
        private Fragments.LoginOrRegistrationFragment loginOrRegFragment;
        private Fragments.SearchFragment searchFragment;
        private Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment loadingMapFragment;
        //public static List<Experience> experienceList;
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

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //declaring mainLayout
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                 .SetDefaultFontPath("fonts/HelveticaNeueLight")
                 .SetFontAttrId(Resource.Attribute.fontPath)
                 .Build());
            //declaring mainLayout
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            activityIndicator = FindViewById<ProgressBar>(Resource.Id.activityIndicator);
            TextView experienceTitleTV = FindViewById<TextView>(Resource.Id.experienceTitleTV);

            activityIndicator.Visibility = Android.Views.ViewStates.Gone;

            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            recyclerView.SetLayoutManager(layoutManager);

            string path = "fonts/HelveticaNeueLight.ttf";
            Typeface tf = Typeface.CreateFromAsset(Assets, path);
            experienceTitleTV.Typeface = tf;

            experienceTitleTV.Text = "Search results";

            //here we create DB
            dbr.CreateDB();
            //here we create table
            dbr.CreateUsersTable();

            //declaring path for RETRIEVING DATA
            string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            var db = new SQLiteConnection(dbPath);
            var user_table = db.Table<ORM.UsersDataTable>();

            MainActivity.isLogined = false;
            //clearing table
            foreach (var item in user_table)
            {
                MainActivity.isLogined = true;
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

            var responseSearch = JsonConvert.DeserializeObject<RootObjectSearchByWord>(Fragments.SearchFragment.content);

            //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE 
            List<Experience> tmpReverseList = new List<Experience>();
            for (int i = responseSearch.experiences.Count - 1; i >= 0; i--)
            {
                tmpReverseList.Add(new Experience
                {
                    id = responseSearch.experiences[i].id,
                    title = responseSearch.experiences[i].title,
                    description = responseSearch.experiences[i].description,
                    owner_id = responseSearch.experiences[i].owner_id,
                    price = responseSearch.experiences[i].price,
                    min_capacity = responseSearch.experiences[i].min_capacity,
                    max_capacity = responseSearch.experiences[i].max_capacity,
                    location = responseSearch.experiences[i].location,
                    created_at = responseSearch.experiences[i].created_at,
                    updated_at = responseSearch.experiences[i].updated_at,
                    price_rate = responseSearch.experiences[i].price_rate,
                    duration = responseSearch.experiences[i].duration,
                    duration_type = responseSearch.experiences[i].duration_type,
                    video_url = responseSearch.experiences[i].video_url,
                    alien_video_id = responseSearch.experiences[i].alien_video_id,
                    video_source = responseSearch.experiences[i].video_source,
                    has_cover = responseSearch.experiences[i].has_cover,
                    status = responseSearch.experiences[i].status,
                    publish_date = responseSearch.experiences[i].publish_date,
                    meet_place_address = responseSearch.experiences[i].meet_place_address,
                    meet_place_city = responseSearch.experiences[i].meet_place_city,
                    meet_place_country = responseSearch.experiences[i].meet_place_country,
                    nearby_landmarks = responseSearch.experiences[i].nearby_landmarks,
                    must_have = responseSearch.experiences[i].must_have,
                    instructions = responseSearch.experiences[i].instructions,
                    approved = responseSearch.experiences[i].approved,
                    approved_by = responseSearch.experiences[i].approved_by,
                    approve_date = responseSearch.experiences[i].approve_date,
                    lat = responseSearch.experiences[i].lat,
                    lng = responseSearch.experiences[i].lng,
                    cover_image = responseSearch.experiences[i].cover_image
                });
            }
            var searchAdapter = new SearchAdapter(tmpReverseList, this);
            //THIS CONSTRUCTION IS TO DISPLAY ITEMS FROM REVERSE ENDED

            recyclerView.SetAdapter(searchAdapter);

            fragmentManager = this.FragmentManager;
            loginOrRegFragment = new Fragments.LoginOrRegistrationFragment();
            searchFragment = new Fragments.SearchFragment();
            loadingMapFragment = new Fragments.LoadingMyExperiencesAndGettingWishlistFrom_DB_ForMapFragment();

            FindViewById<Button>(Resource.Id.searchBn).Click += delegate
            {
                searchFragment.Show(fragmentManager, "fragmentManager");
            };

            mainLayout.SetBackgroundResource(Resource.Drawable.NoTours);
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
                if (arg1.Item.TitleFormatted.ToString() == "Add Experience (for pals)")
                {
                    RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLatOfExp = null;
                    RecyclerViewSample.Activities.MapForChooseTourCoordsActivity.chosenLngOfExp = null;
                    if (MainActivity.isLogined == false)
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
                if (MainActivity.isLogined == false)
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
                if (MainActivity.isLogined == false)
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
                if (MainActivity.isLogined == false)
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
                if (MainActivity.isLogined == false)
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
                //Toast.MakeText(this, "You clicked My bookings", ToastLength.Short).Show();
            }
            if (e.Position == 7)
            {
                StartActivity(typeof(AboutActivity));
                //Toast.MakeText(this, "You clicked My bookings", ToastLength.Short).Show();
            }
        }
    }
}