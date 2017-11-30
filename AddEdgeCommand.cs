using System;
using System.Management.Automation;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Reflection;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace psCosmosGraph
{
    [Cmdlet("Add", "Edge")]
    public class AddEdgeCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "SingleEdge")]
        public string From;

        [Parameter(Mandatory = true, ParameterSetName = "SingleEdge")]
        public string To;

        [Parameter(Mandatory = true, ParameterSetName = "MultipleEdges")]
        public PSObject[] Edges;

        private List<Task<FeedResponse<dynamic>>> tasks = new List<Task<FeedResponse<dynamic>>>();
        private List<string> gremlinQueries = new List<string>();

        protected override void BeginProcessing()
        {
            // what the fuck is going on here?
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_BindingRedirect;

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {

            switch (ParameterSetName)
            {
                case "SingleEdge": { WriteVerbose("Single Edge processing"); SingleEdge();  break;  };
                case "MultipleEdges": { WriteVerbose("Multiple Edge processing");  MultipleEdges(); break; };
            }

           

        }

        protected override void EndProcessing()
        {
            WriteVerbose("Waiting on tasks");
            Task.WaitAll(tasks.ToArray());
            base.EndProcessing();
        }


        private void SingleEdge()
        {
            string q = $"g.V('{From}').addE('linked').to(g.V('{To}'))";
            WriteVerbose($"adding {q}");

            var client = Config.Instance.Client;
            IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(Config.Instance.Graph, q);
            tasks.Add(query.ExecuteNextAsync());
        }

        private void MultipleEdges()
        {
            var client = Config.Instance.Client;

            WriteVerbose("creating tasks");
            foreach (PSObject e in Edges)
            {
                var props = e.Properties;
                    

                string q = $"g.V('{props["From"].Value}').addE('linked').to(g.V('{props["To"].Value}'))";
                WriteVerbose($"adding a task for a {q}");

                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(Config.Instance.Graph, q);
                tasks.Add(query.ExecuteNextAsync());
                }
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
