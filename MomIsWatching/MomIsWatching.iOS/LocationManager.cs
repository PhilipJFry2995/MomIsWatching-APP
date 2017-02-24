using System;

using CoreLocation;
using UIKit;
using Foundation;
using Plugin.Battery;

namespace MomIsWatching.iOS
{
	public class LocationManager
	{
		protected CLLocationManager locMgr;
		private DateTime lastUpdate;

		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

		public LocationManager ()
		{
			locMgr = new CLLocationManager ();

			locMgr.PausesLocationUpdatesAutomatically = false; 

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				locMgr.RequestAlwaysAuthorization();
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				locMgr.AllowsBackgroundLocationUpdates = true;
			}
			LocationUpdated += SendLocation;

			lastUpdate = DateTime.Now;
		}

		public CLLocationManager LocMgr {
			get { return this.locMgr; }
		}
		
		public void StartLocationUpdates ()
		{
			if (CLLocationManager.LocationServicesEnabled) {

				LocMgr.DesiredAccuracy = 1;

				if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {
					LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {
						this.LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
					};

				} else {
					LocMgr.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
						this.LocationUpdated (this, new LocationUpdatedEventArgs (e.NewLocation));
					};
				}

				LocMgr.StartUpdatingLocation ();

				LocMgr.Failed += (object sender, NSErrorEventArgs e) => {
					Console.WriteLine (e.Error);
				}; 
				
			} 
			else 
			{
				Console.WriteLine ("Location services not enabled, please enable this in your Settings");
			}
		}

		public void SendLocation (object sender, LocationUpdatedEventArgs e)
		{
			CLLocation location = e.Location;

			Console.WriteLine ("Longitude: " + location.Coordinate.Longitude);
			Console.WriteLine ("Latitude: " + location.Coordinate.Latitude);

			Console.WriteLine((DateTime.Now - lastUpdate).Seconds);

			if ((DateTime.Now - lastUpdate).Seconds >= AppDelegate.interval)
			{
				string package = "{"
				+ "\"DeviceId\":" + "\"" + AppDelegate.deviceId + "\","
				+ "\"Location\":" + "\"" + location.Coordinate.Latitude.ToString().Replace(',', '.') + ";" + location.Coordinate.Longitude.ToString().Replace(',', '.') + "\","
												   + "\"Charge\":" + Math.Abs(CrossBattery.Current.RemainingChargePercent) + ","
				+ "\"IsSos\":" + "0}";
				Console.WriteLine("Package:" + package);
				AppDelegate.websocket.Send(package);
				lastUpdate = DateTime.Now;
			}
			else 
			{
				Console.WriteLine("Time hasn't come yet, young samurai");
			}

		}
		
	}
}