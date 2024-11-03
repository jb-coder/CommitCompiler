using CommitCompiler.Services;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Moq;
using Xunit;

namespace CommitCompiler.Tests.Services
{
    public class AzureDevOpsServiceTests
    {
        private readonly Mock<GitHttpClient> _mockGitClient;
        private readonly Mock<ProjectHttpClient> _mockProjectClient;
        private readonly AzureDevOpsService _service;

    //    public AzureDevOpsServiceTests()
    //    {
    //        // Crear mocks para los clientes de Azure DevOps
    //        _mockGitClient = new Mock<GitHttpClient>(MockBehavior.Strict, new Uri("http://localhost"), new VssCredentials());
    //        _mockProjectClient = new Mock<ProjectHttpClient>(MockBehavior.Strict, new Uri("http://localhost"), new VssCredentials());

    //        // Inyectar los mocks en el servicio (Usamos Reflection para establecerlos)
    //        _service = new AzureDevOpsService("testOrg", "testToken");
    //        typeof(AzureDevOpsService)
    //            .GetField("_gitClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    //            .SetValue(_service, _mockGitClient.Object);
    //        typeof(AzureDevOpsService)
    //            .GetField("_projectClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    //            .SetValue(_service, _mockProjectClient.Object);
    //    }

    //    [Fact]
    //    public async Task GetProjectsAsync_ShouldReturnProjects()
    //    {
    //        // Arrange
    //        var projects = new List<TeamProjectReference>
    //{
    //    new TeamProjectReference { Id = Guid.NewGuid(), Name = "Project1" },
    //    new TeamProjectReference { Id = Guid.NewGuid(), Name = "Project2" }
    //};

    //        _mockProjectClient
    //            .Setup(client => client.GetProjects(It.IsAny<ProjectState?>(), It.IsAny<int?>(), It.IsAny<object>()))
    //            .ReturnsAsync(new PagedList<TeamProjectReference>(projects, continuationToken: null));

    //        // Act
    //        var result = await _service.GetProjectsAsync();

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(2, result.Count);
    //        Assert.Equal("Project1", result[0].Name);
    //        Assert.Equal("Project2", result[1].Name);
    //    }





    //    [Fact]
    //    public async Task GetRepositoriesAsync_ShouldReturnRepositories()
    //    {
    //        // Arrange
    //        string projectName = "Project1";
    //        var repositories = new List<GitRepository>
    //        {
    //            new GitRepository { Id = Guid.NewGuid(), Name = "Repo1" },
    //            new GitRepository { Id = Guid.NewGuid(), Name = "Repo2" }
    //        };
    //        _mockGitClient
    //            .Setup(client => client.GetRepositoriesAsync(projectName))
    //            .ReturnsAsync(repositories);

    //        // Act
    //        var result = await _service.GetRepositoriesAsync(projectName);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(2, result.Count);
    //        Assert.Equal("Repo1", result[0].Name);
    //        Assert.Equal("Repo2", result[1].Name);
    //    }

    //    [Fact]
    //    public async Task GetCommitsAsync_ShouldReturnCommits()
    //    {
    //        // Arrange
    //        string projectName = "Project1";
    //        string repositoryId = "Repo1";
    //        var commits = new List<GitCommitRef>
    //        {
    //            new GitCommitRef { CommitId = "12345" },
    //            new GitCommitRef { CommitId = "67890" }
    //        };
    //        _mockGitClient
    //            .Setup(client => client.GetCommitsAsync(projectName, repositoryId, It.IsAny<GitQueryCommitsCriteria>()))
    //            .ReturnsAsync(commits);

    //        // Act
    //        var result = await _service.GetCommitsAsync(projectName, repositoryId);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(2, result.Count);
    //        Assert.Equal("12345", result[0].CommitId);
    //        Assert.Equal("67890", result[1].CommitId);
    //    }

    //    [Fact]
    //    public async Task GetBranchesAsync_ShouldReturnBranches()
    //    {
    //        // Arrange
    //        string projectName = "Project1";
    //        string repositoryId = "Repo1";
    //        var branches = new List<GitRef>
    //        {
    //            new GitRef { Name = "refs/heads/main" },
    //            new GitRef { Name = "refs/heads/develop" }
    //        };
    //        _mockGitClient
    //            .Setup(client => client.GetRefsAsync(projectName, repositoryId, "heads/"))
    //            .ReturnsAsync(branches);

    //        // Act
    //        var result = await _service.GetBranchesAsync(projectName, repositoryId);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(2, result.Count);
    //        Assert.Equal("refs/heads/main", result[0].Name);
    //        Assert.Equal("refs/heads/develop", result[1].Name);
    //    }

    //    [Fact]
    //    public async Task GetCommitsByBranchAsync_ShouldReturnFilteredCommits()
    //    {
    //        // Arrange
    //        string projectName = "Project1";
    //        string repositoryId = "Repo1";
    //        string branchName = "main";
    //        DateTime startDate = DateTime.Now.AddDays(-10);
    //        DateTime endDate = DateTime.Now;
    //        var commits = new List<GitCommitRef>
    //        {
    //            new GitCommitRef { CommitId = "11111" },
    //            new GitCommitRef { CommitId = "22222" }
    //        };

    //        _mockGitClient
    //            .Setup(client => client.GetCommitsAsync(projectName, repositoryId, It.IsAny<GitQueryCommitsCriteria>()))
    //            .ReturnsAsync(commits);

    //        // Act
    //        var result = await _service.GetCommitsByBranchAsync(projectName, repositoryId, branchName, startDate, endDate);

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Equal(2, result.Count);
    //        Assert.Equal("11111", result[0].CommitId);
    //        Assert.Equal("22222", result[1].CommitId);
    //    }

    //    [Fact]
    //    public async Task GetCommitsByBranchAsync_ShouldThrowArgumentException_WhenStartDateIsAfterEndDate()
    //    {
    //        // Arrange
    //        string projectName = "Project1";
    //        string repositoryId = "Repo1";
    //        string branchName = "main";
    //        DateTime startDate = DateTime.Now;
    //        DateTime endDate = DateTime.Now.AddDays(-10);

    //        // Act & Assert
    //        await Assert.ThrowsAsync<ArgumentException>(() =>
    //            _service.GetCommitsByBranchAsync(projectName, repositoryId, branchName, startDate, endDate));
    //    }
    }
}
