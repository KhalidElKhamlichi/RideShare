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
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Rides;
using System.Net;
using System.Collections.Specialized;

namespace Ride_Share
{
    [Activity(Label = "Add demand", Theme = "@style/Theme.DesignDemo")]
    public class AddDemandActivity : Android.Support.V7.App.AppCompatActivity
    {
        Spinner BackbagSpinner;
        Spinner TravelbagSpinner;
        Button submit;
        EditText origin, destination, date;
        CheckBox smokes, stops;
        DatePickerDialog dateDialog;
        TimePicker timePicker;

        string Driver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddDemand);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            BackbagSpinner = FindViewById<Spinner>(Resource.Id.BackBagSpinner);
            TravelbagSpinner = FindViewById<Spinner>(Resource.Id.TravelbagSpinner);
            submit = FindViewById<Button>(Resource.Id.SubmitBtn);
            origin = FindViewById<EditText>(Resource.Id.OriginInput);
            date = FindViewById<EditText>(Resource.Id.DatePicker);
            destination = FindViewById<EditText>(Resource.Id.DestinationInput);
            timePicker = FindViewById<TimePicker>(Resource.Id.TimePicker);
            smokes = FindViewById<CheckBox>(Resource.Id.SmokesCheckBox);
            stops = FindViewById<CheckBox>(Resource.Id.StopsCheckBox);

            timePicker.SetIs24HourView(Java.Lang.Boolean.True);

            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.sizes_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            BackbagSpinner.Adapter = adapter;
            TravelbagSpinner.Adapter = adapter;


            DateTime today = DateTime.Today;
            today = today.AddDays(2);
            dateDialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
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

            submit.Click += AddRide;
        }

        private string getTime()
        {
            StringBuilder strTime = new StringBuilder();
            string hours = timePicker.Hour.ToString();
            string minutes = timePicker.Minute.ToString();
            if (timePicker.Hour < 10)
                hours = "0" + timePicker.Hour;
            if (timePicker.Minute < 10)
                minutes = "0" + timePicker.Minute;

            strTime.Append(hours + ":" + minutes);

            return strTime.ToString();
        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            date.Text = dateDialog.DatePicker.DateTime.ToString("dd/MM/yyy");
        }

        void AddRide(object sender, EventArgs args)
        {

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
            parameters.Add("backbag", BackbagSpinner.SelectedItem.ToString());
            parameters.Add("travelbag", TravelbagSpinner.SelectedItem.ToString());
            parameters.Add("submitter", HomeActivity.currentUser.Username);

            client.UploadValuesCompleted += uploadValueComplete;
            client.UploadValuesAsync(uri, parameters);
        }

        void uploadValueComplete(object sender, UploadValuesCompletedEventArgs args)
        {

            string receivedString = Encoding.UTF8.GetString(args.Result);


            var dialog = new AlertDialog.Builder(this);
            dialog.SetMessage(receivedString);
            dialog.SetNeutralButton("OK", delegate
            {

            });
            dialog.Show();

            if (receivedString.Contains("Success."))
            {
                Finish();
            }

        }
    }
}