using System;
using System.Windows;
using System.Windows.Controls;
using Epsiloner.Wpf.Attributes;

namespace Epsiloner.Wpf.Controls
{
    /// <summary>
    /// Data template selector for selecting best matching registered view with <see cref="ViewForAttribute"/> attribute.
    /// </summary>
    public class ViewDataTemplateSelector : DataTemplateSelector
    {
        private static ViewDataTemplateSelector _instance;

        /// <summary>
        /// Default data template selector for views which uses <see cref="ViewForAttribute"/> attribute.
        /// </summary>
        public static ViewDataTemplateSelector Instance => _instance ?? (_instance = new ViewDataTemplateSelector());

        private ViewDataTemplateSelector()
        {
        }

        /// <inheritdoc cref="DataTemplateSelector.SelectTemplate"/>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if (item == null)
                return base.SelectTemplate(null, container);

            var dataType = item.GetType();
            var viewType = ViewForAttribute.GetBestMatchingViewType(dataType);
            if (viewType != null)
            {
                return CreateDataTemplate(dataType, viewType, item);
            }

            return base.SelectTemplate(item, container);
        }

        private static DataTemplate CreateDataTemplate(Type dataType, Type viewType, object dataContext)
        {
            var dataTemplate = new DataTemplate(dataType);
            var elementFactory = new FrameworkElementFactory(viewType);
            elementFactory.SetValue(FrameworkElement.DataContextProperty, dataContext);
            dataTemplate.VisualTree = elementFactory;

            return dataTemplate;
        }
    }
}
