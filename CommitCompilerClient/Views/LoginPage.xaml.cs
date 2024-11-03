using CommitCompiler.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommitCompiler.Views
{
    public partial class LoginPage : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            // Suscribirse al evento LoginSuccess para cerrar la ventana
            _viewModel.LoginSuccess += OnLoginSuccess;
            this.Loaded += LoginPage_Loaded;
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var viewModel = this.DataContext as LoginViewModel;

            if (viewModel != null)
            {
                viewModel.PersonalAccessToken = passwordBox.Password;
            }
        }
        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as LoginViewModel;
            if (viewModel != null && !string.IsNullOrEmpty(viewModel.PersonalAccessToken))
            {
                // Si ya hay un Token cargado en el ViewModel, asignarlo al PasswordBox
                txtToken.Password = viewModel.PersonalAccessToken;
            }
        }
        // Método para cerrar la ventana cuando el login es exitoso
        private void OnLoginSuccess()
        {
            this.Dispatcher.Invoke(() => this.Close());
        }
        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);

            // Evitar posibles fugas de memoria eliminando la suscripción al evento
            _viewModel.LoginSuccess -= OnLoginSuccess;
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
    }
}