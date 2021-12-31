using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace CLW.Views.Attached
{
    class ListBoxProps : DependencyObject
    {




        public static ICommand GetAddButtonCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(AddButtonCommandProperty);
        }

        public static void SetAddButtonCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(AddButtonCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for AddButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonCommandProperty =
            DependencyProperty.RegisterAttached("AddButtonCommand", typeof(ICommand), typeof(ListBoxProps), new PropertyMetadata(null,OnAddButtonCommandChanged));

        private static void OnAddButtonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d.GetType()!=typeof(ListBox) || e.NewValue==null || (e.NewValue as ICommand)== null)
            {
                return;
            }
            ListBox lb = d as ListBox;
            lb.SourceUpdated += (s, ee) =>
            {
                if(ee.Property == ListBox.ItemsSourceProperty)
                {
                    lb.SelectedIndex = lb.Items.Count - 1;
                   
                }
            };

        }

















        public static bool GetScrollToAddedItem(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollToAddedItemProperty);
        }

        public static void SetScrollToAddedItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollToAddedItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollToAddedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToAddedItemProperty =
            DependencyProperty.RegisterAttached("ScrollToAddedItem", typeof(bool), typeof(ListBoxProps), new PropertyMetadata(false, OnScrollToAddedItemChanged));

        private static void OnScrollToAddedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ButtonProperties.HasToogledOn(e))
            {
                ListBox lb = d as ListBox;
                CollectionViewSource cvs = lb.ItemsSource as CollectionViewSource;
                
                lb.SourceUpdated  += handleSourceUpdated;
            }
            else if (ButtonProperties.HasToogledOff(e))
            {
                ListBox lb = d as ListBox;
                lb.SourceUpdated -= handleSourceUpdated;
            }
        }

        private static void handleSourceUpdated(object sender, DataTransferEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("updated");
           if(e.Property == ListBox.ItemsSourceProperty)
            {
                System.Diagnostics.Trace.WriteLine("updated ItemsSourceProperty");
                ListBox lstBox = sender as ListBox;
                lstBox.SelectedIndex = lstBox.Items.Count - 1;
                lstBox.ScrollIntoView(lstBox.SelectedItem);
            }
        }
    }
}
