using System;

namespace Epsiloner.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NotSupportedInThemes : Attribute
    {
        public NotSupportedInThemes(params MaterialDesignTheme[] themes)
        {
            Themes = themes;
        }

        public MaterialDesignTheme[] Themes { get; }
    }
}
