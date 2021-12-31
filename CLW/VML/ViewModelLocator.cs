using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CLW.VML
{
    public class ViewModelLocator
    {


        public static bool GetAutoHookedViewModel(DependencyObject obj)
        {

            return (bool)obj.GetValue(AutoHookedViewModelProperty);
        }

        public static void SetAutoHookedViewModel(DependencyObject obj, bool value)
        {
            Trace.WriteLine("SetAutoHookedViewModel was called");
            obj.SetValue(AutoHookedViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoHookedViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoHookedViewModelProperty =
            DependencyProperty.RegisterAttached("AutoHookedViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, AutoHookedVMChanged));

        private static void AutoHookedVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                //design time

                //main window
                if ((d as FrameworkElement).GetType() == typeof(MainWindow))
                {
                    ((FrameworkElement)d).DataContext = new ViewModel.MainViewModel();
                    Debug.WriteLine("hooked mainViewModel to the mainWindow");
                }

                //dummy watcher
                if ((d as FrameworkElement).GetType() == typeof (CLW.Views.WatcherView) )
                {
                    ((FrameworkElement)d).DataContext = new ViewModel.WatcherViewModel("Dummy Title3", 5, "dummy", true);
                }

                //notification window
                if ((d as FrameworkElement).GetType() == typeof(CLW.Views.NotificationWindowView))
                {
                    ((FrameworkElement)d).DataContext = new ViewModel.NotificationViewModel();
                    Debug.WriteLine("hooked design time NotificationViewModel to notif win");
                }
               

            }
            else
            {

                //real time
                //main window
                Trace.WriteLine("hooked up mainViewModel in runtime");

                if ((d as FrameworkElement).GetType() == typeof(MainWindow))
                {
                    Trace.WriteLine("target type is mainWindow");

                    ((FrameworkElement)d).DataContext = new ViewModel.MainViewModel();
                    Trace.WriteLine(" mainWindow's datacontext was assigned");

                }

            }
            
        }



    }
}
