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

namespace Ride_Share
{
    public class ValidationMethods
    {
        public static bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

        public static bool isValidPhone(string phonenumber)
        {
            return Android.Util.Patterns.Phone.Matcher(phonenumber).Matches();
        }
    }
}