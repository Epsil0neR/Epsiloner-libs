using System.ComponentModel;
using Epsiloner.Wpf.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace Test.Epsiloner.Wpf.Core.ViewModels
{
    public class ValuePropertyTests
    {

        class Person : ViewModel
        {
            private readonly ValueProperty<int> _age;
            private readonly ValueProperty<string> _fullNameProperty;

            public ValueProperty<string> FullNameProperty
            {
                get => _fullNameProperty;
                set => _fullNameProperty.Value = value;
            }

            public ValueProperty<int> Age
            {
                get => _age;
                set => _age.Value = value;
            }

            public Person()
            {
                _fullNameProperty = new ValueProperty<string>().With(v => RaisePropertyChanged(nameof(FullNameProperty)));
                _age = new ValueProperty<int>().With(v => RaisePropertyChanged(nameof(Age)));
            }
        }

        private readonly ITestOutputHelper _output;

        public ValuePropertyTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test()
        {
            var p = new Person();
            //p.PropertyChanged += VMOnPropertyChanged;
            p.Age.With(v => _output.WriteLine($"Age: {v}"), true);
            p.FullNameProperty.With(v => _output.WriteLine($"Full name: {v}"));

            p.Age = 123;
            p.Age.Value = 100500;

            p.FullNameProperty.Value = new ValueProperty<string>("VL");
            p.FullNameProperty.Value = "new ValueProperty<string>(\"VL\");";
        }

        private void VMOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _output.WriteLine($"Property changed: {e.PropertyName}");
        }
    }
}
