using System.Runtime.ExceptionServices;

namespace CommonExtensions.TryExtensions
{
    /// <summary>
    /// Static factory methods for <see cref="Try{T}"/> types.
    /// </summary>
    public class Try<T>
    {
        /// <summary>
        /// The value
        /// </summary>
        private readonly T _value;

        /// <summary>
        /// Creates a wrapper for an exception.
        /// </summary>
        /// <param name="exception">The exception to wrap.</param>
        public static Try<T> FromException(Exception exception) => new Try<T>(exception, default, true);

        /// <summary>
        /// Creates a wrapper for a value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        public static Try<T> FromValue(T value) => new Try<T>(default, value, false);

        /// <summary>
        /// Executes the specified function, and wraps either the result or the exception.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public static Try<T> Create(Func<T> func)
        {
            try
            {
                return FromValue(func());
            }
            catch (Exception exception)
            {
                return FromException(exception);
            }
        }

        /// <summary>
        /// Executes the specified function, and wraps either the result or the exception.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public static async Task<Try<T>> Create(Func<Task<T>> func)
        {
            try
            {
                return FromValue(await func().ConfigureAwait(false));
            }
            catch (Exception exception)
            {
                return FromException(exception);
            }
        }

        /// <summary>
        /// Maps a wrapped value to another mapped value.
        /// If this instance is an exception, <paramref name="func"/> is not invoked, and this method returns a wrapper for that exception.
        /// If <paramref name="func"/> throws an exception, this method returns a wrapper for that exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the mapping.</typeparam>
        /// <param name="func">The mapping function. Exceptions from this method are captured and wrapped.</param>
        public Try<TResult> Map<TResult>(Func<T, TResult> func) => Bind(value => Try<TResult>.Create(() => func(value)));

        /// <summary>
        /// Maps a wrapped value to another mapped value.
        /// If this instance is an exception, <paramref name="func"/> is not invoked, and this method immediately returns a wrapper for that exception.
        /// If <paramref name="func"/> throws an exception, this method returns a wrapper for that exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the mapping.</typeparam>
        /// <param name="func">The mapping function. Exceptions from this method are captured and wrapped.</param>
        public Task<Try<TResult>> Map<TResult>(Func<T, Task<TResult>> func) => Bind(value => Try<TResult>.Create(async () => await func(value).ConfigureAwait(false)));

        /// <summary>
        /// Binds the wrapped value.
        /// If this instance is an exception, <paramref name="bind"/> is not invoked, and this method returns a wrapper for that exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the binding.</typeparam>
        /// <param name="bind">The binding function. Should not throw exceptions.</param>
        public Try<TResult> Bind<TResult>(Func<T, Try<TResult>> bind) => IsException ? Try<TResult>.FromException(Exception) : bind(_value);

        /// <summary>
        /// Binds the wrapped value.
        /// If this instance is an exception, <paramref name="bind"/> is not invoked, and this method immediately returns a wrapper for that exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the binding.</typeparam>
        /// <param name="bind">The binding function. Should not throw exceptions.</param>
        public async Task<Try<TResult>> Bind<TResult>(Func<T, Task<Try<TResult>>> bind) => IsException ? Try<TResult>.FromException(Exception) : await bind(_value).ConfigureAwait(false);

        /// <summary>
        /// Executes an action for the wrapped exception or value.
        /// </summary>
        /// <param name="whenException">The action to execute if this instance is an exception.</param>
        /// <param name="whenValue">The action to execute if this instance is a value.</param>
        public TResult Match<TResult>(Func<Exception, TResult> whenException, Func<T, TResult> whenValue) => IsException ? whenException(Exception) : whenValue(_value);

        /// <summary>
        /// Enables LINQ support as a monad.
        /// </summary>
        public Try<TResult> Select<TResult>(Func<T, TResult> func) => Map(func);

        /// <summary>
        /// Enables LINQ support as a monad.
        /// </summary>
        public Try<TResult> SelectMany<TOther, TResult>(Func<T, Try<TOther>> bind, Func<T, TOther, TResult> project) => Bind(a => bind(a).Select(b => project(a, b)));

        /// <summary>
        /// Deconstructs this wrapper into two variables.
        /// </summary>
        /// <param name="exception">The wrapped exception, or <c>null</c> if this instance is a value.</param>
        /// <param name="value">The wrapped value, or <c>default</c> if this instance is an exception.</param>
        public void Deconstruct(out Exception exception, out T value)
        {
            exception = Exception;
            value = _value;
        }

        /// <summary>
        /// Whether this instance is an exception.
        /// </summary>
        public bool IsException => this.Exception != null;

        /// <summary>
        /// Whether this instance is a value.
        /// </summary>
        public bool IsValue => !IsException;

        /// <summary>
        /// Gets the wrapped exception, if any. Returns <c>null</c> if this instance is not an exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the wrapped value. If this instance is an exception, then that exception is (re)thrown.
        /// </summary>
        public T Value => this.IsException ? throw Rethrow() : this._value;

        /// <summary>
        /// Gets the wrapped value. If this instance is an exception, then null.
        /// </summary>
        public T ValueOrDefault => this.IsException ? default : this._value;

        /// <summary>
        /// Executes action on exception
        /// </summary>
        /// <param name="action">Action to executes</param>
        public Try<T> Otherwise(Action<Exception> action)
        {
            if (this.IsException)
            {
                action(this.Exception);
            }
            return this;
        }
        /// <summary>
        /// Gets the wrapped value. If this instance is an exception, then use default supplier.
        /// </summary>
        public T ValueOr(Func<T> defaultSupplier)
        {
            return this.IsException ? defaultSupplier() : this._value;
        }

        /// <summary>
        /// Gets the wrapped value. If this instance is an exception, then use default supplier.
        /// </summary>
        public T ValueOr(T @default)
        {
            return this.IsException ? @default : this._value;
        }

        /// <summary>
        /// A string representation, useful for debugging.
        /// </summary>
        public override string ToString() => this.IsException ? $"Exception: {this.Exception}" : $"Value: {_value}";

        /// <summary>
        /// Creates new instance of Try
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="value"></param>
        /// <param name="isException"></param>
        private Try(Exception exception, T value, bool isException)
        {
            if (isException)
            {
                Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            }
            else
            {
                _value = value;
            }
        }

        /// <summary>
        /// Rethrow the exception
        /// </summary>
        /// <returns></returns>
        private Exception Rethrow()
        {
            ExceptionDispatchInfo.Capture(Exception).Throw();
            return Exception;
        }
    }
}
