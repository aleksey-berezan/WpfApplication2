using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // parent form subscriptions
            PreviewMouseDown += MainWindow_MouseDown;
            LocationChanged += OnPopupToClose;
            SizeChanged += OnPopupToClose;
            Closed += MainWindow_Closed;
            KeyDown += MainWindow_KeyDown;
            KeyUp += MainWindow_KeyDown;

            // Popup elements
            TextBox.MouseDownDirect += OnPopupToOpen;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Closed -= MainWindow_Closed;
            PreviewMouseDown -= MainWindow_MouseDown;
            TextBox.MouseDownDirect -= OnPopupToOpen;
            LocationChanged -= OnPopupToClose;
            SizeChanged -= OnPopupToClose;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            OnGeneric((FrameworkElement)e.Source);
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnGeneric((FrameworkElement)e.Source);
        }

        private void OnGeneric(FrameworkElement source)
        {
            bool isInPopupHierarchy = IsInHierarchy(TextBox, source)
                                      || IsInHierarchy(Popup, source);
            Popup.IsOpen = isInPopupHierarchy;
        }

        private void OnPopupToOpen(object s, EventArgs e)
        {
            Popup.IsOpen = true;
        }

        private void OnPopupToClose(object s, EventArgs e)
        {
            Popup.IsOpen = false;
        }

        private static bool IsInHierarchy(FrameworkElement checkedElement, FrameworkElement hierarchyBottom)
        {
            return GetParentsHierarchy(hierarchyBottom).Any(x => ReferenceEquals(x, checkedElement));
        }

        private static IEnumerable<FrameworkElement> GetParentsHierarchy(FrameworkElement fe)
        {
            yield return fe;
            foreach (var parent in GetParents(fe))
            {
                yield return parent;
            }
        }

        private static IEnumerable<FrameworkElement> GetParents(FrameworkElement fe)
        {
            FrameworkElement parent = fe.Parent as FrameworkElement;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent as FrameworkElement;
            }
        }
    }

    public class TextBoxEx : TextBox
    {
        public event EventHandler MouseDownDirect;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            MouseDownDirect?.Invoke(this, e);
        }
    }
}
