using System.Windows;

namespace poid.Models
{
    public static class Notify
    {
        #region Methods

        public static MessageBoxResult Error(string message)
        {
            return Error(message, "Error", MessageBoxButton.OK);
        }

        public static MessageBoxResult Error(string message, string header)
        {
            return Error(message, header, MessageBoxButton.OK);
        }

        public static MessageBoxResult Error(string message, MessageBoxButton button)
        {
            return Error(message, "Error", button);
        }

        public static MessageBoxResult Error(string message, string header, MessageBoxButton button)
        {
            return MessageBox.Show(message, header, button, MessageBoxImage.Error);
        }

        public static MessageBoxResult Info(string message)
        {
            return Info(message, "Info", MessageBoxButton.OK);
        }

        public static MessageBoxResult Info(string message, string header)
        {
            return Info(message, header, MessageBoxButton.OK);
        }

        public static MessageBoxResult Info(string message, MessageBoxButton button)
        {
            return Info(message, "Info", button);
        }

        public static MessageBoxResult Info(string message, string header, MessageBoxButton button)
        {
            return MessageBox.Show(message, header, button, MessageBoxImage.Information);
        }

        #endregion
    }
}
