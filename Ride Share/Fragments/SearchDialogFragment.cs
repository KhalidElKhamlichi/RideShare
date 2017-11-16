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

namespace Ride_Share.Fragments
{
    class SearchDialogFragment : DialogFragment
    {
        public event EventHandler<DialogEventArgs> DialogClosed;

        EditText From, To, Date, Time;
        NumberPicker np;
        TimePicker timePicker;
        DatePickerDialog dateDialog;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //return base.OnCreateView(inflater, container, savedInstanceState);
            //SetStyle(DialogFragmentStyle.NoTitle, Resource.Style.Base_Theme_DesignDemo_FloatingDialog);

            View view = inflater.Inflate(Resource.Layout.SearchDialog, container, false);
            timePicker = view.FindViewById<TimePicker>(Resource.Id.timePickerSearch);
            From = view.FindViewById<EditText>(Resource.Id.FromSearch);
            To = view.FindViewById<EditText>(Resource.Id.ToSearch);
            Date = view.FindViewById<EditText>(Resource.Id.DatePickerSearch);
            //Time = view.FindViewById<EditText>(Resource.Id.TimeSearch);
            Button search = view.FindViewById<Button>(Resource.Id.SearchBtn);
            np = view.FindViewById<NumberPicker>(Resource.Id.FlexSearch);
            np.MinValue = 0;
            np.MaxValue = 12;
            np.Value = 1;

            timePicker.SetIs24HourView(Java.Lang.Boolean.True);

            search.Click += (sender, e) =>
            {
                Dismiss();
            };

            DateTime today = DateTime.Today;
            today = today.AddDays(2);

            dateDialog = new DatePickerDialog(this.Context, OnDateSet, today.Year, today.Month - 1, today.Day);
            dateDialog.DatePicker.MinDate = today.Millisecond;
            //dateDialog.DatePicker.SetBackgroundColor(new Android.Graphics.Color(56, 57, 58));
            Date.Focusable = false;
            Date.FocusableInTouchMode = false;
            Date.Clickable = false;
            Date.LongClickable = false;
            Date.SetCursorVisible(false);
            Date.Click += (sender, e) =>
            {
                dateDialog.Show();
            };

            return view;

        }

        private void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            Date.Text = dateDialog.DatePicker.DateTime.ToString("dd/MM/yyy");
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

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                DialogClosed(this, new DialogEventArgs {
                    Origin = From.Text,
                    Destination = To.Text,
                    Time = getTime(),
                    Date = Date.Text,
                    TimeFlex = np.Value
                    
                });
            }

        }
    }

    public class DialogEventArgs
    {
        public string Origin { set; get; }
        public string Destination { set; get; }
        public string Date { set; get; }
        public string Time { set; get; }
        public double TimeFlex { set; get; }
    }
}