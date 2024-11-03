using CommitCompiler.Services;
using CommitCompiler.Services.Interface;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using CommitCompiler.Models;
using System.Windows.Controls;
using System.ComponentModel;
using CommitCompiler.Views;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.Azure.Pipelines.WebApi;
using HandyControl.Tools;
using Microsoft.TeamFoundation.Core.WebApi;

namespace CommitCompiler.ViewModels
{
    public class DashBoardViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IComboService _comboService;

        private AzureDevOpsService _azureDevOpsService;

        public SeriesCollection Series { get; set; }

        private Guid? _idApplication;
        private string _selectedCbRepoItem;
        private string _selectedCbBranchItem;
        private string _selectedCbProjectItem;
        private ObservableCollection<string> _cbRepoItems;
        private ObservableCollection<string> _cbBranchItems;
        private ObservableCollection<string> _cbProjectItems;
        private DateTime _startDate;
        private DateTime _endDate = DateTime.Now;
        private string _totalCommit;
        private string _totalCommitAdd;
        private string _totalCommitFix;

        private List<TeamProjectReference> _projects;
        private List<GitRepository> _repositories;
        private List<GitRef> _branches;
        private List<GitCommitRef> _commits;

        public string TotalCommit
        {
            get => _totalCommit;
            set
            {
                if (_totalCommit != value)
                {
                    _totalCommit = value;
                    OnPropertyChanged(nameof(TotalCommit));
                }
            }
        }

        public string TotalCommitAdd
        {
            get => _totalCommitAdd;
            set
            {
                if (_totalCommitAdd != value)
                {
                    _totalCommitAdd = value;
                    OnPropertyChanged(nameof(TotalCommitAdd));
                }
            }
        }

        public string TotalCommitFix
        {
            get => _totalCommitFix;
            set
            {
                if (_totalCommitFix != value)
                {
                    _totalCommitFix = value;
                    OnPropertyChanged(nameof(TotalCommitFix));
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                    LoadCommitForSelectedDate();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                    LoadCommitForSelectedDate();
                }
            }
        }

        private Dictionary<string, Brush> _authorColors = new Dictionary<string, Brush>();
        private List<Brush> _colorPalette = new List<Brush>
        {
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Yellow,
            Brushes.Orange,
            Brushes.Purple,
            Brushes.Cyan,
            Brushes.Magenta
        };

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
                LoadCommitForSelectedBranch();
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

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        private SeriesCollection _seriesCollection2;
        public SeriesCollection SeriesCollection2
        {
            get => _seriesCollection2;
            set
            {
                _seriesCollection2 = value;
                OnPropertyChanged(nameof(SeriesCollection2));
            }
        }

        private SeriesCollection _seriesCollection3;
        public SeriesCollection SeriesCollection3
        {
            get => _seriesCollection3;
            set
            {
                _seriesCollection3 = value;
                OnPropertyChanged(nameof(SeriesCollection3));
            }
        }

        public DashBoardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _azureDevOpsService = new AzureDevOpsService(App.GlobalUserInput.Organization,
                                                            App.GlobalUserInput.PersonalAccessToken);
            _comboService = new ComboService();

