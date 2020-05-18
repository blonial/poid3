using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace poid.Models
{
    public class NotifyPropertyChangedEvent : INotifyPropertyChanged
    {
        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
