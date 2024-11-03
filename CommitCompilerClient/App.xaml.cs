using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using CommitCompiler.Services;
using CommitCompiler.Services.Interface;
using CommitCompiler.ViewModels;
using CommitCompiler.Views;
using System.Windows.Controls;
using CommitCompiler.Models;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Globalization;
using HandyControl.Tools;
using HandyControl.Properties.Langs;
using Microsoft.VisualStudio.Services.Common;

namespace CommitCompiler
{
    public partial class App : Application
    {

        public IServiceProvider ServiceProvider { get; private set; }
        public static UserInputModel GlobalUserInput { get; set; } = new UserInputModel();
        public static List<TeamProjectReference> Projects { get; set; } = new List<TeamProjectReference>();
        public static List<GitRepository> Repositories { get; set; } = new List<GitRepository>();
        public static List<GitRef> Branches { get; set; } = new List<GitRef>();
        public static List<GitCommitRef> Commits { get; set; } = new List<GitCommitRef>();

        public static App Current => (App)Application.Current;

        public App()
        {
            ServiceProvider = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigHelper.Instance.SetLang("es");
            var navigationService = ServiceProvider.GetRequiredService<INavigationService>();

            // Mostrar la ventana de splash antes de abrir la MainWindow
            var splashWindow = new SplashWindow();
            splashWindow.Show();

            // Crear un temporizador o usar un Task para esperar un par de segundos
            Task.Delay(2000).ContinueWith(_ =>
            {
                splashWindow.Dispatcher.Invoke(() =>
                {
                    splashWindow.Close();

                    // Iniciar la ventana principal después de la SplashScreen
                    var loginPage = ServiceProvider.GetRequiredService<LoginPage>();
                    loginPage.Show();
                    loginPage.Closed += (sender, args) =>
                    {
                        // Si el login fue exitoso, abrir MainWindow
                        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                        mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
                        mainWindow.Show();
                    };
                });
            });


        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Registrar ViewModels con inyección de dependencia
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<DashBoardViewModel>();
            services.AddTransient<ConfigurationViewModel>();
            services.AddTransient<ProcessViewModel>();
            services.AddTransient<LogViewModel>();

            // Registrar Ventanas y Servicios
            services.AddSingleton<LoginPage>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainPage>();
            services.AddSingleton<DashBoardPage>();
            services.AddSingleton<ConfigurationPage>();
            services.AddSingleton<ProcessPage>();
            services.AddSingleton<LogPage>();

            services.AddSingleton<INavigationService, NavigationService>(provider =>
            {
                var mainWindow = provider.GetRequiredService<MainWindow>();
                var frame = (Frame)mainWindow.FindName("MainContent");
                return new NavigationService(frame, provider);
            });

            return services.BuildServiceProvider();
        }
    }
}