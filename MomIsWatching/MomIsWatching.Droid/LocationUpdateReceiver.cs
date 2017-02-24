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

using Plugin.Geolocator;
using Android.Util;
using Plugin.Battery;
using Xamarin.Forms;

namespace MomIsWatching.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    class LocationUpdateReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(Forms.Context, "Publishing coordinates...", ToastLength.Short).Show();
            publishCoordinates();
        }

        private async void publishCoordinates()
        {
            var position = await CrossGeolocator.Current.GetPositionAsync(10000);
            
            string package = "{ "
                + "\"deviceId\":" + "\"" + MainActivity.deviceId + "\","
                + "\"location\":" + "\"" + position.Latitude.ToString().Replace(',', '.') + ";" + position.Longitude.ToString().Replace(',', '.') + "\","
                + "\"charge\":" + CrossBattery.Current.RemainingChargePercent + ","
                + "\"isSos\":" + "0 }";
            Log.Debug("tag", "Package:" + package);

            MainActivity.websocket.Send(package);
        }
    }
}