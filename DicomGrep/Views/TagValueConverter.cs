using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DicomGrep.Views
{
    [ValueConversion(typeof(DicomTag), typeof(string))]
    public class TagValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DicomTag dicomTag)
            {
                string para = "X";
                if (parameter is string _p && !string.IsNullOrWhiteSpace(_p))
                {
                    para = (string)parameter;
                }

                if (para == "C")
                {
                    if (dicomTag.PrivateCreator == null)
                    {
                        return $"({dicomTag.Group:x4},{dicomTag.Element:x4})";
                    }
                    return $"({dicomTag.Group:x4},xx{dicomTag.Element & 0xFF:x2})";
                }
                else
                {
                    return dicomTag.ToString(para, null);
                }
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
