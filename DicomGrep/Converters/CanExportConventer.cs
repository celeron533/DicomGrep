using DicomGrep.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DicomGrep.Converters
{
    public class CanExportConventer : IValueConverter
    {
        // allow export the search result when the search is completed
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MainStatusEnum)
            {
                return ((MainStatusEnum)value == MainStatusEnum.Complete);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
