using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CLW.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindowView.xaml
    /// </summary>
    public partial class NotificationWindowView : Window
    {
        public NotificationWindowView()
        {
            InitializeComponent();
        }

        //copied code from old clw notification window
        private void animateSlideUp()
        {

            Left = SystemParameters.PrimaryScreenWidth - Width - 2;

            double H = SystemParameters.PrimaryScreenHeight;

            Storyboard sb = new Storyboard();
            sb.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            DoubleAnimation ra = new DoubleAnimation() { From = H - Height, To = H - Height - 39, Duration = sb.Duration };
            ra.EasingFunction = new ElasticEase() { Oscillations = 1, Springiness = 0.05 };
            DoubleAnimation OpacityAnim = new DoubleAnimation() { From = 0, To = 0.92, Duration = sb.Duration };
            Storyboard.SetTarget(ra, this);
            Storyboard.SetTargetProperty(ra, new PropertyPath(Window.TopProperty));
            Storyboard.SetTarget(OpacityAnim, this);
            Storyboard.SetTargetProperty(OpacityAnim, new PropertyPath(Window.OpacityProperty));
            sb.Children.Add(ra);
            sb.Children.Add(OpacityAnim);
            sb.Begin();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            animateSlideUp();
        }
    }
}
