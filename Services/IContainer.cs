using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IContainer
    {
        void Register<TService, TImplementation>(Lifecycle lifecycle) where TImplementation : TService;
        void Register<TService>(Func<IContainer, TService> instanceCreator, Lifecycle lifecycle);
        void RegisterInstance<TService>(TService instance);
        TService Resolve<TService>();
    }

}
