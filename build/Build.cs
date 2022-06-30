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

    AbsolutePath DbUpDirectory => RootDirectory / "Database/CustomerApi.ServiceTests/bin/debug/net5.0/Database.exe";

    [LocalExecutable(@"C:\My\Programs\AutoBuild_BDD\Database\bin\Debug\net5.0\Database.exe")]
    readonly Tool dbup;

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
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
       .Executes(() =>
       {
           using var localDB = new SqlLocalDbApi();
           using TemporarySqlLocalDbInstance instance = localDB.CreateTemporaryInstance(deleteFiles: true);

           dbup.Invoke(instance.ConnectionString);

           DotNetTest(s => s
            .SetProjectFile(AutoBuild_BDD)
            .SetConfiguration(Configuration)
            .EnableNoRestore()
            .EnableNoBuild());

           
       });
}
