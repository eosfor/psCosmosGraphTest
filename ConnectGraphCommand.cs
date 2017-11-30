using System;
using System.Management.Automation;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Reflection;

namespace psCosmosGraph
{
    [Cmdlet("Connect", "Graph")]
    public class ConnectGraphCommand : PSCmdlet
    {

        [Parameter(Mandatory = true)]
        public string Endpoint;

        [Parameter(Mandatory = true)]
        public string Authkey;

        [Parameter(Mandatory = true)]
        public string DBName;

        [Parameter(Mandatory = true)]
        public string CollectionName;


        protected override void BeginProcessing()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_BindingRedirect;

            DocumentClient client = new DocumentClient(
                    new Uri(Endpoint),
                    Authkey);

            ResourceResponse<DocumentCollection> graph = client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DBName, CollectionName)).Result;

            Config.Create(graph, client);

            //base.BeginProcessing();
        }

        public static Assembly CurrentDomain_BindingRedirect(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            switch (name.Name)
            {
                case "Newtonsoft.Json":
                    return typeof(Newtonsoft.Json.JsonSerializer).Assembly;

                case "System.Collections.Immutable":
                    return Assembly.LoadFrom("System.Collections.Immutable.dll");

                default:
                    return null;
            }
        }

    }
}
