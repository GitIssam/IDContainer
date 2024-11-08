using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class ContainerTests
    {
        private readonly Container _container = new Container();

        [Fact]
        public void RegisterAndResolveSingleton_ShouldReturnSameInstance()
        {
            _container.Register<IService, ServiceImplementation>(Lifecycle.Singleton);
            var instance1 = _container.Resolve<IService>();
            var instance2 = _container.Resolve<IService>();
            Assert.Same(instance1, instance2);
        }
        public interface IService { }
        public class ServiceImplementation : IService { }

        public interface IClient
        {
            IService Service { get; }
        }

        public class ClientImplementation : IClient
        {
            public IService Service { get; }
            public ClientImplementation(IService service)
            {
                Service = service;
            }
        }
    }
}
