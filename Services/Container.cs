using System.Collections.Generic;

namespace Services
{
    public class Container : IContainer
    {
        private readonly Dictionary<Type, (Func<IContainer, object> factory, Lifecycle lifecycle)> _registrations = new();
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly Stack<Dictionary<Type, object>> _scopedInstancesStack = new();

        public void Register<TService, TImplementation>(Lifecycle lifecycle) where TImplementation : TService
        {
            Register(typeof(TService), container => ResolveWithParameters(typeof(TImplementation)), lifecycle);
        }

        public void Register<TService>(Func<IContainer, TService> instanceCreator, Lifecycle lifecycle)
        {
            Register(typeof(TService), c => instanceCreator(c), lifecycle);
        }

        public void RegisterInstance<TService>(TService instance)
        {
            _registrations[typeof(TService)] = (container => instance, Lifecycle.Singleton);
        }

        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        private object Resolve(Type type)
        {
            if (_registrations.TryGetValue(type, out var registration))
            {
                switch (registration.lifecycle)
                {
                    case Lifecycle.Singleton:
                        if (!_singletonInstances.ContainsKey(type))
                        {
                            _singletonInstances[type] = registration.factory(this);
                        }
                        return _singletonInstances[type];

                    case Lifecycle.Scoped:
                        // Gérer les instances Scoped avec la pile
                        if (_scopedInstancesStack.Count == 0)
                        {
                            _scopedInstancesStack.Push(new Dictionary<Type, object>());
                        }

                        var currentScope = _scopedInstancesStack.Peek();
                        if (!currentScope.ContainsKey(type))
                        {
                            currentScope[type] = registration.factory(this);
                        }
                        return currentScope[type];

                    case Lifecycle.Transient:
                        return registration.factory(this);
                }
            }

            throw new InvalidOperationException($"Service of type {type} is not registered.");
        }

        public void BeginScope()
        {
            // Démarrer un nouveau scope
            _scopedInstancesStack.Push(new Dictionary<Type, object>());
        }

        public void EndScope()
        {
            // Terminer le scope actuel et supprimer les instances
            if (_scopedInstancesStack.Count > 0)
            {
                _scopedInstancesStack.Pop();
            }
        }
        private void Register(Type type, Func<IContainer, object> factory, Lifecycle lifecycle)
        {
            _registrations[type] = (factory, lifecycle);
        }

        private object ResolveWithParameters(Type type)
        {
            var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            if (constructor == null) throw new InvalidOperationException($"No public constructor found for {type}");

            var parameters = constructor.GetParameters()
                                        .Select(p => Resolve(p.ParameterType))
                                        .ToArray();
            return constructor.Invoke(parameters);
        }
    }
}
