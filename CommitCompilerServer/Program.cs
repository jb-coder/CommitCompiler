using CommitCompilerShared.Data;
using CommitCompilerShared.Services;

class Program
{
    static async Task Main(string[] args)
    {
        CommitCompilerContext context = new CommitCompilerContext();
        var buildService = new BuildService(context);

        await buildService.ExecuteBuildProcess();
    }
}
