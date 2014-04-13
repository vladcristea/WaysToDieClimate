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
using Visifire.Charts;

namespace Climate
{
    public partial class FreezingChart : PhoneApplicationPage
    {
        public FreezingChart()
        {
            InitializeComponent();
        }
        string location;
        int year;
        string season;
        double latitude;
        double longitute;
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
            LoadLocations();
            base.OnNavigatedTo(e);
        }

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
            JSONCall();
        }

        private void JSONCall()
        {
            WebClient webClient = new WebClient();
            var cell_index_y = (int)((49.9375 - latitude) / 0.0083333333);
            var cell_index_x = (int)((longitute - (-125.020833335)) / 0.0083333333);
           
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                webClient.DownloadStringAsync(new Uri("http://opennexapp.s3.amazonaws.com/cesm1-cam5/rcp85r1i1p1/tmin/" + cell_index_x + "/" + cell_index_y));
        }
        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var s = e.Result.Split('[');
            var res = s[1].Split(']');
            var results = res[0].Split(',');
            DateTime date = new DateTime(1950, 01, 15);
            List<TemperatureDate> tempDate = new List<TemperatureDate>();
            foreach (var temp in results)
            {
                tempDate.Add(new TemperatureDate() { Date = date, Value = temp });
                date = date.AddMonths(1);
            }
            int month;
            if (season == "Summer")
            { month = 7; }
            else
            {
                month = 1;
            }
            var maxTempDate = tempDate.Where(temp => temp.Date.Month == month).ToList();

            DataSeries dataSeries = new DataSeries();
            dataSeries.RenderAs = RenderAs.Line;
            DataPoint dataPoint;

            for (int i = 0; i < maxTempDate.Count; i++)
            {
                // Create a new instance of DataPoint
                dataPoint = new DataPoint();
                // Set YValue for a DataPoint
                dataPoint.YValue = double.Parse(maxTempDate[i].Value);
                dataPoint.AxisXLabel = maxTempDate[i].Date.Year.ToString();
                // Add dataPoint to DataPoints collection
                dataSeries.DataPoints.Add(dataPoint);
            }

            chart.Series.Add(dataSeries);

        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FreezingDetails.xaml?location=" + location + "&season=" + season + "&year=" + year, UriKind.Relative));
        }
    }
}