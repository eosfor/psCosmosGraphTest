using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using System.Management.Automation;

namespace psCosmosGraph
{
    [Cmdlet("Test", "Graph")]
    public class TestGraph : PSCmdlet
    {

        protected override void ProcessRecord()
        {
            string endpoint = "<myEndpoint>";
            string authKey = "<mysecret>";

            DocumentClient client = new DocumentClient(
                    new Uri(endpoint),
                    authKey);

            WriteObject(client);

            var database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = "graphdb01" });

            database.Wait();

            WriteObject(database);

            ResourceResponse<DocumentCollection> graph = client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("graphdb01", "app")).Result;

            IDocumentQuery<dynamic> query = client.CreateGremlinQuery<dynamic>(graph, "g.V().count()");

            WriteObject(query);

            //base.ProcessRecord();
        }
    }
}