            LoadProject();
        }

        private async Task LoadProject()
        {
            _projects = await _azureDevOpsService.GetProjectsAsync();
            (CbProjectItems,SelectedCbProjectItem) = await _comboService.LoadProjectsAsync(_projects);
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

        private async Task LoadCommitForSelectedBranch()
        {
            if (string.IsNullOrEmpty(SelectedCbBranchItem) || SelectedCbBranchItem == "Seleccione Rama..." || !_idApplication.HasValue)
            {
                return; // Asegúrate de que _idApplication tenga un valor
            }

            _commits = await _azureDevOpsService.GetCommitsByBranchAsync(SelectedCbProjectItem, 
                                                                        _idApplication.Value.ToString(), 
                                                                        SelectedCbBranchItem, 
                                                                        StartDate, 
                                                                        EndDate);

            InitialSeriesCollection(); // Actualiza los gráficos después de cargar los commits
        }

        private async Task LoadCommitForSelectedDate()
        {
            if (string.IsNullOrEmpty(SelectedCbBranchItem) || SelectedCbBranchItem == "Seleccione Rama..." || !_idApplication.HasValue)
            {
                return; // Asegúrate de que _idApplication tenga un valor
            }

            _commits = await _azureDevOpsService.GetCommitsByBranchAsync(SelectedCbProjectItem, _idApplication.Value.ToString(), SelectedCbBranchItem, StartDate, EndDate);

            InitialSeriesCollection(); // Actualiza los gráficos después de cargar los commits
        }

        private void InitialSeriesCollection()
        {
            if (_commits.Count == 0) { return; }

            // Agrupar commits por autor y contar cuántos commits hizo cada autor
            var commitsByAuthor = _commits
                .Where(x => x.Comment.Contains("ADD") || x.Comment.Contains("FIX"))
                .GroupBy(commit => commit.Author.Name)
                .Select(group => new
                {
                    AuthorName = group.Key,
                    CommitCount = group.Count()
                });

            var commitsAddByAuthor = _commits
                .Where(x => x.Comment.Contains("ADD"))
                .GroupBy(commit => commit.Author.Name)
                .Select(group => new
                {
                    AuthorName = group.Key,
                    CommitCount = group.Count()
                });

            var commitsFixByAuthor = _commits
                .Where(x => x.Comment.Contains("FIX"))
                .GroupBy(commit => commit.Author.Name)
                .Select(group => new
                {
                    AuthorName = group.Key,
                    CommitCount = group.Count()
                });

            TotalCommit = _commits
                            .Where(x => x.Comment.Contains("FIX") || x.Comment.Contains("ADD"))
                            .Count().ToString();
            TotalCommitAdd = _commits
                            .Where(x => x.Comment.Contains("ADD"))
                            .Count().ToString();
            TotalCommitFix = _commits
                            .Where(x => x.Comment.Contains("FIX"))
                            .Count().ToString();

            // Crear la serie de gráficos usando los datos agrupados
            SeriesCollection = new SeriesCollection();

            foreach (var author in commitsByAuthor)
            {
                SeriesCollection.Add(new PieSeries
                {
                    Title = author.AuthorName, // Nombre del autor
                    Values = new ChartValues<ObservableValue> { new ObservableValue(author.CommitCount) },
                    DataLabels = true,
                    Fill = GetColorForAuthor(author.AuthorName)
                });
            }

            SeriesCollection2 = new SeriesCollection();

            foreach (var author in commitsAddByAuthor)
            {
                SeriesCollection2.Add(new PieSeries
                {
                    Title = author.AuthorName, // Nombre del autor
                    Values = new ChartValues<ObservableValue> { new ObservableValue(author.CommitCount) },
                    DataLabels = true,
                    Fill = GetColorForAuthor(author.AuthorName)
                });
            }

            SeriesCollection3 = new SeriesCollection();

            foreach (var author in commitsFixByAuthor)
            {
                SeriesCollection3.Add(new PieSeries
                {
                    Title = author.AuthorName, // Nombre del autor
                    Values = new ChartValues<ObservableValue> { new ObservableValue(author.CommitCount) },
                    DataLabels = true,
                    Fill = GetColorForAuthor(author.AuthorName)
                });
            }
        }

        // Método para asignar colores dinámicamente a los autores
        private Brush GetColorForAuthor(string authorName)
        {
            // Si el autor ya tiene un color asignado, devolvemos ese color
            if (_authorColors.ContainsKey(authorName))
            {
                return _authorColors[authorName];
            }

            // Si no tiene color asignado, tomamos el siguiente color de la lista
            var colorIndex = _authorColors.Count % _colorPalette.Count;
            var assignedColor = _colorPalette[colorIndex];

            // Asignamos el color al autor y lo guardamos en el diccionario
            _authorColors[authorName] = assignedColor;

            return assignedColor;
        }




    }
}





//Series = new SeriesCollection
//{
//    new StackedAreaSeries
//    {
//        Values = new ChartValues<double> {20, 30, 35, 45, 65, 85},
//        Title = "Javier Baena Santa-Cruz"
//    },
//    new StackedAreaSeries
//    {
//        Values = new ChartValues<double> {10, 12, 18, 20, 38, 40},
//        Title = "Jose Antonio Martos Pozo"
//    },
//    new StackedAreaSeries
//    {
//        Values = new ChartValues<double> {5, 8, 12, 15, 22, 25},
//        Title = "Alejandro Galán Prados"
//    }
//};

//SeriesCollection4 = new SeriesCollection
//{
//    new PieSeries
//    {
//        Title = "Current",
//        Values = new ChartValues<ObservableValue> { new ObservableValue(45) },
//        DataLabels = true,

//    },
//    new PieSeries
//    {
//        Title = "Target",
//        Values = new ChartValues<ObservableValue> { new ObservableValue(25) },
//        DataLabels = true,

//    },
//     new PieSeries
//    {
//        Title = "Lost",
//        Values = new ChartValues<ObservableValue> { new ObservableValue(30) },
//        DataLabels = true,

//    },
//};
//DataContext = this;
// Método opcional para asignar colores a los autores