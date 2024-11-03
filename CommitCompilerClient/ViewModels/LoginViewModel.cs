using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommitCompiler.Models;
using CommitCompiler.Services;
using CommitCompiler.Services.Interface;

namespace CommitCompiler.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private AzureDevOpsService _azureDevOpsService;

        public UserInputModel UserInput { get; set; }
        public string PersonalAccessToken
        {
            get => _personalAccessToken;
            set => SetProperty(ref _personalAccessToken, value);
        }
        private string _personalAccessToken;

        public bool RememberCredentials
        {
            get => _rememberCredentials;
            set => SetProperty(ref _rememberCredentials, value);
        }
        private bool _rememberCredentials;

        public string Organization
        {
            get => _organization;
            set => SetProperty(ref _organization, value);
        }
        private string _organization;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        private string _errorMessage;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        private bool _isLoading;

        public ICommand LoginCommand { get; }
        public event Action LoginSuccess;

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            UserInput = new UserInputModel();
            LoginCommand = new RelayCommand(async () => await Login());
            LoadCredentials();
        }

        public async Task Login()
        {
            if (string.IsNullOrEmpty(Organization) || string.IsNullOrEmpty(PersonalAccessToken))
            {
                ErrorMessage = "Todos los campos son obligatorios.";
                return;
            }

            IsLoading = true;

            try
            {
                _azureDevOpsService = new AzureDevOpsService(Organization, PersonalAccessToken);
                await _azureDevOpsService.GetProjectsAsync();
                App.GlobalUserInput.Organization = Organization;
                App.GlobalUserInput.PersonalAccessToken = PersonalAccessToken;

                SaveCredentials();
                LoginSuccess?.Invoke();
            }
            catch
            {
                ErrorMessage = "Autenticación fallida. Verifique sus datos.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SaveCredentials()
        {
            if (RememberCredentials)
                UserCredentials.SaveCredentials(Organization, PersonalAccessToken);
            else
                UserCredentials.DeleteCredentials();
        }

        public void LoadCredentials()
        {
            var savedCredentials = UserCredentials.LoadCredentials();
            if (savedCredentials != null)
            {
                Organization = savedCredentials.Organization;
                PersonalAccessToken = savedCredentials.Token;
                RememberCredentials = true; // Se cargaron, por lo que la opción debe estar marcada
            }
        }
    }
}
