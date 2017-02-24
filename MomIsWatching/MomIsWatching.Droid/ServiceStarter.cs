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
            //Intent locationService = new Intent(Xamarin.Forms.Forms.Context, typeof(LocationService));
            //Xamarin.Forms.Forms.Context.StartService(locationService);

            // Construct an intent that will execute the AlarmReceiver
            Intent intent = new Intent(Xamarin.Forms.Forms.Context, typeof(LocationUpdateReceiver));
            // Create a PendingIntent to be triggered when the alarm goes off
            PendingIntent pIntent = PendingIntent.GetBroadcast(Xamarin.Forms.Forms.Context, 0, intent, PendingIntentFlags.UpdateCurrent);
            // Setup periodic alarm every 5 seconds
            long firstMillis = SystemClock.ElapsedRealtime(); // alarm is set right away
            AlarmManager alarm = (AlarmManager)Xamarin.Forms.Forms.Context.GetSystemService("alarm");
            // First parameter is the type: ELAPSED_REALTIME, ELAPSED_REALTIME_WAKEUP, RTC_WAKEUP
            // Interval can be INTERVAL_FIFTEEN_MINUTES, INTERVAL_HALF_HOUR, INTERVAL_HOUR, INTERVAL_DAY
            long interval = 5 * 1000;
            alarm.SetRepeating(AlarmType.RtcWakeup, firstMillis, interval, pIntent);
        }


        public void stopService()
        {
            
        }
    }
}