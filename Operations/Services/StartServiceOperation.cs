﻿using System.ComponentModel;
using System.ServiceProcess;
using System.Threading.Tasks;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Documentation;
using Inedo.BuildMaster.Extensibility;
using Inedo.BuildMaster.Extensibility.Agents;
using Inedo.BuildMaster.Extensibility.Operations;
using Inedo.Diagnostics;

namespace Inedo.BuildMasterExtensions.Windows.Operations.Services
{
    [DisplayName("Start Windows Service")]
    [Description("Starts an existing Windows service.")]
    [DefaultProperty(nameof(ServiceName))]
    [ScriptAlias("Start-Service")]
    [Tag("services")]
    [Example(@"# starts the HDARS service on the remote server
Start-Service HDARS;")]
    [ScriptNamespace("Windows", PreferUnqualified = true)]
    public sealed class StartServiceOperation : ExecuteOperation
    {
        [ScriptAlias("Name")]
        public string ServiceName { get; set; }
        [ScriptAlias("WaitForRunningStatus")]
        public bool WaitForRunningStatus { get; set; } = true;

        public override Task ExecuteAsync(IOperationExecutionContext context)
        {
            this.LogInformation($"Starting service {this.ServiceName}...");
            if (context.Simulation)
            {
                this.LogInformation("Service is running.");
                return Complete;
            }

            var jobExecuter = context.Agent.GetService<IRemoteJobExecuter>();
            var job = new ControlServiceJob { ServiceName = this.ServiceName, TargetStatus = ServiceControllerStatus.Running, WaitForTargetStatus = this.WaitForRunningStatus };
            return jobExecuter.ExecuteJobAsync(job, context.CancellationToken);
        }

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            return new ExtendedRichDescription(
                new RichDescription(
                    "Start ",
                    new BuildMaster.Documentation.Hilite(config[nameof(ServiceName)]),
                    " service"
                )
            );
        }
    }
}
