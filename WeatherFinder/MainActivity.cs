using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;

namespace WeatherFinder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private DrawerLayout _drawerLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.drawer_layout);
            ConfigureNavigation();

            if (FragmentManager.FindFragmentById(Resource.Id.fragment_view) == null)
                InitializeView();
        }

        /// <summary>
        /// Configure the navigation drawer and banner
        /// </summary>
        private void ConfigureNavigation()
        {
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            var nv = FindViewById<NavigationView>(Resource.Id.nav_view);
            nv.SetNavigationItemSelectedListener(this);

            var tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //inflates the menu. Using this instead of overriding OnCreateOptionsMenu() to avoid the overflow menu
            tb.InflateMenu(Resource.Menu.main_menu);
            SetSupportActionBar(tb);
            var ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeButtonEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_grey_white_1000_18dp);
        }

        /// <summary>
        /// On initial run, gets the default fragment of the UI and initializes components within it.
        /// </summary>
        private void InitializeView()
        {
            using (var tran = FragmentManager.BeginTransaction())
            {
                var fragment = new WeatherFinderFragment();
                tran.Add(Resource.Id.fragment_view, fragment, string.Empty);
                tran.Commit();
            }
        }

        /// <summary>
        /// When an item in the nav drawer is clicked, handle the event
        /// </summary>
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            using (var tran = FragmentManager.BeginTransaction())
            {
                Fragment f;
                if (menuItem.ItemId == Resource.Id.forecast)
                    f = new WeatherForecastFragment();
                else
                    f = new WeatherFinderFragment();

                tran.Replace(Resource.Id.fragment_view, f, string.Empty);
                tran.Commit();
            }

            _drawerLayout.CloseDrawers();
            return true;
        }

        /// <summary>
        /// Handles clicking the burger button in the action bar
        /// </summary>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}

