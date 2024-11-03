using System;
using System.IO;
using CommitCompiler.Services;
using Newtonsoft.Json;
using Xunit;

namespace CommitCompiler.Tests.Services
{
    public class UserCredentialsTests : IDisposable
    {
        private readonly string testCredentialsFilePath;

        public UserCredentialsTests()
        {
            // Configurar una ruta de archivo temporal para las pruebas
            testCredentialsFilePath = Path.Combine(
                Path.GetTempPath(), "CommitCompiler", "userCredentials_test.json");

            // Reemplazar la ruta de archivo en la clase `UserCredentials`
            typeof(UserCredentials)
                .GetField("credentialsFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .SetValue(null, testCredentialsFilePath);
        }

        // Limpiar después de cada prueba
        public void Dispose()
        {
            if (File.Exists(testCredentialsFilePath))
            {
                File.Delete(testCredentialsFilePath);
            }
        }

        [Fact]
        public void SaveCredentials_ShouldCreateFileWithCorrectData()
        {
            // Arrange
            string organization = "TestOrg";
            string token = "TestToken";

            // Act
            UserCredentials.SaveCredentials(organization, token);

            // Assert
            Assert.True(File.Exists(testCredentialsFilePath));

            var json = File.ReadAllText(testCredentialsFilePath);
            var savedCredentials = JsonConvert.DeserializeObject<UserCredentials>(json);

            Assert.Equal(organization, savedCredentials.Organization);
            Assert.Equal(token, savedCredentials.Token);
        }

        [Fact]
        public void LoadCredentials_ShouldReturnCorrectData_WhenFileExists()
        {
            // Arrange
            var expectedCredentials = new UserCredentials
            {
                Organization = "TestOrg",
                Token = "TestToken"
            };

            // Guardar credenciales simuladas
            File.WriteAllText(testCredentialsFilePath, JsonConvert.SerializeObject(expectedCredentials));

            // Act
            var loadedCredentials = UserCredentials.LoadCredentials();

            // Assert
            Assert.NotNull(loadedCredentials);
            Assert.Equal(expectedCredentials.Organization, loadedCredentials.Organization);
            Assert.Equal(expectedCredentials.Token, loadedCredentials.Token);
        }

        [Fact]
        public void LoadCredentials_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Act
            var loadedCredentials = UserCredentials.LoadCredentials();

            // Assert
            Assert.Null(loadedCredentials);
        }

        [Fact]
        public void DeleteCredentials_ShouldDeleteFile_WhenFileExists()
        {
            // Arrange
            File.WriteAllText(testCredentialsFilePath, "test content");

            // Act
            UserCredentials.DeleteCredentials();

            // Assert
            Assert.False(File.Exists(testCredentialsFilePath));
        }

        [Fact]
        public void DeleteCredentials_ShouldDoNothing_WhenFileDoesNotExist()
        {
            // Act
            var exception = Record.Exception(() => UserCredentials.DeleteCredentials());

            // Assert
            Assert.Null(exception); // No debería lanzar excepción
            Assert.False(File.Exists(testCredentialsFilePath));
        }
    }
}
