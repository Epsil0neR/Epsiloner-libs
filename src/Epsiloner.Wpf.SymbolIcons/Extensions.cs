using System;
using Epsiloner.Wpf.Glyphs;

namespace Epsiloner.Wpf
{
    internal static class Extensions
    {
        public static string ToCode(this SegoeUiGlyph? glyph)
        {
            if (!glyph.HasValue)
                return string.Empty;

            var value = (int)glyph.Value;
            return char.ConvertFromUtf32(value);
        }

        public static string ToCode(this MaterialDesignIcon? icon)
        {
            if (!icon.HasValue)
                return string.Empty;

            var value = (int)icon.Value;
            return char.ConvertFromUtf32(value);
        }
    }
}