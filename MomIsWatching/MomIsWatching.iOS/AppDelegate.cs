using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation;
using Security;
using UIKit;
using WebSocket4Net;

namespace MomIsWatching.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		public static WebSocket websocket;
		public static string deviceId;
		public static int interval;
		public static LocationManager Manager = null;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

			deviceId = GetIdentifier();
			websocket = new WebSocket("ws://momiswatching.azurewebsites.net/Subscriptions/DeviceSubscriptionHandler.ashx?deviceId=" + deviceId); // ?deviceId=...
			websocket.Open();
			websocket.MessageReceived += MessageReceiver;
			interval = 5;

			Manager = new LocationManager();
			Manager.StartLocationUpdates();

            return base.FinishedLaunching(app, options);
        }

		private void MessageReceiver(object sender, EventArgs e)
		{
			var arguments = (MessageReceivedEventArgs)e;
			interval = int.Parse(arguments.Message);

			Console.WriteLine("Message:" + arguments.Message);
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
