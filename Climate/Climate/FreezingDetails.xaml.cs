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

namespace Climate
{
    public partial class FreezingDetails : PhoneApplicationPage
    {
        public FreezingDetails()
        {
            InitializeComponent();
        }

        string location;
        int year;
        string season;
     
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
            base.OnNavigatedTo(e);
        }
        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FreezingChart.xaml?location=" + location + "&season=" + season + "&year=" + year, UriKind.Relative));
        }
    }
}