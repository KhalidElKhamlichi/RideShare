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
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Ride_Share.Fragments;

namespace Ride_Share
{
    [Activity(Label = "My Rides", Theme = "@style/Theme.DesignDemo")]
    public class MyRidesActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        ImageView profileImage;
        CoordinatorLayout cl;
        string username;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MyRides);

            username = HomeActivity.currentUser.Username;

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout_myRides);
            navigationView = FindViewById<NavigationView>(Resource.Id.navigation_myRides);
            navigationView.SetCheckedItem(Resource.Id.nav_rides);
            cl = FindViewById<CoordinatorLayout>(Resource.Id.coordinator);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            cl.RemoveView(fab);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);
            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);

            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(viewPager);

            tabs.SetupWithViewPager(viewPager);
            View headerLayout = navigationView.GetHeaderView(0);

            TextView usernameView = headerLayout.FindViewById<TextView>(Resource.Id.UsernameHeader);
            usernameView.Text = username;
            profileImage = headerLayout.FindViewById<ImageView>(Resource.Id.imgViewHeader);
            profileImage.Click += goToProfile;
            //profileImage.SetImageBitmap(currentUser.Image);

            navigationView.NavigationItemSelected += (sender, e) => {
                //e.MenuItem.SetChecked(true);
                var item = e.MenuItem;
                switch (item.ItemId)
                {
                    case Resource.Id.nav_rides:
                        break;
                    case Resource.Id.nav_home:
                        Finish();
                        break;
                    case Resource.Id.nav_demand:
                        goToAddDemand();
                        break;
                    case Resource.Id.nav_logout:
                        Logout();
                        break;

                }
                drawerLayout.CloseDrawers();
                navigationView.SetCheckedItem(Resource.Id.nav_rides);
            };

        }

        protected override void  OnResume()
        {
            base.OnResume();
            navigationView.SetCheckedItem(Resource.Id.nav_rides);
        }

        private void goToAddDemand()
        {
            StartActivity(new Intent(this, typeof(AddDemandActivity)));
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            MyRidesCreatedFragment myRides = new MyRidesCreatedFragment();
            MyRidesJoinedFragment joined = new MyRidesJoinedFragment();
            MyRidesDemandsFragment myDemands = new MyRidesDemandsFragment();
            //offers.setUsername(username);
            if (!HomeActivity.currentUser.Type.Contains("Passenger"))
                adapter.AddFragment(myRides, "Created");
            adapter.AddFragment(joined, "Joined");
            adapter.AddFragment(myDemands, "My Demands");

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
                    if (profileImage.Drawable == null)
                        profileImage.SetImageBitmap(HomeActivity.currentUser.Image);
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

        private void goToProfile(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            StartActivity(intent);
        }
    }
}