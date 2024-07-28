using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DicomGrep.ViewModels
{
    // https://stackoverflow.com/questions/36149863/how-to-write-a-viewmodelbase-in-mvvm/36151255#36151255

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void InvalidateRequerySuggested()
        {
            Application.Current?.Dispatcher.BeginInvoke((Action)CommandManager.InvalidateRequerySuggested);
        }

        // service helper
        protected static T GetService<T>() where T : class
        {
            return (T)App.Current.Services.GetService(typeof(T));
        }
    }
}
