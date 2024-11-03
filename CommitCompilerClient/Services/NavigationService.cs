// NavigationService.cs
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommitCompiler.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace CommitCompiler.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(Frame frame, IServiceProvider serviceProvider)
        {
            _frame = frame;
            _serviceProvider = serviceProvider;
        }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetMainFrame(Frame frame)
        {
            _frame = frame;
        }
        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            // Resolvemos el ViewModel desde el contenedor de dependencias
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // Asumimos que la vista correspondiente al ViewModel es una Page con el mismo nombre (reemplazando "ViewModel" por "Page")
            var viewTypeName = typeof(TViewModel).Name.Replace("ViewModel", "Page");
            var viewType = Type.GetType($"CommitCompiler.Views.{viewTypeName}");

            if (viewType == null)
            {
                throw new InvalidOperationException($"No se pudo encontrar la vista para el ViewModel {typeof(TViewModel).Name}");
            }

            // Crear una instancia de la Page
            var view = _serviceProvider.GetService(viewType) as Page;

            if (view == null)
            {
                throw new InvalidOperationException($"La vista {viewTypeName} no es del tipo Page.");
            }

            // Establecemos el DataContext de la vista con el ViewModel
            view.DataContext = viewModel;

            // Navegamos a la nueva Page en el Frame
            _frame.Navigate(view);
        }

        //public void NavigateTo<TViewModel>() where TViewModel : class
        //{
        //    var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        //    _contentControl.Content = null; // Limpia el contenido actual
        //    _contentControl.DataContext = viewModel; // Establece el DataContext en el nuevo ViewModel

        //    // Aquí verificamos que el DataTemplate se resuelva automáticamente.
        //    // Si no, podríamos instanciar manualmente la vista y asignarla.
        //    if (_contentControl.Content == null && !typeof(TViewModel).Name.Contains("window"))
        //    {
        //        var viewType = typeof(TViewModel).Name.Replace("ViewModel", "Page");
        //        var view = _serviceProvider.GetService(Type.GetType($"CommitCompiler.Views.{viewType}"));
        //        _contentControl.Content = view;
        //    }
        //}
    }

}

