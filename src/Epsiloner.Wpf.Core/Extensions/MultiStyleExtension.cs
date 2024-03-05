using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Epsiloner.Wpf.Extensions
{
    /// <summary>
    /// Extension that allows to merge multiple styles into one.
    /// </summary>
    [MarkupExtensionReturnType(typeof(Style))]
    public class MultiStyleExtension : MarkupExtension
    {
        private readonly Style[] _styles;

        #region  Constructors

        /// <summary>
        /// Initialize a new extension that merges multiple styles.
        /// </summary>
        /// <param name="styles">Styles to merge</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MultiStyleExtension(params Style[] styles)
        {
            if (styles == null || !styles.Any())
                throw new ArgumentNullException(nameof(styles));

            _styles = styles;
        }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1)
            : this(new[] { style1 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2)
            : this(new[] { style1, style2 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3)
            : this(new[] { style1, style2, style3 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4)
            : this(new[] { style1, style2, style3, style4 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4, Style style5)
            : this(new[] { style1, style2, style3, style4, style5 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4, Style style5, Style style6)
            : this(new[] { style1, style2, style3, style4, style5, style6 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4, Style style5, Style style6, Style style7)
            : this(new[] { style1, style2, style3, style4, style5, style6, style7 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4, Style style5, Style style6, Style style7, Style style8)
            : this(new[] { style1, style2, style3, style4, style5, style6, style7, style8 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4, Style style5, Style style6, Style style7, Style style8, Style style9)
            : this(new[] { style1, style2, style3, style4, style5, style6, style7, style8, style9 })
        { }

        /// <inheritdoc />
        public MultiStyleExtension(Style style1, Style style2, Style style3, Style style4, Style style5, Style style6, Style style7, Style style8, Style style9, Style style10)
            : this(new[] { style1, style2, style3, style4, style5, style6, style7, style8, style9, style10 })
        { }
        #endregion

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetMerged();
        }

        /// <summary>
        /// Gets merged styles.
        /// </summary>
        /// <returns></returns>
        private Style GetMerged()
        {
            var s = _styles?.Where(x => x != null).ToList();
            if (s.Count == 1)
                return s[0];

            var rv = new Style();
            foreach (var style in s)
                rv.Merge(style);

            return rv;
        }

        /// <summary>
        /// Gets merged styles. NOTE: Executes merging on every request.
        /// </summary>
        public Style Merged => GetMerged();
    }

    internal static class MultiStyleMethods
    {
        /// <summary>
        /// Merges the two styles passed as parameters. The first style will be modified to include any 
        /// information present in the second. If there are collisions, the second style takes priority.
        /// </summary>
        /// <param name="style1">First style to merge, which will be modified to include information from the second one.</param>
        /// <param name="style2">Second style to merge.</param>
        public static void Merge(this Style style1, Style style2)
        {
            if (style1 == null)
                throw new ArgumentNullException(nameof(style1));
            if (style2 == null)
                throw new ArgumentNullException(nameof(style2));
            if (style1.TargetType.IsAssignableFrom(style2.TargetType))
                style1.TargetType = style2.TargetType;
            if (style2.BasedOn != null)
                Merge(style1, style2.BasedOn);
            foreach (SetterBase currentSetter in style2.Setters)
                style1.Setters.Add(currentSetter);
            foreach (TriggerBase currentTrigger in style2.Triggers)
                style1.Triggers.Add(currentTrigger);

            // This code is only needed when using DynamicResources.
            foreach (object key in style2.Resources.Keys)
                style1.Resources[key] = style2.Resources[key];
        }
    }
}
