using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Climate.ServiceReference1;
using Facebook;
using System.Windows.Media.Imaging;

namespace Climate
{
    public partial class Temperature : PhoneApplicationPage
    {
        private const string FBApi = "231687710362137";
        private string message;
        private string starvationMessage;
        private string freezinfMessage;
        private string dehydratationMessage;
        private string fireMessage;
        private string randomMessage;
        private int randomIndex;
        private FacebookClient client;
        private List<string> randomReasons;
        public Temperature()
        {
            InitializeComponent();
            
            client = new FacebookClient();
            client.PostCompleted += (o, args) =>
            {
                //Checking for errors
                if (args.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                }
                else
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Message posted successfully"));
                }
            };
        }
        double latitude;
        double longitute;
        int year;
        string season;
        string location;
        List<TemperatureDate> maxTempDate;
        List<TemperatureDate> minTempDate;
        List<TemperatureDate> prepDate;
        private void LoadLocations()
        {
            GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");

            GeocodeRequest geocodeRequest = new GeocodeRequest();
            {
                geocodeRequest.Credentials = new Credentials() { ApplicationId = "AmLt8xEVcOt7YnueGJfSuGU14Y8ednN6f8TMUPwd50GZMkvzluzkIKzQKAB92zO2" };
                geocodeRequest.Query = location;
                geocodeRequest.Options = new GeocodeOptions()
                {
                    Filters = new System.Collections.ObjectModel.ObservableCollection<FilterBase>()
                };

            };
            geocodeRequest.Options.Filters.Add(new ConfidenceFilter()
            {
                MinimumConfidence = Confidence.High
            });

            geocodeService.GeocodeAsync(geocodeRequest);
            geocodeService.GeocodeCompleted += (sender, e) => geoCode_GeocodeCompleted(sender, e);
        }

