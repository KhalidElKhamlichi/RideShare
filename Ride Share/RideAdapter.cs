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
    class RideAdapter : BaseAdapter<Ride>
    {
        Activity context;
        List<Ride> rides;

        public RideAdapter(Activity Context, List<Ride> Rides)
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
            var view = context.LayoutInflater.Inflate(Resource.Layout.RideRow, parent, false);
            Refractored.Controls.CircleImageView photo = view.FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.photo);
            var name = view.FindViewById<TextView>(Resource.Id.nameTextView);
            var ridePath = view.FindViewById<TextView>(Resource.Id.specialTextView);
            var date = view.FindViewById<TextView>(Resource.Id.dateTextView);

            //Stream stream = context.Assets.Open("profile.png");
            //Drawable drawable = Drawable.CreateFromStream(stream, null);

            //photo.SetImageDrawable(drawable);
            if (rides[position].Driver.Equals("empty"))
            {
                try
                {
                    photo.SetImageBitmap(rides[position].UserSubmitter.Image);
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                }
                name.Text = rides[position].Submitter;
            }
                
            else
            {
                try
                {
                    photo.SetImageBitmap(rides[position].Userdriver.Image);
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                }
                name.Text = rides[position].Driver;
            }
                
            ridePath.Text = rides[position].ToString();
            date.Text = rides[position].Date;
            return view;
        }
    
    }
}