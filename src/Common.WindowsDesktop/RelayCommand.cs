using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MvvmCommon.WindowsDesktop
{
    #region Delegates for WPF Command Methods
    public delegate void ExecuteHandler();
    public delegate bool CanExecuteHandler();

    public delegate void ExecuteHandler<T>(T parameter);
    public delegate bool CanExecuteHandler<T>(T parameter);

    public delegate bool ConverterFromObjectHandler<T>(in object value, out T result);
    #endregion

    /// <summary> A class that implements <see cref = "ICommand" />. <br/>
    /// Implementation taken from <see href="https://www.cyberforum.ru/wpf-silverlight/thread2390714-page4.html#post13535649"/>
    /// and added a constructor for methods without a parameter.</summary>
    public class RelayCommand : ICommand
    {
        protected readonly CanExecuteHandler<object> canExecute;
        protected readonly ExecuteHandler<object> execute;
        private readonly EventHandler requerySuggested;

        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        public event EventHandler? CanExecuteChanged;

        /// <summary> Command constructor. </summary>
        /// <param name = "execute"> Command method to execute. </param>
        /// <param name = "canExecute"> Method that returns the state of the command. </param>
        public RelayCommand(ExecuteHandler<object> execute, CanExecuteHandler<object> canExecute = null)
           : this()
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object}, CanExecuteHandler{object})"/>
        public RelayCommand(ExecuteHandler execute, CanExecuteHandler? canExecute = null)
                : this
                (
                      p => execute(),
                      p => canExecute?.Invoke() ?? true
                )
        { }

        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;

        /// <summary> The method that raises the event <see cref="CanExecuteChanged"/>.</summary>
        public void RaiseCanExecuteChanged()
        {
            if (dispatcher.CheckAccess())
            {
                invalidate();
            }
            else
            {
                _ = dispatcher.BeginInvoke(invalidate);
            }
        }
        private readonly Action invalidate;
        private RelayCommand()
        {
            invalidate = () => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            requerySuggested = (o, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            CommandManager.RequerySuggested += requerySuggested;
        }

        /// <inheritdoc cref="ICommand.CanExecute(object)"/>
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc cref="ICommand.Execute(object)"/>
        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }
    }
    /// <summary>  implementation for generic parameter methods. </summary>
    /// <typeparam name = "T"> Method parameter type. </typeparam>  
    public class RelayCommand<T> : RelayCommand
    {
        /// <summary> Command constructor. </summary>
        /// <param name = "execute"> Command method to execute. </param>
        /// <param name = "canExecute"> Method that returns the state of the command. </param>
        /// <param name="converter">Optional converter to convert <see cref="object"/> to <typeparamref name="T"/>. <br/>
        /// It is called when the parameter
        /// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/is">
        /// is not compatible</see> with a <typeparamref name="T"/> type.
        /// </param>
        public RelayCommand(ExecuteHandler<T> execute, CanExecuteHandler<T> canExecute, ConverterFromObjectHandler<T>? converter = null)
            : base
            (
                  p =>
                  {
                      if (p is T t ||
                        (converter != null && converter(p, out t)))
                      {
                          execute(t);
                      }
                  },
                  p => ((p is T t) || (converter != null && converter(p, out t))) &&
                        (canExecute?.Invoke(t) ?? true)
            )
        { }

        /// <summary> Command constructor. </summary>
        /// <param name = "execute"> Command method to execute. </param>
        /// <param name="converter">Optional converter to convert <see cref="object"/> to <typeparamref name="T"/>.</param>
        public RelayCommand(ExecuteHandler<T> execute, ConverterFromObjectHandler<T> converter = null)
           : this(execute, null, converter)
        { }
    }

    public class RelayCommandAsync : RelayCommand, ICommand, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <inheritdoc cref="INotifyDataErrorInfo.ErrorsChanged"/>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>The command is in the execution state of the <see cref="RelayCommand.Execute(object)"/> method.</summary>
        public bool IsBusy { get; private set; }

        /// <inheritdoc cref="INotifyDataErrorInfo.HasErrors"/>
        public bool HasErrors { get; private set; }

        /// <summary>Exception from the last execution of the <see cref="RelayCommand.Execute(object)"/> method.</summary>
        public Exception ExecuteException { get; private set; }

        // A flag indicating a "call to execute busy a command" error.
        private bool isBusyExecuteError;

        /// <summary>Sets a value to the <see cref="IsBisy"/> property and notifies of its change.</summary>
        /// <param name="isBusy">The value for the property.</param>
        protected void SetIsBusy(bool isBusy)
        {
            if (IsBusy != isBusy)
            {
                IsBusy = isBusy;
                PropertyChanged?.Invoke(this, Args.IsBusyPropertyEventArgs);
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>Sets the HasErrors property and reports an entity-level error.</summary>
        /// <param name="hasErrors">The value for the property.</param>
        protected void SetEntityHasErrors(bool hasErrors)
              => SetHasErrors(hasErrors, Args.EntityLevelErrorsEventArgs);

        /// <summary>Sets the HasErrors property and reports an entity or property level error.</summary>
        /// <param name="hasErrors">The value for the property.</param>
        /// <param name="args">Argument with data about the error level.</param>
        protected void SetHasErrors(bool hasErrors, DataErrorsChangedEventArgs args)
        {
            if (HasErrors != hasErrors)
            {
                HasErrors = hasErrors;
                PropertyChanged?.Invoke(this, Args.HasErrorsPropertyEventArgs);
            }
            ErrorsChanged?.Invoke(this, args);
        }


        /// <summary>Sets a value to the <see cref="ExecuteException"/> property and notifies of its change.</summary>
        /// <param name="exception">The value for the property.</param>
        protected void SetExecuteException(Exception exception)
        {
            if (ExecuteException != exception)
            {
                ExecuteException = exception;
                PropertyChanged?.Invoke(this, Args.ExecuteExceptionPropertyEventArgs);
            }
        }

        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object}, CanExecuteHandler{object})"/>
        public RelayCommandAsync(ExecuteHandler<object> execute, CanExecuteHandler<object>? canExecute = null)
            : this(new AsyncData(execute, canExecute))
        { }

        /// <inheritdoc cref="RelayCommand(ExecuteHandler, CanExecuteHandler)"/>
        public RelayCommandAsync(ExecuteHandler execute, CanExecuteHandler? canExecute = null)
            : this(new AsyncData(execute, canExecute))
        { }

        // The field for storing additional, auxiliary data generated
        // during the generation of the asynchronous method, wrapping
        // the one obtained in the constructor.
        private readonly AsyncData data;

        /// <inheritdoc cref="RelayCommand(ExecuteHandler{object}, CanExecuteHandler{object})"/>
        protected RelayCommandAsync(AsyncData data)
            : base(data.ExecuteAsync, data.CanExecuteAsync)
        {
            this.data = data;
            this.data.commandAsync = this;
        }

        /// <inheritdoc cref="INotifyDataErrorInfo.GetErrors(string)"/>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                if (isBusyExecuteError)
                {
                    yield return Args.BusyExecuteErrorMessage;
                }

                if (ExecuteException != null)
                {
                    yield return ExecuteException;
                }
            }
            IEnumerable errors = GetErrorsOverride(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    yield return error;
                }
            }
        }

        /// <summary>Method overridden in derived classes to add error information</summary>
        /// <param name="propertyName">The name of the property to retrieve validation
        /// errors for; or null or Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        protected virtual IEnumerable GetErrorsOverride(string propertyName)
            => null;

        /// <summary>A class with persistent elements to avoid re-creating them frequently.</summary>
        public static class Args
        {
            public const string BusyExecuteErrorMessage = "Called the execution of a command when it is busy.";

            public static readonly PropertyChangedEventArgs IsBusyPropertyEventArgs = new PropertyChangedEventArgs(nameof(IsBusy));
            public static readonly PropertyChangedEventArgs HasErrorsPropertyEventArgs = new PropertyChangedEventArgs(nameof(HasErrors));
            public static readonly PropertyChangedEventArgs ExecuteExceptionPropertyEventArgs = new PropertyChangedEventArgs(nameof(ExecuteException));

            public static readonly DataErrorsChangedEventArgs EntityLevelErrorsEventArgs = new DataErrorsChangedEventArgs(string.Empty);
        }

        /// <summary>A class for storing additional, auxiliary data and methods that are generated
        /// when generating asynchronous methods that wrap the synchronous methods received
        /// in the constructor.</summary>
        protected class AsyncData
        {
            public RelayCommandAsync commandAsync;
            public async void ExecuteAsync(object parameter)
            {
                if (commandAsync.IsBusy)
                {
                    commandAsync.isBusyExecuteError = true;
                    commandAsync.SetEntityHasErrors(true);
                }
                else
                {
                    commandAsync.SetIsBusy(true);

                    try
                    {
                        await Task.Run(() => execute(parameter));

                        commandAsync.isBusyExecuteError = false;
                        commandAsync.SetExecuteException(null);
                        commandAsync.SetEntityHasErrors(false);
                    }
                    catch (Exception ex)
                    {
                        commandAsync.SetExecuteException(ex);
                        commandAsync.SetEntityHasErrors(true);
                    }
                    finally
                    {
                        commandAsync.SetIsBusy(false);
                    }
                }
            }

            public CanExecuteHandler<object> CanExecuteAsync { get; }
            private bool canExecuteNullAsync(object parameter) => !commandAsync.IsBusy;
            private bool canExecuteAsync(object parameter) => !commandAsync.IsBusy && canExecute(parameter);

            private readonly ExecuteHandler<object> execute;
            private readonly CanExecuteHandler<object> canExecute;

            /// <inheritdoc cref="AsyncData(ExecuteHandler, CanExecuteHandler)"/>
            public AsyncData(ExecuteHandler<object> execute, CanExecuteHandler<object> canExecute)
            {
                this.execute = execute ?? throw new ArgumentNullException(nameof(execute));


                if (canExecute == null)
                {
                    CanExecuteAsync = canExecuteNullAsync;
                }
                else
                {
                    this.canExecute = canExecute;
                    CanExecuteAsync = canExecuteAsync;
                }
            }

            /// <summary>Creates an instance.</summary>
            /// <param name="execute">Synchronous Execute method.</param>
            /// <param name="canExecute">Synchronous CanExecute method.</param>
            public AsyncData(ExecuteHandler execute, CanExecuteHandler canExecute)
            {
                if (execute == null)
                {
                    throw new ArgumentNullException(nameof(execute));
                }

                this.execute = p => execute();


                if (canExecute == null)
                {
                    CanExecuteAsync = canExecuteNullAsync;
                }
                else
                {
                    this.canExecute = p => canExecute();
                    CanExecuteAsync = canExecuteAsync;
                }
            }
        }

    }

    /// <summary> <see cref="RelayCommandAsync"/>implementation for generic parameter methods. </summary>
    /// <typeparam name = "T"> Method parameter type. </typeparam>  
    public class RelayCommandAsync<T> : RelayCommandAsync
    {
        /// <inheritdoc cref="RelayCommand{T}(ExecuteHandler{T}, CanExecuteHandler{T}, ConverterFromObjectHandler{T})"/>
        public RelayCommandAsync(ExecuteHandler<T> execute, CanExecuteHandler<T> canExecute, ConverterFromObjectHandler<T> converter = null)
            : base
            (
                  p =>
                  {
                      if (p is T t ||
                        (converter != null && converter(p, out t)))
                      {
                          execute(t);
                      }
                  },
                  p => ((p is T t) || (converter != null && converter(p, out t))) &&
                    (canExecute?.Invoke(t) ?? true)
            )
        { }


        /// <inheritdoc cref="RelayCommand{T}(ExecuteHandler{T}, ConverterFromObjectHandler{T})"/>
        public RelayCommandAsync(ExecuteHandler<T> execute, ConverterFromObjectHandler<T> converter = null)
           : this(execute, null, converter)
        { }
    }
}
