using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CLW.Views.Attached
{
    public class BrowsePathCommand : DependencyObject
    {



        public static TextBox GetTargetTextBox(DependencyObject obj)
        {
            return (TextBox)obj.GetValue(TargetTextBoxProperty);
        }

        public static void SetTargetTextBox(DependencyObject obj, TextBox value)
        {
            obj.SetValue(TargetTextBoxProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetTextBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetTextBoxProperty =
            DependencyProperty.RegisterAttached("TargetTextBox", typeof(TextBox), typeof(BrowsePathCommand), new PropertyMetadata(null, OnTargetTextBoxChanged));



        

        public enum BrowsePathOperationType { file,directory}





        public static BrowsePathOperationType GetOperationType(DependencyObject obj)
        {
            return (BrowsePathOperationType)obj.GetValue(OperationTypeProperty);
        }

        public static void SetOperationType(DependencyObject obj, BrowsePathOperationType value)
        {
            obj.SetValue(OperationTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for OperationType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OperationTypeProperty =
            DependencyProperty.RegisterAttached("OperationType", typeof(BrowsePathOperationType), typeof(BrowsePathCommand), new PropertyMetadata(BrowsePathOperationType.file, OnOperationTypeChanged));

        private static void OnOperationTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void OnTargetTextBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null && e.NewValue.GetType() == typeof(TextBox))
            {
                Button butt = d as Button;
                TextBox targetTB = e.NewValue as TextBox;
                butt.Command = new MICommand(() => {
                    if(BrowsePathCommand.GetOperationType(butt)== BrowsePathOperationType.file)
                    {
                        Services.Utils.PickFileAsync((res) =>
                        {
                            targetTB.Dispatcher.Invoke(() =>
                            {
                                if (res.Success)
                                {
                                    targetTB.Text = res.selectedPath;
                                }
                            });
                        });
                    }
                    else 
                    {
                        Services.Utils.PickFolderAsync((res) =>
                        {
                            targetTB.Dispatcher.Invoke(() =>
                            {
                                if (res.Success)
                                {
                                    targetTB.Text = res.selectedPath;
                                }
                            });
                        });
                    }
                    
                });
            }
            
        }
    }






}
