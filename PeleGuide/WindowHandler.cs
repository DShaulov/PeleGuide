using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace PeleGuide
{
    public class WindowHandler
    {
        private readonly Window window;

        public WindowHandler(Window window)
        {
            this.window = window;
        }

        public void HandleTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as FrameworkElement;
            while (clickedElement != null)
            {
                if (clickedElement is Button button)
                {
                    HandleTitleBarButtonClick(button.Name);
                    return;
                }
                clickedElement = VisualTreeHelper.GetParent(clickedElement) as FrameworkElement;
            }

            HandleWindowDragging(e);
        }

        private void HandleTitleBarButtonClick(string buttonName)
        {
            switch (buttonName)
            {
                case "MinimizeButton":
                    window.WindowState = WindowState.Minimized;
                    break;
                case "MaximizeButton":
                    ToggleMaximizeState();
                    break;
                case "CloseButton":
                    window.Close();
                    break;
            }
        }

        private void HandleWindowDragging(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximizeState();
            }
            else
            {
                window.DragMove();
            }
        }

        private void ToggleMaximizeState()
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
    }
}
