using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommitCompiler.Services
{
    public class AzureDevOpsService
    {
        private readonly VssConnection _connection;
        private readonly GitHttpClient _gitClient;
        private readonly ProjectHttpClient _projectClient;
        public AzureDevOpsService(string organization, string personalAccessToken)
        {
            // Establecer la conexión con Azure DevOps usando un PAT (Personal Access Token)
            var credentials = new VssBasicCredential(string.Empty, personalAccessToken);
            string uri = "https://dev.azure.com/" + organization;

            _connection = new VssConnection(new Uri(uri), credentials);

            // Inicializar los clientes para proyectos y repositorios
            _gitClient = _connection.GetClient<GitHttpClient>();
            _projectClient = _connection.GetClient<ProjectHttpClient>();
        }

        // Obtener todos los proyectos de la organización
        public async Task<List<TeamProjectReference>> GetProjectsAsync()
        {
            var projects = await _projectClient.GetProjects();
            return projects.ToList();
        }

        // Obtener los repositorios de un proyecto
        public async Task<List<GitRepository>> GetRepositoriesAsync(string projectName)
        {
            List<GitRepository> repositories = await _gitClient.GetRepositoriesAsync(projectName);
            return repositories;
        }
        // Obtener los commits de un repositorio (Sin criterios adicionales)
        public async Task<List<GitCommitRef>> GetCommitsAsync(string projectName, string repositoryId)
        {
            // La sobrecarga correcta que no requiere un criterio explícito
            List<GitCommitRef> commits = await _gitClient.GetCommitsAsync(projectName, repositoryId, new GitQueryCommitsCriteria());
            return commits;
        }

        // Obtener las ramas (refs/heads) de un repositorio
        public async Task<List<GitRef>> GetBranchesAsync(string projectName, string repositoryId)
        {
            List<GitRef> branches = await _gitClient.GetRefsAsync(projectName, repositoryId, filter: "heads/");
            return branches;
        }

        // Obtener los commits de una rama específica en un repositorio
        public async Task<List<GitCommitRef>> GetCommitsByBranchAsync(string projectName, string repositoryId, string branchName, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Asegurarse de que la fecha de inicio no sea posterior a la fecha de fin
                if (startDate > endDate)
                {
                    throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha de fin.");
                }

                // Definir los criterios de búsqueda basados en el ObjectId de la rama
                GitQueryCommitsCriteria criteria = new GitQueryCommitsCriteria
                {
                    ItemVersion = new GitVersionDescriptor
                    {
                        VersionType = GitVersionType.Branch,
                        Version = branchName
                    },
                    Top = 10000,  // Establecemos un límite en la cantidad de commits a devolver
                    FromDate = startDate.ToString("o"), // Formato de fecha ISO 8601
                    ToDate = endDate.ToString("o")      // Formato de fecha ISO 8601
                };

                // Llamada a la API para obtener los commits filtrados
                List<GitCommitRef> filteredCommits = await _gitClient.GetCommitsAsync(projectName, repositoryId, criteria);

                if (filteredCommits.Count == 0)
                {
                    throw new Exception("No se encontraron commits en el rango de fechas especificado.");
                }

                // Devolver los commits encontrados
                return filteredCommits;
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y mostrar el error
                Console.WriteLine($"Error al obtener commits: {ex.Message}");
                throw;
            }
        }





    }
}
