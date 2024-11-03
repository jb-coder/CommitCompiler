using CommitCompiler.Services;
using CommitCompiler.Services.Interface;
using CommitCompilerShared.Data;
using CommitCompilerShared.Models;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CommitCompiler.ViewModels
{
    public class ConfigurationViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IComboService _comboService;
        private readonly BuildConfigurationService _buildConfigurationService;
        private AzureDevOpsService _azureDevOpsService;
        public ICommand SaveCommand { get; }

        //public ObservableCollection<Process> Processes { get; set; }
        private Guid? _idApplication;
        private string _selectedCbRepoItem;
        private string _selectedCbBranchItem;
        private string _selectedCbBranchMergeItem;
        private string _selectedCbProjectItem;
        private ObservableCollection<string> _cbRepoItems;
        private ObservableCollection<string> _cbBranchItems;
        private ObservableCollection<string> _cbProjectItems;
        private List<TeamProjectReference> _projects;
        private List<GitRepository> _repositories;
        private List<GitRef> _branches;
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string EmailSubject { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public bool AutoMerge { get; set; }
        public string SelectedDestinationBranch { get; set; }
        public string ProcessTime { get; set; }
        public string Path { get; set; }
        public string PassEmail { get; set; }

        private bool _autoMerge;
        public bool AutoMerge
        {
            get { return _autoMerge; }
            set
            {
                if (_autoMerge != value)
                {
                    _autoMerge = value;
                    OnPropertyChanged(nameof(AutoMerge));
                }
            }
        }
        public string SelectedCbRepoItem
        {
            get => _selectedCbRepoItem;
            set
            {
                _selectedCbRepoItem = value;
                OnPropertyChanged(nameof(SelectedCbRepoItem));
                LoadBranchesForSelectedApp();
            }
        }

        public string SelectedCbBranchItem
        {
            get => _selectedCbBranchItem;
            set
            {
                _selectedCbBranchItem = value;
                OnPropertyChanged(nameof(SelectedCbBranchItem));
            }
        }
        public string SelectedCbBranchMergeItem
        {
            get => _selectedCbBranchMergeItem;
            set
            {
                _selectedCbBranchMergeItem = value;
                OnPropertyChanged(nameof(SelectedCbBranchMergeItem));
            }
        }

        public ObservableCollection<string> CbBranchItems
        {
            get => _cbBranchItems;
            set
            {
                _cbBranchItems = value;
                OnPropertyChanged(nameof(CbBranchItems));
            }
        }

        public string SelectedCbProjectItem
        {
            get => _selectedCbProjectItem;
            set
            {
                _selectedCbProjectItem = value;
                OnPropertyChanged(nameof(SelectedCbProjectItem));
                LoadRepoForSelectedProject();
            }
        }

        public ObservableCollection<string> CbProjectItems
        {
            get => _cbProjectItems;
            set
            {
                _cbProjectItems = value;
                OnPropertyChanged(nameof(CbProjectItems));
            }
        }

        public ObservableCollection<string> CbRepoItems
        {
            get => _cbRepoItems;
            set
            {
                _cbRepoItems = value;
                OnPropertyChanged(nameof(CbRepoItems));
            }
        }
        public ConfigurationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CommitCompilerContext context = new CommitCompilerContext();
            _buildConfigurationService = new BuildConfigurationService(context);

            _azureDevOpsService = new AzureDevOpsService(App.GlobalUserInput.Organization,
                                                App.GlobalUserInput.PersonalAccessToken);
            _comboService = new ComboService();

            SaveCommand = new RelayCommand(Save);
            AutoMerge = false;
            LoadProject();
        }

        private async Task LoadProject()
        {
            _projects = await _azureDevOpsService.GetProjectsAsync();
            (CbProjectItems, SelectedCbProjectItem) = await _comboService.LoadProjectsAsync(_projects);
        }

        private async Task LoadRepoForSelectedProject()
        {
            _repositories = await _azureDevOpsService.GetRepositoriesAsync(SelectedCbProjectItem);
            (CbRepoItems, SelectedCbRepoItem) = await _comboService.LoadReposAsync(_repositories, SelectedCbProjectItem);
        }

        private async Task LoadBranchesForSelectedApp()
        {
            var selectedRepository = _repositories.FirstOrDefault(repo => repo.Name == SelectedCbRepoItem);
            if (selectedRepository == null)
            {
                // Manejo de error si no se encuentra el repositorio
                return;
            }

            _idApplication = selectedRepository.Id; // Asignación directa si se encontró el repositorio

            _branches = await _azureDevOpsService.GetBranchesAsync(SelectedCbProjectItem, _idApplication.ToString());
            (CbBranchItems, SelectedCbBranchItem) = await _comboService.LoadBranchesAsync(_branches, SelectedCbProjectItem, _idApplication);
        }


        private void Save()
        {
            // Logic to save the configuration
            List<string> errors = new List<string>();
            BuildConfiguration conf = new BuildConfiguration();

            errors = CheckForErrors();

            if (errors.Count > 0)
            {
                // Muestra los errores en un MessageBox
                string errorMessage = string.Join(Environment.NewLine, errors);
                MessageBox.Show(errorMessage, "Errores de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Guardamos en el objeto la configuración si no hemos detectado ningun error de usuario
            conf.Organization = App.GlobalUserInput.Organization;
            conf.Token = App.GlobalUserInput.PersonalAccessToken;
            conf.Project = SelectedCbProjectItem;
            conf.Repository = SelectedCbRepoItem;
            conf.Branch = SelectedCbBranchItem;
            conf.DateStartProcess = StartDate;
            conf.DateEndProcess = EndDate;
            conf.ProcessTime = double.TryParse(ProcessTime, out double processTimeValue) ? processTimeValue : 10;
            conf.AutoMerge = AutoMerge;
            conf.DestinationBranch = SelectedCbBranchMergeItem;
            conf.Notification = true;
            conf.EmailOriginSender = SenderEmail;
            conf.EmailOriginPass = PassEmail;
            conf.EmailDestination = RecipientEmail;
            conf.EmailDestinationSubject = EmailSubject;
            conf.NotificationSubjectCommit = "Prueba";
            conf.PathDestination = Path;

            //Grabamos la configuracion en la db
            try
            {
                _buildConfigurationService.AddBuildConfiguration(conf); // Llama al servicio para guardar
                MessageBox.Show("Configuración guardada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la configuración: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private List<string> CheckForErrors()
        {
            var errores = new List<string>();

            // Verificar elementos seleccionados
            if (SelectedCbProjectItem.Contains("Seleccione"))
            {
                errores.Add("Debe seleccionar un proyecto válido.");
            }
            if (SelectedCbRepoItem.Contains("Seleccione"))
            {
                errores.Add("Debe seleccionar un repositorio válido.");
            }
            if (SelectedCbBranchItem.Contains("Seleccione"))
            {
                errores.Add("Debe seleccionar una rama válida.");
            }

            // Validar el correo electrónico
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(SenderEmail, pattern))
            {
                errores.Add("El correo electrónico del remitente no es válido.");
            }
            if (!Regex.IsMatch(RecipientEmail, pattern))
            {
                errores.Add("El correo electrónico del remitente no es válido.");
            }
            // Validar contraseña de correo
            if (string.IsNullOrWhiteSpace(PassEmail))
            {
                errores.Add("La contraseña del correo electrónico no puede estar vacía.");
            }

            // Validar asunto del correo
            if (string.IsNullOrWhiteSpace(EmailSubject))
            {
                errores.Add("El asunto del correo electrónico no puede estar vacío.");
            }

            // Validar fechas de inicio y fin
            if (StartDate == DateTime.MinValue)
            {
                errores.Add("La fecha de inicio no puede estar vacía.");
            }
            if (EndDate == DateTime.MinValue)
            {
                errores.Add("La fecha de fin no puede estar vacía.");
            }
            if (EndDate < StartDate)
            {
                errores.Add("La fecha de fin no puede ser anterior a la fecha de inicio.");
            }

            // Validar ProcessTime
            if (string.IsNullOrWhiteSpace(ProcessTime))
            {
                errores.Add("El tiempo de procesamiento no puede estar vacío.");
            }
            else if (!int.TryParse(ProcessTime, out _))
            {
                errores.Add("El tiempo de procesamiento debe ser un número entero.");
            }

            // Validar la existencia del directorio
            if (!Directory.Exists(Path))
            {
                errores.Add("La ruta especificada no es un directorio válido o no existe.");
            }

            // Validar que la rama de merge esté seleccionada si se activa AutoMerge
            if (AutoMerge && SelectedCbBranchMergeItem.Contains("Seleccione"))
            {
                errores.Add("Debe seleccionar una rama para la fusión automática.");
            }

            // Retorna la lista de errores (si está vacía, no hay errores)
            return errores;
        }

    }
}