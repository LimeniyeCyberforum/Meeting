using Framework.Delegates;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;

namespace Meeting.Wpf.Controls
{
    public enum StatusEnum
    {
        Empty,
        Success,
        Fail
    }

    public class StateEmptiedEventArgs : EventArgs { }
    public class StateSuccessedEventArgs : EventArgs { }
    public class StateFailedEventArgs : EventArgs { }

    public class StateTextBox : PlaceholderTextBox.PlaceholderTextBox
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
                typeof(StateTextBox), new FrameworkPropertyMetadata(StatusEnum.Empty, delegate (DependencyObject s, DependencyPropertyChangedEventArgs e)
                {
                    (s as StateTextBox)?.OnStatusPropertyChanged(e);
                }));

        #endregion

        #region DependencyProperty : IsValid

        public bool? IsValid
        {
            get => (bool?)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(nameof(IsValid), typeof(bool?),
                typeof(StateTextBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
                    {
                        (s as StateTextBox)?.OnIsValidPropertyChanged(e);
                    }));

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

        protected virtual void OnStatusPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var newValue = (StatusEnum)e.NewValue;
            var oldValue = (StatusEnum)e.OldValue;

            if (newValue != oldValue)
            {
                if (newValue == StatusEnum.Success)
                {
                    IsValid = true;
                }
                else if (newValue == StatusEnum.Fail)
                {
                    IsValid = false;
                }
                else
                {
                    IsValid = null;
                }
            }
        }

        protected virtual void OnIsValidPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool?)e.NewValue;
            var oldValue = (bool?)e.OldValue;

            if (newValue != oldValue)
            {
                if (newValue == true)
                {
                    Status = StatusEnum.Success;
                }
                else if (newValue == false)
                {
                    Status = StatusEnum.Fail;
                }
                else
                {
                    Status = StatusEnum.Empty;
                }
            }
        }

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
