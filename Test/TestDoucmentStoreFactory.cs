using Raven.Client;
using Raven.Client.Embedded;

using ZuegerAdressbook.Service;

namespace Test
{
    public class TestDoucmentStoreFactory : IDocumentStoreFactory
    {
        private EmbeddableDocumentStore _documentStore;

        public IDocumentStore CreateDocumentStore()
        {
            if (_documentStore == null)
            {
                _documentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };

                _documentStore.Configuration.Storage.Voron.AllowOn32Bits = true;

                _documentStore.Initialize();
            }

            return _documentStore;
        }
    }
}
