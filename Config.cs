using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psCosmosGraph
{

    class Config
    {
        private static Config _Instance;
        private static ResourceResponse<DocumentCollection> _Graph;
        private static DocumentClient _Client;

        //other members... 
        private Config()
        {  // never used
            throw new Exception("WTF, who called this constructor?!?");
        }
        private Config(ResourceResponse<DocumentCollection> graph, DocumentClient client)
        {
            Config._Graph = graph;
            Config._Client = client;
        }
        public static Config Instance
        {
            get
            {
                if (_Instance == null)
                {
                    throw new Exception("Object not created");
                }
                return _Instance;
            }
        }
        public ResourceResponse<DocumentCollection> Graph
        {
            get
            {
                if (_Instance == null)
                {
                    throw new Exception("Object not created");
                }
                return _Graph;
            }
        }
        public DocumentClient Client
        {
            get
            {
                if (_Instance == null)
                {
                    throw new Exception("Object not created");
                }
                return _Client;
            }
        }

        public static void Create(ResourceResponse<DocumentCollection> graph, DocumentClient client)
        {
            if (_Instance != null)
            {
                throw new Exception("Object already created");
            }
            _Instance = new Config(graph, client);
        }
    }
}
