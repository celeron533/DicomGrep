using DicomGrep.Views;
using DicomGrepCore.Entities;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DicomGrep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _startDragPoint;
        private ResultDicomFile _draggedFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            new AboutView().ShowDialog();
        }

        private void DataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedFile == null)
            {
                return;
            }

            Point currentPosition = e.GetPosition(null);
            Vector dragDelta = currentPosition - _startDragPoint;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(dragDelta.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(dragDelta.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                string[] filePaths = { _draggedFile.FullFilename };
                DataObject dataObject = new DataObject(DataFormats.FileDrop, filePaths);

                DragDrop.DoDragDrop((DataGrid)sender, dataObject, DragDropEffects.Copy);

                _draggedFile = null;
            }
        }

        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startDragPoint = e.GetPosition(null);
            var dataGrid = sender as DataGrid;
            var row = FindAncestor<DataGridRow>((DependencyObject)e.OriginalSource);
            if (row != null)
            {
                _draggedFile = row.Item as ResultDicomFile;
            }
        }

        private T FindAncestor<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindAncestor<T>(parent);
        }
    }
}
