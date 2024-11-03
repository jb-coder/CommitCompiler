using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.ObjectModel;

namespace CommitCompiler.Services.Interface
{
    public interface IComboService
    {
        Task<(ObservableCollection<string>,string)> LoadProjectsAsync(List<TeamProjectReference> projects);
        Task<(ObservableCollection<string>, string)> LoadReposAsync(List<GitRepository> repositories,string selectedProject);
        Task<(ObservableCollection<string>, string)> LoadBranchesAsync(List<GitRef> branches, string selectedProject, Guid? appId);
    }
}