using Ninject;
using Ninject.Modules;

using ZuegerAdressbook.Model;
using ZuegerAdressbook.ViewModels;

namespace ZuegerAdressbook
{
    public static class IocKernel
    {
        private static StandardKernel _kernel;

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static PersonViewModel GetPersonViewModel(IChangeListener parent, Person person = null)
        {
            var personParameterConfig = new Ninject.Parameters.ConstructorArgument("person", person);
            var parentParameterConfig = new Ninject.Parameters.ConstructorArgument("parent", parent);
            return _kernel.Get<PersonViewModel>(personParameterConfig, parentParameterConfig);
        }

        public static DocumentViewModel GetDocumentViewModel(IChangeListener parent, Document document = null)
        {
            var documentParameterConfig = new Ninject.Parameters.ConstructorArgument("document", document);
            var parentParameterConfig = new Ninject.Parameters.ConstructorArgument("parent", parent);
            return _kernel.Get<DocumentViewModel>(documentParameterConfig, parentParameterConfig);
        }

        public static void Initialize(params INinjectModule[] modules)
        {
            if (_kernel == null)
            {
                _kernel = new StandardKernel(modules);
            }
        }
    }
}
