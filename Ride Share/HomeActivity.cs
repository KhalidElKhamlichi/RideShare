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

using Users;
using Rides;
using System.Net;
using Newtonsoft.Json;
using Android.Support.V4.Widget;
using Android.Graphics;
using System.ComponentModel;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using static Android.Widget.AdapterView;
using Android.Util;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

using Android.Support.V4.View;
using Android.Support.V4.App;
using Java.Lang;
using Ride_Share.Fragments;
using Refractored.Controls;
using Android.Graphics.Drawables;

namespace Ride_Share
{
    //[Activity(Label = "Home", Theme = "@style/Theme.AppCompat")]
    [Activity(Label = "Home", Theme = "@style/Theme.DesignDemo")]
    public class HomeActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        public static User currentUser = new User();
        CircleImageView profileImage;
        string username;
        SearchDialogFragment df;
        OffersFragment offers;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Console.WriteLine("onCreate home called");
            SetContentView(Resource.Layout.Home);

            username = Intent.GetStringExtra("Username");
            currentUser.Username = username;

            if (currentUser.Image == null)
                currentUser.setImg();

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.navigation);
           
            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);
            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (sender, e) => {
                Android.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                df = new SearchDialogFragment();
                //df.SetStyle(DialogFragmentStyle.NoTitle, Resource.Style.Base_Theme_DesignDemo_FloatingDialog);
                df.DialogClosed += offers.OnSearch;
                df.Show(transaction, "Search");
            };

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            SetUpViewPager(viewPager);

            tabs.SetupWithViewPager(viewPager);
            View headerLayout = navigationView.GetHeaderView(0);
            
            TextView usernameView = headerLayout.FindViewById<TextView>(Resource.Id.UsernameHeader);
            usernameView.Text = username;
            profileImage = headerLayout.FindViewById<CircleImageView>(Resource.Id.imgViewHeader);
            profileImage.Click += goToProfile;

            navigationView.NavigationItemSelected += (sender, e) => {
                e.MenuItem.SetChecked(true);
                var item = e.MenuItem;
                switch (item.ItemId)
                {
                    case Resource.Id.nav_rides:
                        goToMyRide();
                        break;
                    case Resource.Id.nav_demand:
                        goToAddDemand();
                        break;
                    case Resource.Id.nav_logout:
                        Logout();
                        break;
                    
                }
                drawerLayout.CloseDrawers();
                navigationView.SetCheckedItem(Resource.Id.nav_home);
            };
            

        }

        protected override void OnResume()
        {
            base.OnResume();
            navigationView.SetCheckedItem(Resource.Id.nav_home);
        }

        private void goToAddDemand()
        {
            StartActivity(new Intent(this, typeof(AddDemandActivity)));
        }

        private void goToMyRide()
        {
            StartActivity(new Intent(this, typeof(MyRidesActivity)));
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            offers = new OffersFragment();
            DemandsFragment demands = new DemandsFragment();
            AddRideFragment addRide = new AddRideFragment();
            Log.Info("myapp", "username: "+username);
            addRide.setDriver(username);

            adapter.AddFragment(offers, "Offers");
            if (!currentUser.Type.Contains("Passenger"))
            {
                adapter.AddFragment(demands, "Demands");
                adapter.AddFragment(addRide, "Offer Ride");
            }
            viewPager.Adapter = adapter;
        }


        
        void Logout()
        {
            var intent = new Intent(this, typeof(LoginActivity));
            /*intent.AddFlags(ActivityFlags.ClearTask);
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.NewTask);*/
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.ClearTop);

            StartActivity(intent);
            FinishAffinity();
            //Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if(profileImage.Drawable == null)
                        profileImage.SetImageBitmap(currentUser.Image);
                    if (drawerLayout.IsDrawerOpen((int)GravityFlags.Left))
                    {
                        drawerLayout.CloseDrawers();
                    }
                    else
                        drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(RideDetailsActivity));
            intent.PutExtra("pos", e.Position.ToString());
            StartActivity(intent);
        }

        private void goToProfile(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            StartActivity(intent);
        }

    }

    public class TabAdapter : FragmentPagerAdapter
    {
        public List<SupportFragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        public TabAdapter(SupportFragmentManager sfm) : base(sfm)
        {
            Fragments = new List<SupportFragment>();
            FragmentNames = new List<string>();
        }

        public void AddFragment(SupportFragment fragment, string name)
        {
            Fragments.Add(fragment);
            FragmentNames.Add(name);
        }

        public override int Count
        {
            get
            {
                return Fragments.Count;
            }
        }

        public override SupportFragment GetItem(int position)
        {
            return Fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(FragmentNames[position]);
        }
    }
}