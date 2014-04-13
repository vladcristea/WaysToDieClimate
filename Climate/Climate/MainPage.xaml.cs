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
using System.IO;



namespace Climate
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            List<string> years = new List<string>();
            for (var i = 1950; i <= 2100; i++)
            {
                years.Add(i.ToString());
            }
            yearComboBox.ItemsSource = years;
            List<string> seasons = new List<string>();
            seasons.Add("Summer");
            seasons.Add("Winter");
            seasonComboBox.ItemsSource = seasons;
        }
       

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Temperature.xaml?location="+LocationTextBox.Text+"&season="+seasonComboBox.SelectedItem+"&year="+yearComboBox.SelectedItem,UriKind.Relative));
            //   LoadLocations(LocationTextBox.Text);
        }
      
    }
}