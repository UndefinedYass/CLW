using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLW.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {

       

        public event PropertyChangedEventHandler PropertyChanged;

        internal void notif(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
