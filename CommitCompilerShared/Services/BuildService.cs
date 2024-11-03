using CommitCompilerShared.Data;
using CommitCompilerShared.Models;
using LibGit2Sharp;
using Microsoft.EntityFrameworkCore;

namespace CommitCompilerShared.Services
{
    public class BuildService
    {
    private readonly CommitCompilerContext _dbContext;

    public BuildService(CommitCompilerContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteBuildProcess()
    {
        // Obtiene la configuración más reciente
        var buildConfiguration = await _dbContext.BuildConfigurations
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();

        if (buildConfiguration != null)
        {
            // Configuraciones de Git
            string repositoryPath = "C:/"; // Ruta local del repositorio
            string branch = buildConfiguration.Branch; // Rama a verificar
            string destinationPath = buildConfiguration.PathDestination; // Ruta de destino para la compilación

            // Verifica nuevos commits
            bool hasNewCommits = await CheckForNewCommits(repositoryPath, branch, buildConfiguration.Token);

            if (hasNewCommits)
            {
                // Lógica de compilación
                CompileProject(destinationPath);

                // Enviar notificación
                await SendNotification(buildConfiguration);
            }
            else
            {
                Console.WriteLine("No hay nuevos commits en la rama.");
            }
        }
        else
        {
            Console.WriteLine("No se encontró configuración de compilación.");
        }
    }

    private async Task<bool> CheckForNewCommits(string repoPath, string branch, string token)
    {
        // Aquí implementas la lógica para conectarte a Git y verificar nuevos commits.
        using (var repo = new Repository(repoPath))
        {
            var remote = repo.Network.Remotes.FirstOrDefault();
            var branchRef = repo.Branches[branch];

            if (branchRef == null)
            {
                Console.WriteLine("La rama especificada no existe.");
                return false;
            }

            // Fetch para actualizar la información del remoto
            Commands.Fetch(repo, remote.Name, null, null, null);

            // Compara los commits
            var localCommit = branchRef.Commits.First();
            var remoteCommit = repo.Branches[$"origin/{branch}"].Commits.First();

            return localCommit.Id != remoteCommit.Id; // Hay un nuevo commit
        }
    }

    private void CompileProject(string path)
    {
        // Aquí implementaremos la lógica para compilar el proyecto.
        Console.WriteLine($"Compilando el proyecto en {path}...");

        //Logica Compilacion *************
                

        Console.WriteLine("Compilación completada.");
    }

    private async Task SendNotification(BuildConfiguration config)
    {
        // Lógica para enviar un mensaje.
        Console.WriteLine($"Enviando notificación a {config.EmailDestination}...");
        // Código para enviar el correo (MailKit o System.Net.Mail).

        // Envio De mail *******************


        Console.WriteLine("Notificación enviada.");
    }
    }
}
