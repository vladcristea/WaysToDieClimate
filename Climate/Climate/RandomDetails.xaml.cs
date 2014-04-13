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
using System.Windows.Media.Imaging;

namespace Climate
{
    public partial class RandomDetails : PhoneApplicationPage
    {
        List<RandomReason> reasons = new List<RandomReason>();
        public RandomDetails()
        {
            InitializeComponent();
        }
        int index;
      
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = NavigationContext.QueryString;
            if (parameters.ContainsKey("index"))
            {
                index = Int32.Parse(parameters["index"]);
            }
            LoadRandomDescriptions();
            base.OnNavigatedTo(e);
        }
        private void LoadRandomDescriptions()
        {
            reasons.Add(new RandomReason()
            {
                Name = "Snake Bite",
                ImageUrl = "Images/snake.jpg",
                Description = "Lucky you! Snake bites kill fast and may include some fun effects before you are leaving the planet, such as feelings of joy and happiness. Enjoy the last ride!"
            });
            reasons.Add(new RandomReason()
            {
                Name = "Shark Attack",
                ImageUrl = "Images/shark.jpg",
                Description = "Hmm, your flesh must have a fantastic scent to attract sharks as they usually prefer seals. In that case, if you manage to escape the sharks, also be aware of cannibals."
            });
            reasons.Add(new RandomReason()
            {
                Name = "Falling Aircraft",
                ImageUrl = "Images/aircraft.jpg",
                Description = "You should check the beautiful sky more often. It is full of surprises. Duck!"
            });
            reasons.Add(new RandomReason()
            {
                Name = "Falling Tree",
                ImageUrl = "Images/tree.jpg",
                Description = "Never been much of a tree hugger? You should seize the opportunity to go after them before they come after you. …"
            });
            reasons.Add(new RandomReason()
            {
                Name = "Struck by Lightning",
                ImageUrl = "Images/lighting.jpg",
                Description = "If you’re running low on energy, this might be just right for you. Shine bright like a lightning."
            });
            reasons.Add(new RandomReason()
            {
                Name = "Hit by an Asteroid",
                ImageUrl = "Images/asteroid.jpg",
                Description = "You always wanted a souvenir from space but your astronaut aunt never brought you one? Here comes your asteroid, that one you’ll never forget."
            });
            reasons.Add(new RandomReason()
            {
                Name = "Super Volcano Eruption",
                ImageUrl = "Images/volcano2.jpg",
                Description = "Cold on your feet and in need of a warm blanket? This one will melt you down."
            });
            reasons.Add(new RandomReason()
            {
                Name = "Old Age",
                ImageUrl = "Images/old_age.jpg",
                Description = "What a boring way to die…"
            });
            LoadContent();
        }

        private void LoadContent()
        {
            var el = reasons[index];
            textblockR.Text = el.Description;
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(el.ImageUrl, UriKind.Relative));
            LayoutRoot.Background = brush;
        }
    }
}