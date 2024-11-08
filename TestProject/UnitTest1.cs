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

            // Résolution
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();

            // Vérifie que les instances sont identiques
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterAndResolveTransient_ShouldReturnDifferentInstances()
        {
            // Enregistrement en tant que Transient
            _container.Register<IService, ServiceImplementation>(Lifecycle.Transient);

            // Résolution
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();

            // Vérifie que les instances sont différentes
            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void RegisterAndResolveWithoutInterface_ShouldReturnInstance()
        {
            // Enregistrement de la classe sans interface
            _container.Register<ServiceImplementation, ServiceImplementation>(Lifecycle.Transient);

            // Résolution
            var instance = _container.Resolve<ServiceImplementation>();

            // Vérifie que l'instance n'est pas nulle et du bon type
            Assert.NotNull(instance);
            Assert.IsType<ServiceImplementation>(instance);
        }

        [Fact]
        public void RegisterAndResolveWithConstructorParameters_ShouldInjectDependencies()
        {
            // Enregistrement des services avec constructeur dépendant
            _container.Register<IService, ServiceImplementation>(Lifecycle.Transient);
            _container.Register<IClient, ClientImplementation>(Lifecycle.Transient);

            // Résolution de IClient, qui dépend de IService
            var client = _container.Resolve<IClient>();

            // Vérifie que le client est bien instancié avec la dépendance
            Assert.NotNull(client);
            Assert.IsType<ClientImplementation>(client);
            Assert.NotNull(((ClientImplementation)client).Service);
        }

        [Fact]
        public void RegisterInstance_ShouldReturnSameInstance()
        {
            var serviceInstance = new ServiceImplementation();

            // Enregistrement d'une instance spécifique
            _container.RegisterInstance<IService>(serviceInstance);

            // Résolution
            var resolvedInstance = _container.Resolve<IService>();

            // Vérifie que l'instance enregistrée est identique à celle résolue
            Assert.Same(serviceInstance, resolvedInstance);
        }

        [Fact]
        public void RegisterAndResolveScoped_ShouldReturnSameInstanceWithinScope()
        {
            // Enregistrement en tant que Scoped
            _container.Register<IService, ServiceImplementation>(Lifecycle.Scoped);

            // Simuler un scope (par exemple, un bloc de code ou une méthode)
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();

            // Vérifie que les instances dans le même scope sont identiques
            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void RegisterAndResolveScoped_ShouldReturnDifferentInstancesBetweenScopes()
        {
            // Enregistrement en tant que Scoped
            _container.Register<IService, ServiceImplementation>(Lifecycle.Scoped);

            // Démarrer le premier scope
            _container.BeginScope();
            var instance1 = _container.Resolve<IService>();
            _container.EndScope();

            // Démarrer un nouveau scope
            _container.BeginScope();
            var instance2 = _container.Resolve<IService>();
            _container.EndScope();

            // Vérifie que les instances entre les scopes sont différentes
            Assert.NotSame(instance1, instance2);
        }

    }
}