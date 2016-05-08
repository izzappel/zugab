using Raven.Client;

namespace ZuegerAdressbook.Service
{
    public interface IDocumentStoreFactory
    {
        IDocumentStore CreateDocumentStore();
    }
}
