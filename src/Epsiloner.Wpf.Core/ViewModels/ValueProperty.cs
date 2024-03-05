using System;
using System.Collections.Generic;

namespace Epsiloner.Wpf.ViewModels
{
    public class ValueProperty<T>
    {
        public static implicit operator ValueProperty<T>(T value) => new ValueProperty<T>(value);
        public static implicit operator T(ValueProperty<T> vp) => vp.Value;

        private readonly List<Action<T>> _handlers = new List<Action<T>>();
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;

                _handlers.ForEach(action =>
                {
                    try
                    {
                        action(value);
                    }
                    catch (Exception)
                    {
                    }
                });
            }
        }

        public ValueProperty(T initValue = default)
        {
            _value = initValue;
        }

        public ValueProperty<T> With(Action<T> handler, bool executeNow = false)
        {
            _handlers.Add(handler);
            if (executeNow)
                handler(Value);

            return this;
        }
    }
}
