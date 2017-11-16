using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SupportFragment = Android.Support.V4.App.Fragment;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Net;
using System.Collections.Specialized;
using Android.Text.Format;

namespace Ride_Share.Fragments
{
    public class AddRideFragment : SupportFragment
    {
        NumberPicker seats;
        Spinner BackbagSpinner;
        Spinner TravelbagSpinner;
        Button submit;
        EditText origin, destination, date, time;
        CheckBox smokes, stops;
        TimePicker timePicker;
        DatePickerDialog dateDialog;
        string Driver;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            View view = null;
            Log.Info("myapp", "offers oncreateview called");
            try
            {
                view = inflater.Inflate(Resource.Layout.AddRide, container, false);
                if(timePicker==null)
                    timePicker = view.FindViewById<TimePicker>(Resource.Id.timePicker);
                BackbagSpinner = view.FindViewById<Spinner>(Resource.Id.BackBagSpinner);
                TravelbagSpinner = view.FindViewById<Spinner>(Resource.Id.TravelbagSpinner);
                seats = view.FindViewById<NumberPicker>(Resource.Id.SeatsPicker);
                submit = view.FindViewById<Button>(Resource.Id.SubmitBtn);
                origin = view.FindViewById<EditText>(Resource.Id.OriginInput);
                date = view.FindViewById<EditText>(Resource.Id.DatePicker);
                destination = view.FindViewById<EditText>(Resource.Id.DestinationInput);
                //time = view.FindViewById<EditText>(Resource.Id.TimePicker);
                smokes = view.FindViewById<CheckBox>(Resource.Id.SmokesCheckBox);
                stops = view.FindViewById<CheckBox>(Resource.Id.StopsCheckBox);

                var adapter = ArrayAdapter.CreateFromResource(
                        this.Context, Resource.Array.sizes_array, Android.Resource.Layout.SimpleSpinnerItem);

                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                BackbagSpinner.Adapter = adapter;
                TravelbagSpinner.Adapter = adapter;

                seats.MinValue = 1;
                seats.MaxValue = 6;
                seats.Value = 1;
                if(!timePicker.Is24HourView())
                    timePicker.SetIs24HourView(Java.Lang.Boolean.True);
                submit.Click += AddRide;

                DateTime today = DateTime.Today;
                today = today.AddDays(2);

                dateDialog = new DatePickerDialog(this.Context, OnDateSet, today.Year, today.Month - 1, today.Day);
                dateDialog.DatePicker.MinDate = today.Millisecond;
                //dateDialog.DatePicker.SetBackgroundColor(new Android.Graphics.Color(56, 57, 58));
                date.Focusable = false;
                date.FocusableInTouchMode = false;
                date.Clickable = false;
                date.LongClickable = false;
                date.SetCursorVisible(false);
                date.Click += (sender, e) =>
                {
                    dateDialog.Show();
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return view;

        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            date.Text = dateDialog.DatePicker.DateTime.ToString("dd/MM/yyy");
        }

        private string getTime()
        {
            StringBuilder strTime = new StringBuilder();
            string hours = timePicker.Hour.ToString();
            string minutes = timePicker.Minute.ToString();
            if (timePicker.Hour < 10)
                hours = "0" + timePicker.Hour;
            if(timePicker.Minute < 10)
                minutes = "0" + timePicker.Minute;

            strTime.Append(hours + ":" + minutes);
            
            return strTime.ToString();
        }

        void AddRide(object sender, EventArgs args)
        {
            Log.Info("myapp", "Driver: "+Driver);
            WebClient client = new WebClient();
            string ip = Resources.GetString(Resource.String.ServerIP);
            Uri uri = new Uri("http://" + ip + "/insert_ride.php");
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("origin", origin.Text);
            parameters.Add("destination", destination.Text);
            parameters.Add("date", date.Text);
            parameters.Add("time", getTime());
            parameters.Add("stops", stops.Checked.ToString());
            parameters.Add("smokes", smokes.Checked.ToString());
            parameters.Add("seats", seats.Value.ToString());
            parameters.Add("backbag", BackbagSpinner.SelectedItem.ToString());
            parameters.Add("travelbag", TravelbagSpinner.SelectedItem.ToString());
            parameters.Add("driver", Driver);

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {

            string receivedString = Encoding.UTF8.GetString(args.Result);
            

            var dialog = new AlertDialog.Builder(this.Context);
            dialog.SetMessage(receivedString);
            dialog.SetNeutralButton("OK", delegate
            {

            });
            dialog.Show();

            if (receivedString.Contains("Success."))
            {
                var intent = new Intent(this.Context, typeof(HomeActivity));
                intent.PutExtra("Username", Driver);
                StartActivity(intent);
                //Finish();
            }

        }

        public void setDriver(string username)
        {
            Driver = username;
        }
    }
}