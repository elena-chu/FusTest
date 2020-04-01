using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ws.Extensions.UI.Wpf.Utils
{
    public static class ParentOfTypeExtensions
    {
        /// <summary>  
        /// This recurse the visual tree for ancestors of a specific type.
        /// </summary>  
        internal static IEnumerable<T> GetAncestors<T>(this DependencyObject element)
        where T : class
        {
            return element.GetParents().OfType<T>();
        }

        /// <summary>  
        /// This recurse the visual tree for a parent of a specific type.
        /// </summary>  
        internal static T GetParent<T>(this DependencyObject element)
        where T : FrameworkElement
        {
            return element.ParentOfType<T>();
        }

        private static DependencyObject GetParent(this DependencyObject element)
        {
            DependencyObject parent = null;
            try
            {
                parent = VisualTreeHelper.GetParent(element);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                parent = null;
            }
            if (parent == null)
            {
                FrameworkElement frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                {
                    parent = frameworkElement.Parent;
                }
                FrameworkContentElement frameworkContentElement = element as FrameworkContentElement;
                if (frameworkContentElement != null)
                {
                    parent = frameworkContentElement.Parent;
                }
            }
            return parent;
        }

        /// <summary>
        /// Enumerates through element's parents in the visual tree.
        /// </summary>
        public static IEnumerable<DependencyObject> GetParents(this DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            while (true)
            {
                DependencyObject parent = element.GetParent();
                DependencyObject dependencyObject = parent;
                element = parent;
                if (dependencyObject == null)
                {
                    break;
                }
                yield return element;
            }
        }

        /// <summary>
        /// Searches up in the visual tree for parent element of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parent that will be searched up in the visual object hierarchy. 
        /// The type should be <see cref="T:System.Windows.DependencyObject" />.
        /// </typeparam>
        /// <param name="element">The target <see cref="T:System.Windows.DependencyObject" /> which visual parents will be traversed.</param>
        /// <returns>Visual parent of the specified type if there is any, otherwise null.</returns>
        public static T GetVisualParent<T>(this DependencyObject element)
        where T : DependencyObject
        {
            return element.ParentOfType<T>();
        }

        /// <summary>
        ///  Determines whether the element is an ancestor of the descendant.
        /// </summary>
        /// <returns>true if the visual object is an ancestor of descendant; otherwise, false.</returns>
        public static bool IsAncestorOf(this DependencyObject element, DependencyObject descendant)
        {
            element.TestNotNull("element");
            descendant.TestNotNull("descendant");
            if (descendant == element)
            {
                return true;
            }
            return descendant.GetParents().Contains<DependencyObject>(element);
        }

        /// <summary>
        /// Gets the parent element from the visual tree by given type.
        /// </summary>
        public static T ParentOfType<T>(this DependencyObject element)
        where T : DependencyObject
        {
            if (element == null)
            {
                return default(T);
            }
            return element.GetParents().OfType<T>().FirstOrDefault<T>();
        }
    }
}
