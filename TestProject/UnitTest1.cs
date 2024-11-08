using Services;
using static TestProject.ContainerTests;

namespace TestProject
{
    public class UnitTest1
    {
        private readonly Container _container = new Container();

        [Fact]
        public void RegisterAndResolveSingleton_ShouldReturnSameInstance()
        {
            // Enregistrement en tant que Singleton
            _container.Register<IService, ServiceImplementation>(Lifecycle.Singleton);

            // R�solution
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();

            // V�rifie que les instances sont identiques
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterAndResolveTransient_ShouldReturnDifferentInstances()
        {
            // Enregistrement en tant que Transient
            _container.Register<IService, ServiceImplementation>(Lifecycle.Transient);

            // R�solution
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();

            // V�rifie que les instances sont diff�rentes
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterAndResolveWithoutInterface_ShouldReturnInstance()
        {
            // Enregistrement de la classe sans interface
            _container.Register<ServiceImplementation, ServiceImplementation>(Lifecycle.Transient);

            // R�solution
            var instance = _container.Resolve<ServiceImplementation>();

            // V�rifie que l'instance n'est pas nulle et du bon type
            Assert.NotNull(instance);
            Assert.IsType<ServiceImplementation>(instance);
        }

        [Fact]
        public void RegisterAndResolveWithConstructorParameters_ShouldInjectDependencies()
        {
            // Enregistrement des services avec constructeur d�pendant
            _container.Register<IService, ServiceImplementation>(Lifecycle.Transient);
            _container.Register<IClient, ClientImplementation>(Lifecycle.Transient);

            // R�solution de IClient, qui d�pend de IService
            var client = _container.Resolve<IClient>();

            // V�rifie que le client est bien instanci� avec la d�pendance
            Assert.NotNull(client);
            Assert.IsType<ClientImplementation>(client);
            Assert.NotNull(((ClientImplementation)client).Service);
        }

        [Fact]
        public void RegisterInstance_ShouldReturnSameInstance()
        {
            var serviceInstance = new ServiceImplementation();

            // Enregistrement d'une instance sp�cifique
            _container.RegisterInstance<IService>(serviceInstance);

            // R�solution
            var resolvedInstance = _container.Resolve<IService>();

            // V�rifie que l'instance enregistr�e est identique � celle r�solue
            Assert.Same(serviceInstance, resolvedInstance);
        }

        [Fact]
        public void RegisterAndResolveScoped_ShouldReturnSameInstanceWithinScope()
        {
            // Enregistrement en tant que Scoped
            _container.Register<IService, ServiceImplementation>(Lifecycle.Scoped);

            // Simuler un scope (par exemple, un bloc de code ou une m�thode)
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();

            // V�rifie que les instances dans le m�me scope sont identiques
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterAndResolveScoped_ShouldReturnDifferentInstancesBetweenScopes()
        {
            // Enregistrement en tant que Scoped
            _container.Register<IService, ServiceImplementation>(Lifecycle.Scoped);

            // D�marrer le premier scope
            _container.BeginScope();
            var instance1 = _container.Resolve<IService>();
            _container.EndScope();

            // D�marrer un nouveau scope
            _container.BeginScope();
            var instance2 = _container.Resolve<IService>();
            _container.EndScope();

            // V�rifie que les instances entre les scopes sont diff�rentes
            Assert.NotSame(instance1, instance2);
        }

    }
}