using System.Globalization;

using Android.Content;
using Android.Widget;

using Plugin.Geolocator;
using Android.Util;
using Plugin.Battery;
using Xamarin.Forms;
using System;

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
            if (CrossGeolocator.Current.IsGeolocationAvailable)
            {
                try
                {
                    var position = await CrossGeolocator.Current.GetPositionAsync();
                
                string package = "{ "
                                 + "\"DeviceId\":" + "\"" + MainActivity.DeviceId + "\","
                                 + "\"Location\":" + "\"" +
                                 position.Latitude.ToString(CultureInfo.InvariantCulture).Replace(',', '.') + ";" +
                                 position.Longitude.ToString(CultureInfo.InvariantCulture).Replace(',', '.') + "\","
                                 + "\"Charge\":" + CrossBattery.Current.RemainingChargePercent + ","
                                 + "\"IsSos\":" + "0 }";
                Log.Debug("tag", "Package:" + package);

                MainActivity.Websocket.Send(package);
                }
                catch (Exception e) { Log.Debug("mytag", "BABAX"); }
            }
            else
            {
                Log.Debug("tag", "GPS is not active");
            }
            
        }
    }
}