using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Liquid.Serverless.AzureFunctions.Extensions
{
    /// <summary>
    /// Http Request Middleware Configurator Class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ChainConfigurator
    {
        /// <summary>
        /// Chains the specified middleware services.
        /// </summary>
        /// <typeparam name="TChain"></typeparam>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IChainConfigurator<TChain> Chain<TChain>(this IServiceCollection services) where TChain : class
        {
            return new ChainConfiguratorImpl<TChain>(services);
        }

        /// <summary>
        /// Chain Configurator Interface.
        /// </summary>
        /// <typeparam name="TChain"></typeparam>
        public interface IChainConfigurator<in TChain>
        {
            /// <summary>
            /// Adds this instance.
            /// </summary>
            /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
            /// <returns></returns>
            IChainConfigurator<TChain> Add<TImplementation>() where TImplementation : TChain;

            /// <summary>
            /// Configures this instance.
            /// </summary>
            void Configure();
        }

        /// <summary>
        /// Chain Configurator Implementor
        /// </summary>
        /// <typeparam name="TChain"></typeparam>
        /// <seealso cref="IChainConfigurator{T}" />
        private class ChainConfiguratorImpl<TChain> : IChainConfigurator<TChain> where TChain : class
        {
            private readonly IServiceCollection _services;
            private readonly List<Type> _types;
            private readonly Type _interfaceType;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChainConfiguratorImpl{TChain}"/> class.
            /// </summary>
            /// <param name="services">The services.</param>
            public ChainConfiguratorImpl(IServiceCollection services)
            {
                _services = services;
                _types = new List<Type>();
                _interfaceType = typeof(TChain);
            }

            /// <summary>
            /// Adds this instance.
            /// </summary>
            /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
            /// <returns></returns>
            /// <exception cref="ArgumentException">type</exception>
            public IChainConfigurator<TChain> Add<TImplementation>() where TImplementation : TChain
            {
                var type = typeof(TImplementation);

                if (!_interfaceType.IsAssignableFrom(type))
                    throw new ArgumentException($"{type.Name} type is not an implementation of {_interfaceType.Name}", nameof(type));

                _types.Add(type);

                return this;
            }

            /// <summary>
            /// Configures this instance.
            /// </summary>
            /// <exception cref="InvalidOperationException">No implementation defined for {_interfaceType.Name}</exception>
            public void Configure()
            {
                if (_types.Count == 0)
                    throw new InvalidOperationException($"No implementation defined for {_interfaceType.Name}");

                var first = true;
                foreach (var type in _types)
                {
                    ConfigureType(type, first);
                    first = false;
                }
            }

            /// <summary>
            /// Configures the type.
            /// </summary>
            /// <param name="currentType">Type of the current.</param>
            /// <param name="first">if set to <c>true</c> [first].</param>
            private void ConfigureType(Type currentType, bool first)
            {
                var nextType = _types.SkipWhile(type => type != currentType).SkipWhile(type => type == currentType).FirstOrDefault();

                var ctor = currentType.GetConstructors().OrderByDescending(constructorInfo => constructorInfo.GetParameters().Count()).First();

                var parameter = Expression.Parameter(typeof(IServiceProvider), "x");

                var ctorParameters = ctor.GetParameters().Select(parameterInfo =>
                {
                    if (!_interfaceType.IsAssignableFrom(parameterInfo.ParameterType))
                    {
                        return (Expression) Expression.Call(typeof(ServiceProviderServiceExtensions), "GetRequiredService", new[] {parameterInfo.ParameterType}, parameter);
                    }

                    if (nextType == null) return Expression.Constant(null, _interfaceType);


                    return Expression.Call(typeof(ServiceProviderServiceExtensions), "GetRequiredService", new[] { nextType }, parameter);
                });

                var body = Expression.New(ctor, ctorParameters.ToArray());

                var resolveType = first ? _interfaceType : currentType;

                var expressionType = Expression.GetFuncType(typeof(IServiceProvider), resolveType);
                var expression = Expression.Lambda(expressionType, body, parameter);
                var x = (Func<IServiceProvider, object>)expression.Compile();
                _services.AddScoped(resolveType, x);
            }
        }
    }
}