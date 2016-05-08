using System;
using System.IO;

using Raven.Client;
using Raven.Client.Embedded;

namespace ZuegerAdressbook.Service
{
    public class DocumentStoreFactory : IDocumentStoreFactory
    {
        private static IDocumentStore _documentStore;

        public IDocumentStore CreateDocumentStore()
        {
            InitializeDocumentStore();
            return _documentStore;
        }

        private static void InitializeDocumentStore()
        {
            var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "zugab");

            if (_documentStore == null)
            {
                _documentStore = new EmbeddableDocumentStore
                {
                    DataDirectory = Path.Combine(appDataFolder, "data")
                };

                _documentStore.Initialize();
            }
        }
    }
}