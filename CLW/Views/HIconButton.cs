using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CLW
{
    class HIconButton:Button
    {

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (Content != null)
            {
                CONTENT_ICO = (PackIconControlBase)Content;
                CONTENT_ICO.Foreground = Foreground;
            }
        }

        


        public PackIconControlBase PART_ICO { get; set; }

        public PackIconControlBase CONTENT_ICO { get; set; }


        public override void OnApplyTemplate()
        {
           //PART_ICO = (PackIconControlBase) GetTemplateChild("PART_ICON");
        }






        public Brush HoverColor
        {
            get { return (Brush)GetValue(HoverColorProperty); }
            set { SetValue(HoverColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoverColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverColorProperty =
            DependencyProperty.Register("HoverColor", typeof(Brush), typeof(HIconButton), new PropertyMetadata(null));



        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            //if (CONTENT_ICO != null)
                CONTENT_ICO.Foreground = HoverColor;

        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (CONTENT_ICO != null)
                CONTENT_ICO.Foreground = Foreground;
        }



    }
}
