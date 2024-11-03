using CommitCompiler.Models;
using CommitCompiler.Services.Interface;
using CommitCompiler.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommitCompiler.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly Frame _mainFrame;

        public ICommand InicioCommand { get; }
        public ICommand DashBoardCommand { get; }
        public ICommand ProcessCommand { get; }
        public ICommand ConfigurationCommand { get; }
        public ICommand LogCommand { get; }


        public MainWindowViewModel(INavigationService navigationService)
        {
            // Frame principal para navegación
            var mainWindow = Application.Current.MainWindow as MainWindow;
            _mainFrame = mainWindow?.MainFrame;
            navigationService.SetMainFrame(_mainFrame);
            _navigationService = navigationService;

            // Navegar a MainPage al iniciar
            InitializeNavigation();

            InicioCommand = new RelayCommand(async () => await NavigateToAsync<MainViewModel>("Inicio"));
            DashBoardCommand = new RelayCommand(async () => await NavigateToAsync<DashBoardViewModel>("DashBoard"));
            ConfigurationCommand = new RelayCommand(async () => await NavigateToAsync<ConfigurationViewModel>("Configuration"));
            ProcessCommand = new RelayCommand(async () => await NavigateToAsync<ProcessViewModel>("Process"));
            LogCommand = new RelayCommand(async () => await NavigateToAsync<LogViewModel>("Log"));
        }
        private async void InitializeNavigation()
        {
            // Aquí navegas al ViewModel que corresponde a MainPage
            await NavigateToAsync<MainViewModel>("Inicio");
        }
        private async Task NavigateToAsync<TViewModel>(string viewName) where TViewModel : class
        {
            _navigationService.NavigateTo<TViewModel>();
        }
    }
}
