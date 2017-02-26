using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Geolocator;
using System.Diagnostics;
using Java.IO;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Java.Lang;

namespace MomIsWatching
{
    public partial class MainPage
    {
        private ISocketHandler _handler;

        public MainPage()
        {
            InitializeComponent();

            RequestPermission();
        }
        
        async void OnButtonClicked(object sender, EventArgs args)
        {
            unlockButton.IsEnabled = false;
            unlockButton.TextColor = Color.Gray;
            ButtonAnimate(4);

            var results = await CrossGeolocator.Current.GetPositionAsync(10000);
            if(_handler == null)
            {
                _handler = DependencyService.Get<ISocketHandler>();
            }
            _handler.sendSocketInfo(results);

            unlockButton.IsEnabled = true;
            unlockButton.TextColor = Color.White;
        }

        async void RequestPermission()
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
                if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
            }
        }

        void ButtonAnimate(int count)
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
                    Debug.WriteLine("Sleeeeeeeeeeeeeeeeeeeeeeping");
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
