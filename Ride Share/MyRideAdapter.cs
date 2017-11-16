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

using Rides;
using Users;
using System.IO;
using Android.Graphics.Drawables;
using Android.Util;


namespace Ride_Share
{
    class MyRideAdapter : BaseAdapter<Ride>
    {
        Activity context;
        List<Ride> rides;

        public MyRideAdapter(Activity Context, List<Ride> Rides)
        {
            context = Context;
            rides = Rides;
        }

        public override Ride this[int position]
        {
            get
            {
                return rides[position];
            }
        }

        public override int Count
        {
            get
            {
                return rides.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.MyRidesCreatedRow, parent, false);
            var FromView = view.FindViewById<TextView>(Resource.Id.FromTxt);
            var ToView = view.FindViewById<TextView>(Resource.Id.ToTxt);
            var DateView = view.FindViewById<TextView>(Resource.Id.DateTxt);
            var TimeView = view.FindViewById<TextView>(Resource.Id.TimeTxt);

            FromView.Text += rides[position].Origin;
            ToView.Text += rides[position].Destination;
            DateView.Text = rides[position].Date;
            TimeView.Text += rides[position].Time;

            return view;
        }

    }
}