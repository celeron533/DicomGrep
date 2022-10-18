using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DicomGrep.Views
{
    /// <summary>
    /// Interaction logic for DicomTagLookupView.xaml
    /// </summary>
    public partial class DicomTagLookupView : Window
    {
        public DicomTagLookupView()
        {
            InitializeComponent();
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox filterTextBox = (TextBox)sender;
            string filterText = filterTextBox.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(dataGridUid.ItemsSource);

            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = obj =>
                {
                    DicomTag tag = obj as DicomTag;
                    return tag.ToString().Contains(filterText.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    tag.DictionaryEntry.Keyword.Contains(filterText.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    tag.DictionaryEntry.Name.Contains(filterText.Trim(), StringComparison.OrdinalIgnoreCase);

                };
            }
            else
            {
                cv.Filter = null;
            }
        }

        private void pick_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
