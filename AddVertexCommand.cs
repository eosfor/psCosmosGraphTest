using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace psCosmosGraph
{
    [Cmdlet("Add", "Vertex")]
    public class AddVertexCommand : PSCmdlet
    {

        [Parameter()]
        public string[] Vertex;


        private List<Task<FeedResponse<dynamic>>> tasks = new List<Task<FeedResponse<dynamic>>>();

        protected override void BeginProcessing()
        {
            // what the fuck is going on here?
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_BindingRedirect;

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            // what the fuck is going on here?
            // AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_BindingRedirect;

            var client = Config.Instance.Client;


            foreach (var v in Vertex)
            {
                string q = $"g.AddV('node').property('id', '{v}')";
                WriteVerbose(q);

                IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(Config.Instance.Graph, q);

                tasks.Add(query.ExecuteNextAsync());
            }


            //WriteObject(t.Result);

            //base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            WriteVerbose("Waiting on tasks");
            Task.WaitAll(tasks.ToArray());
            base.EndProcessing();
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