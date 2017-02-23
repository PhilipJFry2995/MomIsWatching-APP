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
using MomIsWatching.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ServiceStarter))]
namespace MomIsWatching.Droid
{
    class ServiceStarter : ILocationService
    {

        public void startService()
        {
            Toast.MakeText(Xamarin.Forms.Forms.Context, "Trying to start service...", ToastLength.Short).Show();
            Intent locationService = new Intent(Xamarin.Forms.Forms.Context, typeof(LocationService));
            Xamarin.Forms.Forms.Context.StartService(locationService);
        }

        public void stopService()
        {
            Intent locationService = new Intent(Xamarin.Forms.Forms.Context, typeof(LocationService));
            Xamarin.Forms.Forms.Context.StopService(locationService);
        }
    }
}