using CommitCompilerShared.Data;
using CommitCompilerShared.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace CommitCompilerShared.Services
{
    public class BuildService
    {
        private readonly CommitCompilerContext _dbContext;
        private readonly HttpClient _httpClient;

        public BuildService(CommitCompilerContext dbContext)
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient();
        }

        public async Task ExecuteBuildProcess()
        {
            var buildConfigurations = await _dbContext.BuildConfigurations
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            if (buildConfigurations == null || !buildConfigurations.Any())
            {
                Console.WriteLine("No se ha encontrado ninguna configuración");
                return;
            }

            foreach (var config in buildConfigurations)
            {
                try
                {
                    Console.WriteLine($"Procesando configuración para el proyecto: {config.Repository}");
                    string repositoryPath = @"C:\Temp\Compilaciones";
                    Directory.CreateDirectory(repositoryPath);

                    // Verificar si hay nuevos commits y descargar si es necesario
                    bool hasNewCommits = await CheckForNewCommits(config);

                    if (hasNewCommits)
                    {
                        CompileProject(config.PathDestination);
                        await SendNotification(config);
                    }
                    else
                    {
                        Console.WriteLine("No hay nuevos commits en la rama.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error durante el proceso de compilación: {ex.Message}");
                }
            }
        }

        private async Task<bool> CheckForNewCommits(BuildConfiguration config)
        {
            try
            {
                // Autenticación utilizando el token de acceso personal (PAT)
                string pat = config.Token;
                string baseUrl = "https://dev.azure.com"; // URL base de Azure DevOps
                string organization = config.Organization;
                string project = config.Project;
                string repository = config.Repository;
                string branch = config.Branch;

                // Configurar las cabeceras de autenticación para Azure DevOps
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

                // Llamada a la API REST de Azure DevOps para obtener el último commit en la rama
                string url = $"{baseUrl}/{organization}/{project}/_apis/git/repositories/{repository}/commits?searchCriteria.itemVersion.version={branch}&$top=1&api-version=7.1-preview.1";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var commits = await response.Content.ReadAsAsync<dynamic>();

                    // Si no hay commits, devolver false
                    if (commits.count == 0)
                    {
                        Console.WriteLine("No se encontraron commits en la rama.");
                        return false;
                    }

                    // Comprobar si hay un nuevo commit (compara el último commit remoto con el commit local)
                    var latestCommit = commits.value[0].commitId;
                    var localCommitId = GetLocalCommitId(repository, branch);

                    return localCommitId != latestCommit;
                }
                else
                {
                    Console.WriteLine("Error al obtener los commits.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar commits: {ex.Message}");
                return false;
            }
        }

        private string GetLocalCommitId(string repoPath, string branch)
        {
            // Aquí debes implementar la lógica para obtener el último commit de la rama local
            // Usando comandos Git a través de Process Start (por ejemplo, git rev-parse HEAD)
            return string.Empty;
        }

        private async Task CloneRepository(string repoUrl, string localPath, string token)
        {
            try
            {
                string baseUrl = "https://dev.azure.com";
                string organization = "your-organization"; // Cambia esto por tu organización
                string project = "your-project"; // Cambia esto por tu proyecto
                string repository = "your-repo"; // Cambia esto por el nombre de tu repositorio

                // Autenticación utilizando el token de acceso personal (PAT)
                string cloneUrl = $"{baseUrl}/{organization}/{project}/_git/{repository}";
                string cloneCommand = $"git clone https://{organization}:{token}@dev.azure.com/{organization}/{project}/_git/{repository} {localPath}";

                // Ejecutar el comando git clone
                Console.WriteLine("Clonando el repositorio...");
                await Task.Run(() => Process.Start("git", cloneCommand));
                Console.WriteLine("Repositorio clonado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al clonar el repositorio: {ex.Message}");
            }
        }

        private void CompileProject(string outputPath)
        {
            Console.WriteLine($"Compilando el proyecto en {outputPath}...");

            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"publish -c Release -o {outputPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Error en la compilación: {error}");
                    }
                    else
                    {
                        Console.WriteLine("Compilación completada exitosamente.");
                    }
                }
            }
        }

        private async Task SendNotification(BuildConfiguration config)
        {
            try
            {
                using (var client = new SmtpClient("smtp.example.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(config.EmailOriginSender, config.EmailOriginPass);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(config.EmailOriginSender),
                        Subject = config.EmailDestinationSubject,
                        Body = "El proyecto ha sido compilado y actualizado correctamente.",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(config.EmailDestination);

                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine("Notificación enviada con éxito.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar notificación: {ex.Message}");
            }
        }
    }
}
