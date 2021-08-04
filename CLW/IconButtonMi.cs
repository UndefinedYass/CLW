using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CLW
{
    class IconButtonMi: Button
    {


        public override void OnApplyTemplate()
        {
            
        }




        public Brush hoverBgColor
        {
            get { return (Brush)GetValue(hoverBgColorProperty); }
            set { SetValue(hoverBgColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty hoverBgColorProperty =
            DependencyProperty.Register("hoverBgColor", typeof(Brush), typeof(IconButtonMi), new PropertyMetadata(null));




    }
}
