using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeetingWPF
{
    public delegate bool Equality<T>(T left, T right);

    public abstract partial class BaseInpc : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T propertyField, T newValue, Equality<T>? equality = null, [CallerMemberName] string? propertyName = null)
        {
            bool isEquals = propertyField == null || newValue == null
                ? propertyField == null && newValue == null
                : ReferenceEquals(propertyField, newValue) ||
                    (
                        equality != null
                        ? equality(propertyField, newValue)
                        : propertyField is IEquatable<T> equatable
                            ? equatable.Equals(newValue)
                            : propertyField.Equals(newValue)
                    );
            if (!isEquals)
            {
                T oldValue = propertyField;
                propertyField = newValue;
                RaisePropertyChanged(propertyName);

                ProtectedPropertyChanged?.Invoke(propertyName, oldValue, newValue);
            }

            return !isEquals;
        }

        protected static event PropertyChangedHandler? ProtectedPropertyChanged;
    }

    public abstract partial class BaseInpc : INotifyPropertyChanged
    {
        public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

        protected static void RaiseStaticPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        protected static bool StaticSet<T>(ref T propertyField, T newValue, Equality<T>? equality = null, [CallerMemberName] string? propertyName = null)
        {
            bool isEquals = propertyField == null || newValue == null
                ? propertyField == null && newValue == null
                : ReferenceEquals(propertyField, newValue) ||
                    (
                        equality != null
                        ? equality(propertyField, newValue)
                        : propertyField is IEquatable<T> equatable
                            ? equatable.Equals(newValue)
                            : propertyField.Equals(newValue)
                    );

            if (!isEquals)
            {
                T oldValue = propertyField;
                propertyField = newValue;
                RaiseStaticPropertyChanged(propertyName);

                ProtectedStaticPropertyChanged?.Invoke(propertyName, oldValue, newValue);
            }

            return !isEquals;
        }

        protected static event PropertyChangedHandler? ProtectedStaticPropertyChanged;

        protected delegate void PropertyChangedHandler(string? propertyName, object? oldValue, object? newValue);
    }
}
