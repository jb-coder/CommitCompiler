using CommitCompiler.Services.Interface;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CommitCompiler.Services
{
    public class ComboService : IComboService
    {

        public ComboService()
        {
        }

        public async Task<(ObservableCollection<string>,string)> LoadProjectsAsync(List<TeamProjectReference> projects)
        {
            var projectsString = projects.Select(x => x.Name).ToList();
            var result = new ObservableCollection<string>(projectsString);
            string firstElement = "Seleccione Proyecto...";
            result.Add(firstElement);

            return (result, firstElement);
        }

        public async Task<(ObservableCollection<string>, string)> LoadReposAsync(List<GitRepository> repositories, string selectedProject)
        {
            var result = new ObservableCollection<string>(repositories.Select(repo => repo.Name));
            string firstElement = "Seleccione Repositorio...";
            result.Add(firstElement);

            return (result, firstElement);
        }

        public async Task<(ObservableCollection<string>, string)> LoadBranchesAsync(List<GitRef> branches, string selectedProject, Guid? appId)
        {
            string firstElement = "Seleccione Rama...";

            if (!appId.HasValue)
                return (new ObservableCollection<string>(), firstElement);

            var result = new ObservableCollection<string>(branches.Select(branch => branch.Name.Replace("refs/heads/", "")));
            result.Add(firstElement);

            return (result, firstElement);
        }

    }
}
