using System.Linq;
using Epsiloner.Wpf.Converters;
using Epsiloner.Wpf.Gestures;
using Xunit;

namespace Test.Epsiloner.Wpf.Core.Converters
{
    public class MultiKeyGestureConverterTest
    {
        public MultiKeyGestureConverter Converter { get; } = new MultiKeyGestureConverter();

        [Fact]
        public void CanConvert_OnlyAcceptsString()
        {
            Assert.True(Converter.CanConvertFrom(null, typeof(string)));
            Assert.False(Converter.CanConvertFrom(null, typeof(object)));
            Assert.False(Converter.CanConvertFrom(null, typeof(Gesture)));
            Assert.False(Converter.CanConvertFrom(null, typeof(MultiKeyGesture)));
        }

        [Theory]
        [InlineData("Q", 1)]
        [InlineData("Q A", 2)]
        [InlineData(" ", 0)]
        [InlineData("+Q", 0)] // Modifier expected.
        [InlineData("Alt", 1)]
        [InlineData("Alt+Q", 1)]
        [InlineData("Alt+Q Q", 2)]
        [InlineData("Alt+Q  Q", 2)] // Extra spaces are ignored.
        [InlineData("Q Alt+Q", 2)]
        [InlineData("Alt+Q Alt+Q", 2)]
        [InlineData("Ctrl+Q Alt+Q", 2)]
        public void ConvertFrom(string gestureData, int gesturesCount)
        {
            var gesture = Converter.ConvertFrom(null, null, gestureData) as MultiKeyGesture;
            Assert.Equal(gesturesCount == 0, gesture == null);

            if (gesturesCount > 0)
                Assert.Equal(gesturesCount, gesture?.Gestures.Count());
        }
    }
}
