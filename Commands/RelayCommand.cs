using System;
using System.Windows.Input;

namespace poid.Commands
{
    public class RelayCommand : ICommand
    {
        #region Properties

        private readonly Predicate<object> _canExecute;

        private readonly Action<object> _execute;

        #endregion

        #region Default predicate

        private static readonly Predicate<object> _canExecuteDefault = o => true;

        #endregion

        #region Constructors

        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public RelayCommand(Action<object> execute) : this(RelayCommand._canExecuteDefault, execute)
        {

        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}
