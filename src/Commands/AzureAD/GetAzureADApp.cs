using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PnP.PowerShell.Commands.Attributes;
using PnP.PowerShell.Commands.Base;
using PnP.PowerShell.Commands.Base.PipeBinds;
using PnP.PowerShell.Commands.Model;
using PnP.PowerShell.Commands.Utilities.REST;

namespace PnP.PowerShell.Commands.AzureAD
{
    [Cmdlet(VerbsCommon.Get, "PnPAzureADApp")]
    [RequiredMinimalApiPermissions("Application.Read.All")]
    public class GetAzureADApp : PnPGraphCmdlet
    {
        [Parameter(Mandatory = false)]
        public AzureADAppPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSpecified(nameof(Identity)))
            {
                WriteObject(Identity.GetApp(HttpClient, AccessToken));
            }
            else
            {
                List<AzureADApp> apps = new List<AzureADApp>();
                var result = Utilities.REST.GraphHelper.GetAsync<RestResultCollection<AzureADApp>>(HttpClient, "/v1.0/applications", AccessToken).GetAwaiter().GetResult();
                if (result != null && result.Items.Any())
                {
                    apps.AddRange(result.Items);
                    while (!string.IsNullOrEmpty(result.NextLink))
                    {
                        result = Utilities.REST.GraphHelper.GetAsync<RestResultCollection<AzureADApp>>(HttpClient, result.NextLink, AccessToken).GetAwaiter().GetResult();
                        if (result != null && result.Items.Any())
                        {
                            apps.AddRange(result.Items);
                        }
                    }
                }
                WriteObject(apps, true);
            }
        }
    }
}