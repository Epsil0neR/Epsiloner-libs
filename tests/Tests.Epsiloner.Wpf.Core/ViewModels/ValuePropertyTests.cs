using System.ComponentModel;
using System.Windows.Markup;
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
            private decimal _salary;

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
                _fullNameProperty = new ValueProperty<string>().With(_ => RaisePropertyChanged(nameof(FullNameProperty)));
                _age = new ValueProperty<int>().With(_ => RaisePropertyChanged(nameof(Age)));
            }

            public decimal Salary
            {
                get => _salary;
                set => Set(ref _salary, value);
            }

            [DependsOn(nameof(Salary))]
            public string SalaryAsString => Salary.ToString("C2");

            public string Name
            {
                get;
                set => Set(ref field, value);
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
            p.PropertyChanged += VMOnPropertyChanged;
            p.Age.With(v => _output.WriteLine($"Age: {v}"), true);
            p.FullNameProperty.With(v => _output.WriteLine($"Full name: {v}"));

            p.Age = 123;
            p.Age.Value = 100500;

            p.Salary = 500;
            p.Salary = 1_500;
            p.Salary = 1_000_000;

            p.FullNameProperty.Value = new ValueProperty<string>("VL");
            p.FullNameProperty.Value = "new ValueProperty<string>(\"VL\");";

        }

        private void VMOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _output.WriteLine($"Property changed: {e.PropertyName}");
        }
    }
}
