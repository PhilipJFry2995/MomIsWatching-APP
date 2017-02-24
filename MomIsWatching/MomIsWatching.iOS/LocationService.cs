using System;
using System.Security.Cryptography;
using System.Text;
using Foundation;
using MomIsWatching.iOS;
using Plugin.Battery;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Security;
using WebSocket4Net;

[assembly: Xamarin.Forms.Dependency(typeof(LocationService))]
namespace MomIsWatching.iOS
{
	public class LocationService : ILocationService
	{
		private WebSocket websocket;
		private Position position;

		public void startService()
		{
			//CrossGeolocator.Current.AllowsBackgroundUpdates = true;
			//CrossGeolocator.Current.PositionChanged += (sender, e) =>
			// {
			//	 position = e.Position;
			//	 sendSocketInfo();
			// };

		}

		public void stopService()
		{
			throw new NotImplementedException();
		}

		public void sendSocketInfo()
		{
			websocket = new WebSocket("ws://momiswatching.azurewebsites.net/Subscriptions/DeviceSubscriptionHandler.ashx?deviceId=" + GetIdentifier()); // ?deviceId=...
			websocket.Opened += websocket_Opened;
			websocket.Open();
		}

		private void websocket_Opened(object sender, EventArgs e)
		{
			//{ 'deviceId': 'blabla', 'location': '32.32;12.12', 'charge': 42, 'isSos': 0 }
			string package = "{"
				+ "\"deviceId\":" + "\"" + GetIdentifier() + "\","
				+ "\"location\":" + "\"" + position.Latitude.ToString().Replace(',', '.') + ";" + position.Longitude.ToString().Replace(',', '.') + "\","
			                                       + "\"charge\":" + Math.Abs(CrossBattery.Current.RemainingChargePercent) + ","
				+ "\"isSos\":" + "1}";
			Console.WriteLine("Package:" + package);
			websocket.Send(package);
			websocket.Close();
		}

		public string GetIdentifier()
		{
			var query = new SecRecord(SecKind.GenericPassword);
			query.Service = NSBundle.MainBundle.BundleIdentifier;
			query.Account = "UniqueID";

			NSData uniqueId = SecKeyChain.QueryAsData(query);
			if (uniqueId == null)
			{
				query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
				var err = SecKeyChain.Add(query);
				if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
					return "myCoolIphoneSimulatorWithNoUniqueId";

				return CalculateMD5Hash(query.ValueData.ToString());
			}
			else
			{
				return CalculateMD5Hash(uniqueId.ToString());
			}
		}

		public static string CalculateMD5Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			MD5 md5 = System.Security.Cryptography.MD5.Create();

			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
		}
	}
}
