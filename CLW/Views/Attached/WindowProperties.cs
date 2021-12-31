using CLW.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CLW.Views.Attached
{
    public class WindowProperties
    {


        public static bool GetCloseOnCloseRequest(DependencyObject obj)
        {
            return (bool)obj.GetValue(CloseOnCloseRequestProperty);
        }

        public static void SetCloseOnCloseRequest(DependencyObject obj, bool value)
        {
            obj.SetValue(CloseOnCloseRequestProperty, value);
        }

        // Using a DependencyProperty as the backing store for CloseOnCloseRequest.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseOnCloseRequestProperty =
            DependencyProperty.RegisterAttached("CloseOnCloseRequest", typeof(bool), typeof(WindowProperties), new PropertyMetadata(false,CloseOnCloseRequestChanged));






        public static ViewModel.ICloseWindowViewModel GetViewModel(DependencyObject obj)
        {
            return (ViewModel.ICloseWindowViewModel)obj.GetValue(ViewModelProperty);
        }

        public static void SetViewModel(DependencyObject obj, ViewModel.ICloseWindowViewModel value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached("ViewModel", typeof(ViewModel.ICloseWindowViewModel), typeof(WindowProperties), new PropertyMetadata(null,ViewModelChanged));

        private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is Window)
            {
                if (e.OldValue == null && (e.NewValue as ICloseWindowViewModel) != null)
                {
                    var vm = e.NewValue as ViewModel.ICloseWindowViewModel;
                    vm.TargetWindow = d as Window;
                    vm.CloseRequest += handleCloseRequest;
                }
                else if (e.NewValue == null && (e.OldValue as ICloseWindowViewModel) != null)
                {
                    var old_vm = e.OldValue as ViewModel.ICloseWindowViewModel;
                    old_vm.TargetWindow = null;
                    old_vm.CloseRequest -= handleCloseRequest;
                }
            }
        }

        private static void CloseOnCloseRequestChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void handleCloseRequest(object sender, EventArgs e)
        {
            if(GetCloseOnCloseRequest((sender as ICloseWindowViewModel).TargetWindow))
            (sender as ViewModel.ICloseWindowViewModel)?.TargetWindow?.Close();
        }
    }
}
