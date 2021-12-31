using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CLW.Converters
{
    [ValueConversion(typeof(CollectionViewGroup), typeof(string))]
    class CollectionViewGroupToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null && value is CollectionViewGroup)
            {
                CollectionViewGroup g = value as CollectionViewGroup;
                return $"{g.Name}s ({g.ItemCount})";//todo: this targets the list box that shows cookies and headers, make this generic by using parameter at any similar converting scenario
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
