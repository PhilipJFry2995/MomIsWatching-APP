using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Diagnostics;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Java.Lang;

namespace MomIsWatching
{
    public partial class MainPage : ContentPage
    {
        private ISocketHandler handler;

        public MainPage()
        {
            InitializeComponent();

            StartService();
        }
        
        async void OnButtonClicked(object sender, EventArgs args)
        {
            //ILocationService service = DependencyService.Get<ILocationService>();
            //service.startService();
            //buttonAnimate(4);

            var results = await CrossGeolocator.Current.GetPositionAsync(10000);
            if(handler == null)
            {
                handler = DependencyService.Get<ISocketHandler>();
            }
            handler.sendSocketInfo(results);
        }

        async void StartService()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "Gunna need that location", "OK");
                    }
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                    status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    ILocationService service = DependencyService.Get<ILocationService>();
                    service.startService();
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
            }
        }

        void buttonAnimate(int count)
        {
            Task.Run(() =>
            {
                List<Image> images = new List<Image>();

                for (int i = 0; i < count; i++)
                {
                    Image circleImg = new Image()
                    {
                        Source = ImageSource.FromFile("circle.png"),
                        Margin = new Thickness(0, 80, 0, 0),
                        WidthRequest = 300,
                        HeightRequest = 300
                    };

                    var centerX = Constraint.RelativeToParent(parent => 0);
                    var centerY = Constraint.RelativeToParent(parent => 0);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        relativeView.Children.Add(circleImg, centerX, centerY);
                        relativeView.RaiseChild(unlockButton);
                    });

                    images.Add(circleImg);

                    Task.WhenAll(
                        circleImg.ScaleTo(4, 1000),
                        circleImg.FadeTo(0, 900)
                    );

                    Thread.Sleep(300);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    // Cleaning
                    foreach (Image image in images)
                        relativeView.Children.Remove(image);
                });
            });
        }
    }
}
