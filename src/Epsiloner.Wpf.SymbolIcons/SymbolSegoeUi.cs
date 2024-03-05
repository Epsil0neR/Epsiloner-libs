using System.Windows;
using System.Windows.Controls;
using Epsiloner.Wpf.Glyphs;

namespace Epsiloner.Wpf
{
    public class SymbolSegoeUi : TextBlock
    {
        static SymbolSegoeUi()
        {
            var t = typeof(SymbolSegoeUi);
            DefaultStyleKeyProperty.OverrideMetadata(t, new FrameworkPropertyMetadata(t));
        }

        public static DependencyProperty GlyphProperty = DependencyProperty.Register(
            nameof(Glyph),
            typeof(SegoeUiGlyph?),
            typeof(SymbolSegoeUi),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,
                GlyphPropertyChanged
            )
        );

        private static void GlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var glyph = (SegoeUiGlyph?) e.NewValue;
            d.SetValue(TextProperty, glyph.ToCode());
        }

        public SegoeUiGlyph? Glyph
        {
            get => (SegoeUiGlyph?)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
    }
    public class SymbolMaterial : TextBlock
    {
        static SymbolMaterial()
        {
            var t = typeof(SymbolMaterial);
            DefaultStyleKeyProperty.OverrideMetadata(t, new FrameworkPropertyMetadata(t));
        }

        public static DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(MaterialDesignIcon?),
            typeof(SymbolMaterial),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,
                IconPropertyChanged
            )
        );

        public static DependencyProperty ThemeProperty = DependencyProperty.Register(
            nameof(Theme),
            typeof(MaterialDesignTheme),
            typeof(SymbolMaterial),
            new FrameworkPropertyMetadata(MaterialDesignTheme.Normal,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,
                ThemePropertyChanged
            )
        );

        private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var icon = (MaterialDesignIcon?)e.NewValue;
            d.SetValue(TextProperty, icon.ToCode());
        }

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public MaterialDesignIcon? Icon
        {
            get => (MaterialDesignIcon?)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public MaterialDesignTheme Theme
        {
            get => (MaterialDesignTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        
    }
}