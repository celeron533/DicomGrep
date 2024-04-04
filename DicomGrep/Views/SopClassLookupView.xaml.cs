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
using System.Windows.Threading;

namespace DicomGrep.Views
{
    /// <summary>
    /// Interaction logic for SopClassLookupView.xaml
    /// </summary>
    public partial class SopClassLookupView : Window
    {
        // delayed search for better performance
        private DispatcherTimer timer = new DispatcherTimer();

        public SopClassLookupView()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            TextBox filterTextBox = filter;
            string filterText = filterTextBox.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(dataGridUids.ItemsSource);

            if (!string.IsNullOrEmpty(filterText))
            {
                cv.Filter = obj =>
                {
                    DicomUID uid = obj as DicomUID;
                    return uid.UID.Contains(filterText.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    uid.Name.Contains(filterText.Trim(), StringComparison.OrdinalIgnoreCase);
                };
            }
            else
            {
                cv.Filter = null;
            }
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // reset the timer
            timer.Stop();
            timer.Start();
        }

        private void pick_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void dataGridUid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}