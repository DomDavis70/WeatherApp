using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Service.Voice;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace WeatherApp.Fragments
{
    public class progressDialogueFragment : Android.Support.V4.App.DialogFragment
    {
        string thisStatus;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        
        public progressDialogueFragment(string status)
        {
            thisStatus = status;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.progress, container, false);
            TextView statusTextView = (TextView)view.FindViewById(Resource.Id.currentStatus);
            statusTextView.Text = thisStatus;
            return view;
        }

        
    }
}