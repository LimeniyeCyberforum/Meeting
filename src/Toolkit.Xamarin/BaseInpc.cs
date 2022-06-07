using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Toolkit.Xamarin
{
    public abstract class BaseInpc : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void OnPropertyChanged(IEnumerable<string> propList)
        {
            foreach (string propertyName in propList.Where(name => !string.IsNullOrWhiteSpace(name)))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(params string[] propList)
        {
            foreach (string propertyName in propList.Where(name => !string.IsNullOrWhiteSpace(name)))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnAllPropertyChanged()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
             => PropertyChanged?.Invoke(this, e);

        protected virtual void Set<T>(ref T fieldProperty, T newValue, [CallerMemberName] string propertyName = "")
        {
            if ((fieldProperty != null && !fieldProperty.Equals(newValue)) || (fieldProperty == null && newValue != null))
                PropertyNewValue(ref fieldProperty, newValue, propertyName);
        }

        protected virtual void PropertyNewValue<T>(ref T fieldProperty, T newValue, string propertyName)
        {
            fieldProperty = newValue;
            OnPropertyChanged(propertyName);
        }
    }
}
