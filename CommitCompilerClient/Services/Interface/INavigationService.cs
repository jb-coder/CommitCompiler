﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommitCompiler.Services.Interface
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : class;
        void SetMainFrame(Frame frame);
    }

}
