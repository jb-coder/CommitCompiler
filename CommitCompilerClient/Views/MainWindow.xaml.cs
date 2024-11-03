using CommitCompiler.Services.Interface;
using CommitCompiler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommitCompiler.Views
{

    public partial class MainWindow : Window
    {
        private bool isMenuOpen = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Alterna el estado del menú
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            // Define la distancia de desplazamiento para abrir/cerrar el menú
            double from = isMenuOpen ? 0 : -200;
            double to = isMenuOpen ? -200 : 0;

            // Crea la animación de desplazamiento
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Aplica la animación al Transform del menú
            MenuTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            // Alterna el estado del menú
            isMenuOpen = !isMenuOpen;
        }
    }
}