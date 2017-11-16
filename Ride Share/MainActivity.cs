using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace Ride_Share
{
    [Activity(Label = "Ride_Share", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button login_btn;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            //SetContentView (Resource.Layout.l);

            login_btn = FindViewById<Button>(Resource.Id.LoginBtn);

            
        }

        
    }
}

