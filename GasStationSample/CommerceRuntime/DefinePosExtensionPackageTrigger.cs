namespace GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    public class DefinePosExtensionPackageTrigger : IRequestTrigger
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetExtensionPackageDefinitionsRequest)};
            }
        }

        public void OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            var getExtensionsResponse = (GetExtensionPackageDefinitionsResponse)response;
            var extensionPackageDefinition = new ExtensionPackageDefinition();

            extensionPackageDefinition.Name = "GHZ.GasStationSample";
            extensionPackageDefinition.Publisher = "GHZ";
            extensionPackageDefinition.IsEnabled = true;

            getExtensionsResponse.ExtensionPackageDefinitions.Add(extensionPackageDefinition);
        }

        public void OnExecuting(Request request)
        {
        }
    }
}
