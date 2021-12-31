using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CLW.Views.Attached
{
    public class ButtonProperties
    {




        public static Popup GetTargetPopup(DependencyObject obj)
        {
            return (Popup)obj.GetValue(TargetPopupProperty);
        }

        public static void SetTargetPopup(DependencyObject obj, Popup value)
        {
            obj.SetValue(TargetPopupProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetPopup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetPopupProperty =
            DependencyProperty.RegisterAttached("TargetPopup", typeof(Popup), typeof(ButtonProperties), new PropertyMetadata(null,TargetPropertiesChanged));






        public static bool GetIsOpenPopupAction(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenPopupActionProperty);
        }

        public static void SetIsOpenPopupAction(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenPopupActionProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsOpenPopupAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenPopupActionProperty =
            DependencyProperty.RegisterAttached("IsOpenPopupAction", typeof(bool), typeof(ButtonProperties), new PropertyMetadata(false,IsOpenPopupActionChanged));


        public static bool HasToogledOn(DependencyPropertyChangedEventArgs e)
        {
            return (((e.OldValue as bool?) == null || (e.OldValue as bool?) == false)
                && ((e.NewValue as bool?) == true));
        }
        public static bool HasToogledOff(DependencyPropertyChangedEventArgs e)
        {
            return (((e.NewValue as bool?) == null || (e.NewValue as bool?) == false)
                && ((e.OldValue as bool?) == true));
        }

        private static void IsOpenPopupActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           if(d!=null && d is ButtonBase)
            {
                ButtonBase b = d as ButtonBase;
                if (HasToogledOn(e))
                {
                    b.Command = new MICommand(() =>
                    {
                        var popup = GetTargetPopup(b);
                        if (popup != null)
                            popup.IsOpen = true;
                    });
                }
                else if (HasToogledOff(e))
                {
                    b.Command = null;
                }
            }
        }

        private static void TargetPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
