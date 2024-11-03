using CommitCompiler.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitCompiler.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public LogViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

    }
}
