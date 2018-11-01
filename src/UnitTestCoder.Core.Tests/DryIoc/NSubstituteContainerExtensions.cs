using DryIoc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestCoder.Core.Tests.DryIoc
{
    public static class NSubstituteContainerExtensions
    {
        /// <summary>
        /// Configures the container to create any unregistered types through NSubstitute.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="reuse">Default is Reuse.ScopedOrSingleton</param>
        /// <returns></returns>
        public static IContainer WithNSubstituteFallback(this IContainer container, IReuse reuse = null)
        {
            // See: https://bitbucket.org/dadhi/dryioc/wiki/UsingInTestsWithMockingLibrary

            // Cache all the ReflectionFactory instances by Type.
            var dict = new ConcurrentDictionary<Type, ReflectionFactory>();

            return container.With(rules =>
            {
                return rules.WithUnknownServiceResolvers(request =>
                {
                    var serviceType = request.ServiceType;
                    if(!serviceType.IsAbstract)
                        return null; // Mock interface or abstract class only.

                    return dict.GetOrAdd(serviceType,
                        x => new ReflectionFactory(reuse: reuse ?? Reuse.ScopedOrSingleton,
                            made: Made.Of(
                                () => NSubstitute.Substitute.For(
                                    Arg.Index<Type[]>(0),
                                    Arg.Index<object[]>(1)
                                ),
                                _ => new[] { serviceType },     // arg0
                                _ => (object[])null             // arg1
                            )
                        )
                    );
                });
            });
        }
    }
}