        void geoCode_GeocodeCompleted(object sender, GeocodeCompletedEventArgs e)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            if (e.Result.Results.Count > 0)
            {
                GeocodeResult svcResult = e.Result.Results[0];

                // The service returns a list of locations. We will display each of them
                foreach (Location loc in svcResult.Locations.ToList())
                {
                    latitude = loc.Latitude;
                    longitute = loc.Longitude;
                }
            }
            JSONCall(1);
        }

        private void JSONCall(int direction)
        {
            WebClient webClient = new WebClient();
            var cell_index_y = (int)((49.9375 - latitude) / 0.0083333333);
            var cell_index_x = (int)((longitute - (-125.020833335)) / 0.0083333333);
            if (direction == 1)
            {
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                webClient.DownloadStringAsync(new Uri("http://opennexapp.s3.amazonaws.com/cesm1-cam5/rcp85r1i1p1/tmax/" + cell_index_x + "/" + cell_index_y));
            }
            else
                if (direction == -1)
                {
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompletedMin);
                    webClient.DownloadStringAsync(new Uri("http://opennexapp.s3.amazonaws.com/cesm1-cam5/rcp85r1i1p1/tmin/" + cell_index_x + "/" + cell_index_y));
                }
                else
                {
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompletedPrep);
                    webClient.DownloadStringAsync(new Uri("http://opennexapp.s3.amazonaws.com/cesm1-cam5/rcp85r1i1p1/prcp/" + cell_index_x + "/" + cell_index_y));
                }
                
        }
        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var s = e.Result.Split('[');
            var res = s[1].Split(']');
            var results = res[0].Split(',');
            DateTime date = new DateTime(1950,01,15);
            List<TemperatureDate> tempDate = new List<TemperatureDate>();
            foreach (var temp in results)
            {
                tempDate.Add(new TemperatureDate() { Date = date, Value = temp });               
                date = date.AddMonths(1);
            }
            List<int> months = new List<int>();
            if (season == "Summer")
            { months.Add(6); months.Add(7); months.Add(8); }
            else
            {
                months.Add(1); months.Add(2); months.Add(11); months.Add(12);
            }
            maxTempDate = tempDate.Where(temp => temp.Date.Year == year && months.Contains(temp.Date.Month)).ToList();
            JSONCall(-1);
           
        }
        void webClient_DownloadStringCompletedMin(object sender, DownloadStringCompletedEventArgs e)
        {
            var s = e.Result.Split('[');
            var res = s[1].Split(']');
            var results = res[0].Split(',');
            List<TemperatureDate> tempDate = new List<TemperatureDate>();
            DateTime date = new DateTime(1950, 01, 15);
            foreach (var temp in results)
            {
                tempDate.Add(new TemperatureDate() { Date = date, Value = temp });
                date = date.AddMonths(1);
            }
            List<int> months = new List<int>();
            if (season == "Summer")
            { months.Add(6); months.Add(7); months.Add(8); }
            else
            {
                months.Add(1); months.Add(2); months.Add(11); months.Add(12);
            }
            minTempDate = tempDate.Where(temp => temp.Date.Year == year && months.Contains(temp.Date.Month)).ToList();
            JSONCall(0);
        }


        void webClient_DownloadStringCompletedPrep(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var s = e.Result.Split('[');
                var res = s[1].Split(']');
                var results = res[0].Split(',');
                List<TemperatureDate> tempDate = new List<TemperatureDate>();
                DateTime date = new DateTime(1950, 01, 15);
                foreach (var temp in results)
                {
                    tempDate.Add(new TemperatureDate() { Date = date, Value = temp });
                    date = date.AddMonths(1);
                }
                List<int> months = new List<int>();
                if (season == "Summer")
                { months.Add(6); months.Add(7); months.Add(8); }
                else
                {
                    months.Add(1); months.Add(2); months.Add(11); months.Add(12);
                }
                prepDate = tempDate.Where(temp => temp.Date.Year == year && months.Contains(temp.Date.Month)).ToList();
            }
            catch (Exception ex)
            {
                prepDate = null;
            }
            CheckFreezing();
            CheckStarvation();
            CheckDehydratation();
            CheckFire();
        }
        private void CheckFreezing()
        {
            if (minTempDate.Any(temp => Double.Parse(temp.Value) >= 0))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/1.jpg", UriKind.Relative));
                gridFreezing.Background = brush;
                freezinfMessage = "I won't freeze to death.";
            }

            if (minTempDate.Any(temp => Double.Parse(temp.Value) < 0 && Double.Parse(temp.Value) >= -15))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/2.jpg", UriKind.Relative));
                gridFreezing.Background = brush;
                freezinfMessage = "I probably won't freeze to death.";
            }
            if (minTempDate.Any(temp => Double.Parse(temp.Value) < -15 && Double.Parse(temp.Value) >= -50))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/3.jpg", UriKind.Relative));
                gridFreezing.Background = brush;
                freezinfMessage = "I may freeze to death.";
            }
            if (minTempDate.Any(temp => Double.Parse(temp.Value) < -50))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/4.jpg", UriKind.Relative));
                gridFreezing.Background = brush;
                freezinfMessage = "I'll probably freeze to death.";
            }
        }

        private void CheckStarvation()
        {
            if (minTempDate.Any(temp => Double.Parse(temp.Value) <= -5) || maxTempDate.Any(temp => Double.Parse(temp.Value) >= 45))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/4.jpg", UriKind.Relative));
                gridStarvation.Background = brush;
                starvationMessage = "I'll probably starve to death.";
                return;
            }
            if ((minTempDate.Any(temp => Double.Parse(temp.Value) < 5 && Double.Parse(temp.Value) >= -5)) || (maxTempDate.Any(temp => Double.Parse(temp.Value) < 45 && Double.Parse(temp.Value) >= 30)))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/3.jpg", UriKind.Relative));
                gridStarvation.Background = brush;
                starvationMessage = "I may starve to death.";
                return;
            }
            if ((minTempDate.Any(temp => Double.Parse(temp.Value) < 10 && Double.Parse(temp.Value) >= 5)) || (maxTempDate.Any(temp => Double.Parse(temp.Value) < 30 && Double.Parse(temp.Value) >= 25)))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/2.jpg", UriKind.Relative));
                gridStarvation.Background = brush;
                starvationMessage = "I probably won't starve to death.";
                return;
            }
            var brush1 = new ImageBrush();
            brush1.ImageSource = new BitmapImage(new Uri("Images/1.jpg", UriKind.Relative));
            gridStarvation.Background = brush1;
            starvationMessage = "I won't starve to death.";
        }
        private void CheckDehydratation()
        {
            if (prepDate != null)
            {
                if (prepDate.Any(temp => Double.Parse(temp.Value) <= 1))
                {
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("Images/1.jpg", UriKind.Relative));
                    gridDehydratation.Background = brush;
                    dehydratationMessage = "I won't die from dehydratation.";
                }

                if (prepDate.Any(temp => Double.Parse(temp.Value) > 1 && Double.Parse(temp.Value) <= 2))
                {
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("Images/2.jpg", UriKind.Relative));
                    gridDehydratation.Background = brush;
                    dehydratationMessage = "I probably won't die from dehydratation.";
                }
                if (prepDate.Any(temp => Double.Parse(temp.Value) > 2 && Double.Parse(temp.Value) <= 3))
                {
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("Images/3.jpg", UriKind.Relative));
                    gridDehydratation.Background = brush;
                    dehydratationMessage = "I may die from dehydratation.";
                }
                if (prepDate.Any(temp => Double.Parse(temp.Value) >= 3))
                {
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("Images/4.jpg", UriKind.Relative));
                    gridDehydratation.Background = brush;
                    dehydratationMessage = "I will probably die from dehydratation.";
                }
            }
            else
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/1.jpg", UriKind.Relative));
                gridDehydratation.Background = brush;
            }
        }

        private void CheckFire()
        {
            if (maxTempDate.Any(temp => Double.Parse(temp.Value) <= 20))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/1.jpg", UriKind.Relative));
                gridFire.Background = brush;
                fireMessage = "I won't die in a wild fire.";
            }
            if (maxTempDate.Any(temp => Double.Parse(temp.Value) > 20 && Double.Parse(temp.Value) <= 30))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/2.jpg", UriKind.Relative));
                gridFire.Background = brush;
                fireMessage = "I probably won't die in a wild fire.";
            }
            if (maxTempDate.Any(temp => Double.Parse(temp.Value) > 30 && Double.Parse(temp.Value) <= 50))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/3.jpg", UriKind.Relative));
                gridFire.Background = brush;
                fireMessage = "I may die in a wild fire.";
            }
            if (maxTempDate.Any(temp => Double.Parse(temp.Value) > 50))
            {
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("Images/4.jpg", UriKind.Relative));
                gridFire.Background = brush;
                fireMessage = "I will probably die in a wild fire.";
            }
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = NavigationContext.QueryString;
            if (parameters.ContainsKey("location"))
            {
                location = parameters["location"];
            }
            if (parameters.ContainsKey("year"))
            {
                year = Int32.Parse(parameters["year"]);
            }
            if (parameters.ContainsKey("season"))
            {
                season = parameters["season"];
            }
            LoadRandomReason();
            LoadLocations();
            base.OnNavigatedTo(e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

    

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            //Making sure that the url actually has the access token
            if (!client.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            //Checking that the user successfully accepted our app, otherwise just show the error
            if (oauthResult.IsSuccess)
            {
                //Process result
                client.AccessToken = oauthResult.AccessToken;
                //Hide the browser
                Browser.Visibility = System.Windows.Visibility.Collapsed;
                PostToWall();
            }
            else
            {
                //Process Error
                MessageBox.Show(oauthResult.ErrorDescription);
                Browser.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void PostToWall()
        {
            var parameters = new Dictionary<string, object>();
            parameters["message"] = message;
            client.PostAsync("me/feed", parameters);
        }

        private void gridFreezing_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FreezingChart.xaml?location=" + location + "&season=" + season + "&year=" + year, UriKind.Relative));
        }

        private void gridStarvation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/StarvationChart.xaml?location=" + location + "&season=" + season + "&year=" + year, UriKind.Relative));
        }

        private void gridFire_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FireChart.xaml?location=" + location + "&season=" + season + "&year=" + year, UriKind.Relative));
        }

        private void gridDehydratation_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DehydratationChart.xaml?location=" + location + "&season=" + season + "&year=" + year, UriKind.Relative));
        }
        private void StartPost()
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = FBApi;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";
            //The scope is what give us the access to the users data, in this case
            //we just want to publish on his wall
            parameters["scope"] = "publish_stream";
            Browser.Visibility = System.Windows.Visibility.Visible;
            Browser.Navigate(client.GetLoginUrl(parameters));
        }
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            message = freezinfMessage;
            StartPost();
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            message = starvationMessage;
            StartPost();
        }

        private void Image_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            message = dehydratationMessage;
            StartPost();
        }

        private void Image_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            message = fireMessage;
            StartPost();
        }

        private void Image_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            message = randomMessage;
            StartPost();
        }

        private void LoadRandomReason()
        {
            randomReasons = new List<string>();
            randomReasons.Add("Snake Bite");
            randomReasons.Add("Shark Attack");
            randomReasons.Add("Falling Aircraft");
            randomReasons.Add("Falling Tree");
            randomReasons.Add("Struck by Lightning");
            randomReasons.Add("Hit by an Asteroid");
            randomReasons.Add("Super Volcano Eruption");
            randomReasons.Add("Old Age");
            RandomShow();
        }

        private void RandomShow()
        {
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("Images/1.jpg", UriKind.Relative));
            gridRandom.Background = brush;
            Random random = new Random();
            var index = random.Next(7);
            randomIndex = index;
            var reason = randomReasons[index];
            textBoxRandom.Text = reason;
            randomFacebookButton.Visibility = Visibility.Visible;
            randomMessage = "I won't die from a "+reason+".";
        }

        private void gridRandom_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RandomDetails.xaml?index=" + randomIndex, UriKind.Relative));
        }
    }
}