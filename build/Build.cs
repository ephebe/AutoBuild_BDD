using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

using static Nuke.Common.ControlFlow;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using MartinCostello.SqlLocalDb;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution("AutoBuild_BDD.sln")] readonly Solution AutoBuild_BDD;

    AbsolutePath SourceDirectory => RootDirectory / "src/CustomerApi";
    AbsolutePath TestsDirectory => RootDirectory / "src/CustomerApi.ServiceTests";

    AbsolutePath DbUpDirectory => RootDirectory / "Database";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            DbUpDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(AutoBuild_BDD)
                .EnableNoCache());
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(AutoBuild_BDD)
                .SetConfiguration(Configuration)
                .EnableNoLogo()
                .EnableNoRestore());
        });


    Target Test => _ => _
       .DependsOn(Compile)
       .Executes(async () =>
       {
           using var localDB = new SqlLocalDbApi();
           var dbInstance = localDB.CreateInstance("AutoBuild");
           ISqlLocalDbInstanceManager manager = dbInstance.Manage();
           if (!dbInstance.IsRunning)
           {
               manager.Start();
           }

           using (var connection = dbInstance.CreateConnection())
           {
               await connection.OpenAsync();
               var command = connection.CreateCommand();
               command.CommandType = System.Data.CommandType.Text;
               command.CommandText = "CREATE DATABASE BDD;";
               await command.ExecuteNonQueryAsync();
           }

           DotNetRun(s => s
           .SetProjectFile(DbUpDirectory)
           .EnableNoBuild()
           .EnableNoRestore());

           DotNetTest(s => s
            .SetProjectFile(AutoBuild_BDD)
            .SetConfiguration(Configuration)
            .EnableNoRestore()
            .EnableNoBuild());

           using (var connection = dbInstance.CreateConnection())
           {
               await connection.OpenAsync();
               var command = connection.CreateCommand();
               command.CommandType = System.Data.CommandType.Text;
               command.CommandText = "DROP DATABASE BDD;";
               await command.ExecuteNonQueryAsync();
           }

           manager.Stop();
           localDB.DeleteInstance("AutoBuild", true);

       });
}
