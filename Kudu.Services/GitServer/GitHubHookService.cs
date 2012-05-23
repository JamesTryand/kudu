using System.Web;
using System.Web.Hosting;
using Kudu.Contracts.Infrastructure;
using Kudu.Contracts.Tracing;
using Kudu.Core.Deployment;
using Kudu.Core.SourceControl.Git;
using Newtonsoft.Json;

namespace Kudu.Services.GitServer
{
    public class GitHubHookService : IHttpHandler
    {
        private readonly IGitServer _gitServer;
        private readonly IDeploymentManager _deploymentManager;
        private readonly ITracer _tracer;
        private readonly IOperationLock _deploymentLock;

        public GitHubHookService(ITracer tracer,
                                 IGitServer gitServer,
                                 IDeploymentManager deploymentManager,
                                 IOperationLock deploymentLock)
        {
            _gitServer = gitServer;
            _deploymentManager = deploymentManager;
            _tracer = tracer;
            _deploymentLock = deploymentLock;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            using (_tracer.Step("GitHubHookService"))
            {
                string json = context.Request.Form["payload"];
                dynamic payload = JsonConvert.DeserializeObject(json);
                string repoUrl = payload.repository.url;

                _deploymentLock.LockOperation(() =>
                {
                    _gitServer.Pull(repoUrl);
                    _deploymentManager.Deploy(null);
                },
                () =>
                {
                    context.Response.StatusCode = 409;
                    context.ApplicationInstance.CompleteRequest();
                });
            }
        }
    }
}
