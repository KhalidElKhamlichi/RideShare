
using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

using Android.OS;

namespace Ride_Share
{
    [Activity(Label = "XA GoogleMapV2 Basic Map")]
    public class MapActivity : Android.Support.V4.App.FragmentActivity, IOnMapReadyCallback
    {
        /**
     * Note that this may be null if the Google Play services APK is not available.
     */
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);

            /*var mapFragment = ((SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map));
            mapFragment.GetMapAsync(this);*/
        }

        /**
     * This is where we can add markers or lines, add listeners or move the camera. In this case, we
     * just add a marker near Africa.
     */
        public void OnMapReady(GoogleMap map)
        {
            map.AddMarker(new MarkerOptions().SetPosition(new LatLng(0, 0)).SetTitle("Marker"));
        }
    }
}

