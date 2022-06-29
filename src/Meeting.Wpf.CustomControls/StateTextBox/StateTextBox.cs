using Framework.Delegates;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;

namespace Meeting.Wpf.CustomControls
{
    /// <summary>
    /// This converter are extension of EnumConverter for StatusEnum. Is handling <see cref="bool"/> and <see cref="Nullable{T}">Nullable&lt;bool&gt;</see> values.
    /// </summary>
    /// <remarks> <see langword="null"/> - <see cref="StatusEnum.Empty"/>; <br/>
    /// <see langword="true"/> - <see cref="StatusEnum.Success"/>; <br/> 
    /// <see langword="false"/> - <see cref="StatusEnum.Fail"/>;</remarks>
    public class StatusEnumConverter : EnumConverter
    {
        public StatusEnumConverter()
            : base(typeof(StatusEnum))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(bool) || sourceType == typeof(bool?))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            if (destinationType == typeof(bool) || destinationType == typeof(bool?))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            Type? type = value?.GetType();

            if (type == typeof(bool) || type == null || type == typeof(bool?))
            {
                return value is null ? StatusEnum.Empty : (bool)value ? StatusEnum.Success : StatusEnum.Fail;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            StatusEnum valueStatusEnum = (StatusEnum)(value ?? throw new ArgumentNullException(nameof(value)));

            if (destinationType == typeof(bool))
            {
                return valueStatusEnum switch
                {
                    StatusEnum.Success => true,
                    _ => false
                };
            }
            if (destinationType == typeof(bool?))
            {
                return valueStatusEnum switch
                {
                    StatusEnum.Success => true,
                    StatusEnum.Fail => false,
                    _ => null
                };
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool IsValid(ITypeDescriptorContext? context, object? value)
        {
            if (value is bool? || value is bool || value is null)
            {
                return true;
            }

            return base.IsValid(context, value);
        }
    }

    [TypeConverter(typeof(StatusEnumConverter))]
    public enum StatusEnum
    {
        Empty,
        Success,
        Fail
    }

    public class StateEmptiedEventArgs : EventArgs { }
    public class StateSuccessedEventArgs : EventArgs { }
    public class StateFailedEventArgs : EventArgs { }

    public class StateTextBox : PlaceholderTextBox
    {
        private SerialDisposable _eventSubscriptions = new SerialDisposable();

        private const string ELEMENT_STATUS = "PART_Status";

        static StateTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StateTextBox),
                new FrameworkPropertyMetadata(typeof(StateTextBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _eventSubscriptions.Disposable = null;
            CompositeDisposable disposable = new CompositeDisposable();
            FrameworkElement? statusElement = GetTemplateChild(ELEMENT_STATUS) as FrameworkElement;
            if (statusElement != null)
            {
                statusElement.MouseUp += OnStatusElementMouseUp;
                disposable.Add(Disposable.Create(delegate
                {
                    statusElement.MouseUp -= OnStatusElementMouseUp;
                }));
            }
            _eventSubscriptions.Disposable = disposable;
        }

        private void OnStatusElementMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Command != null)
            {
                if (Command.CanExecute(CommandParameter))
                    Command.Execute(CommandParameter);
            }
        }

        #region DependencyProperty : Status

        public StatusEnum Status
        {
            get => (StatusEnum)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(StatusEnum),
                typeof(StateTextBox), new FrameworkPropertyMetadata(StatusEnum.Empty));

        #endregion

        #region DependencyProperty : CommandProperty

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand),
                typeof(StateTextBox), new PropertyMetadata(default));

        #endregion

        #region DependencyProperty : CommandParameterProperty

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object),
                typeof(StateTextBox), new PropertyMetadata(default));

        #endregion

        #region DependencyProperty : CornerRadius

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius),
                typeof(StateTextBox), new PropertyMetadata(default));

        #endregion

        public event TypedEventHandler<StateTextBox, StateEmptiedEventArgs>? Emptied;

        public event TypedEventHandler<StateTextBox, StateSuccessedEventArgs>? Successed;

        public event TypedEventHandler<StateTextBox, StateFailedEventArgs>? Failed;

        protected void RaiseStatusEmptiedEvent(StateTextBox container)
        {
            this.Emptied?.Invoke(this, new StateEmptiedEventArgs());
        }

        protected void RaiseStatusSuccessedEvent(StateTextBox container)
        {
            this.Successed?.Invoke(this, new StateSuccessedEventArgs());
        }

        protected void RaiseStatusFailedEvent(StateTextBox container)
        {
            this.Failed?.Invoke(this, new StateFailedEventArgs());
        }
    }
}
