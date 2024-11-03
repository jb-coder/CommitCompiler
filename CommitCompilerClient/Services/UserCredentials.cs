using System.IO;
using Newtonsoft.Json;

namespace CommitCompiler.Services
{
    public class UserCredentials
    {
        public string Organization { get; set; }
        public string Token { get; set; }

        // Ruta donde se almacenarán las credenciales
        private static string credentialsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CommitCompiler", "userCredentials.json");

        // Guardar credenciales en un archivo JSON
        public static void SaveCredentials(string organization, string token)
        {
            var credentials = new UserCredentials
            {
                Organization = organization,
                Token = token
            };

            // Crear el directorio si no existe
            var directory = Path.GetDirectoryName(credentialsFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Guardar el archivo JSON
            File.WriteAllText(credentialsFilePath, JsonConvert.SerializeObject(credentials));
        }

        // Cargar credenciales desde el archivo JSON
        public static UserCredentials LoadCredentials()
        {
            if (File.Exists(credentialsFilePath))
            {
                var json = File.ReadAllText(credentialsFilePath);
                return JsonConvert.DeserializeObject<UserCredentials>(json);
            }
            return null;
        }

        // Eliminar credenciales (en caso de que el usuario desmarque la opción de recordar credenciales)
        public static void DeleteCredentials()
        {
            if (File.Exists(credentialsFilePath))
            {
                File.Delete(credentialsFilePath);
            }
        }
    }
}
