using System;

namespace LightBDD.Testing
{
    public class RepeatTimeoutException<T> : TimeoutException
    {
        public T LastValue { get; }

        public RepeatTimeoutException(string message, T lastValue)
            : base($"{message}, Last value: {lastValue}")
        {
            LastValue = lastValue;
        }
    }
}