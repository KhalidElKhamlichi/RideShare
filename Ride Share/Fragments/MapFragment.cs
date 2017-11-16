using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;

namespace Ride_Share.Fragments
{
    public class MapFragment : DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);



        }  

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Map, container, false);

            /*var mapFragment = ((SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapView));
            mapFragment.GetMapAsync(this);
        

            var geo = new Geocoder(this);

            var addressesO = geo.GetFromLocationName(ride.Origin + ", Maroc", 1);
            originAddress = addressesO[0];
            var addressesD = geo.GetFromLocationName(ride.Destination + ", Maroc", 1);
            destinationAddress = addressesD[0];

            string strGoogleDirectionUrl = "https://maps.googleapis.com/maps/api/directions/json?origin=" + ride.Origin + ",Maroc" + "&destination=" + ride.Destination + ",Maroc" + "&key=AIzaSyCS4xW5LbEkzjcd5ATuNnaX4Jss6uyU--c";
            string strJSONDirectionResponse = await FnHttpRequest(strGoogleDirectionUrl);
            var objRoutes = JsonConvert.DeserializeObject<googledirectionclass>(strJSONDirectionResponse);

            string encodedPoints = objRoutes.routes[0].overview_polyline.points;
            var lstDecodedPoints = FnDecodePolylinePoints(encodedPoints);
            //convert list of location point to array of latlng type
            var latLngPoints = new LatLng[lstDecodedPoints.Count];
            int index = 0;
            foreach (Location loc in lstDecodedPoints)
            {
                latLngPoints[index++] = new LatLng(loc.Latitude, loc.Longitude);
            }

            var polylineoption = new PolylineOptions();
            polylineoption.InvokeColor(Color.Red);
            polylineoption.Geodesic(true);
            polylineoption.Add(latLngPoints);
            Map.AddPolyline(polylineoption);*/

            return view;
        }
    }
}