using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ws.Extensions.UI.Wpf.Utils
{
    /// <summary>
    /// Contains extension methods for enumerating the children of an element.
    /// </summary>
    public static class ChildrenOfTypeExtensions
    {
        /// <summary>
        /// Gets all child elements recursively from the visual tree by given type.
        /// </summary>
        public static IEnumerable<T> ChildrenOfType<T>(this DependencyObject element)
        where T : DependencyObject
        {
            return element.GetChildrenRecursive().OfType<T>();
        }

        internal static IEnumerable<T> ChildrenOfType<T>(this DependencyObject element, Type typeWhichChildrenShouldBeSkipped)
        {
            return element.GetChildrenOfType(typeWhichChildrenShouldBeSkipped).OfType<T>();
        }

        /// <summary>
        /// Finds child element of the specified type. Uses breadth-first search.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the child that will be searched in the object hierarchy. The type should be <see cref="T:System.Windows.DependencyObject" />.
        /// </typeparam>
        /// <param name="element">The target <see cref="T:System.Windows.DependencyObject" /> which children will be traversed.</param>
        /// <returns>The first child element that is of the specified type.</returns>
        public static T FindChildByType<T>(this DependencyObject element)
        where T : DependencyObject
        {
            return element.ChildrenOfType<T>().FirstOrDefault<T>();
        }

        internal static IEnumerable<T> FindChildrenByType<T>(this DependencyObject element)
        where T : DependencyObject
        {
            return element.ChildrenOfType<T>();
        }

        internal static FrameworkElement GetChildByName(this FrameworkElement element, string name)
        {
            return (FrameworkElement)element.FindName(name) ?? element.ChildrenOfType<FrameworkElement>().FirstOrDefault<FrameworkElement>((FrameworkElement c) => c.Name == name);
        }

        internal static IEnumerable<T> GetChildren<T>(this DependencyObject parent)
        where T : FrameworkElement
        {
            return parent.GetChildrenRecursive().OfType<T>();
        }

        private static IEnumerable<DependencyObject> GetChildrenOfType(this DependencyObject element, Type typeWhichChildrenShouldBeSkipped)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                DependencyObject dependencyObject = VisualTreeHelper.GetChild(element, i);
                yield return dependencyObject;
                if (!typeWhichChildrenShouldBeSkipped.IsAssignableFrom(dependencyObject.GetType()))
                {
                    foreach (DependencyObject childrenOfType in dependencyObject.GetChildrenOfType(typeWhichChildrenShouldBeSkipped))
                    {
                        yield return childrenOfType;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates through element's children in the visual tree.
        /// </summary>
        private static IEnumerable<DependencyObject> GetChildrenRecursive(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                DependencyObject dependencyObject = VisualTreeHelper.GetChild(element, i);
                if (dependencyObject != null)
                {
                    yield return dependencyObject;
                    foreach (DependencyObject childrenRecursive in dependencyObject.GetChildrenRecursive())
                    {
                        yield return childrenRecursive;
                    }
                }
            }
        }

        /// <summary>
        /// Does a deep search of the element tree, trying to find a descendant of the given type 
        /// (including the element itself).
        /// </summary>
        /// <returns>True if the target is one of the elements.</returns>
        internal static T GetFirstDescendantOfType<T>(this DependencyObject target)
        where T : DependencyObject
        {
            T t = (T)(target as T);
            if (t == null)
            {
                t = target.ChildrenOfType<T>().FirstOrDefault<T>();
            }
            return t;
        }
    }
}
