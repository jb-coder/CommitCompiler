using CommitCompiler.Services;
using CommitCompiler.Services.Interface;
using CommitCompilerShared.Data;
using CommitCompilerShared.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace CommitCompiler.ViewModels
{
    public class ProcessViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly BuildConfigurationService _buildConfigurationService;

        // Colección observable para enlazar al DataGrid
        public ObservableCollection<BuildConfiguration> Processes { get; set; }
        public ProcessViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CommitCompilerContext context = new CommitCompilerContext();
            _buildConfigurationService = new BuildConfigurationService(context);

            // Inicializar la colección y cargar los datos
            Processes = new ObservableCollection<BuildConfiguration>();
            Task task = LoadProcesses();
        }
        private async Task LoadProcesses()
        {
            try
            {
                var configurations = await _buildConfigurationService.GetAllConfigurationsAsync();
                Processes.Clear();

                // Agregar los elementos a la colección observable
                foreach (var config in configurations)
                {
                    Processes.Add(config);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar configuraciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
