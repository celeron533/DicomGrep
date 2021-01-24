﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DicomGrep.ViewModels
{
    // https://www.technical-recipes.com/2016/using-relaycommand-icommand-to-handle-events-in-wpf-and-mvvm/
    // with necessary modifications
    public class RelayCommand<T> : ICommand
    {
        #region Fields 
        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;
        #endregion // Fields 

        #region Constructors 
        public RelayCommand(Action<T> execute) : this(execute, null) { }
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 


        #region ICommand Members 
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter) { _execute((T)parameter); }
        #endregion // ICommand Members 
    }
}
