using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace UnitTestCoder.Core.Tests.DryIoc
{
    [TestClass]
    public class NSubstituteContainerExtensionsTests
    {
        [TestMethod]
        public void NSubstituteContainerExtensionsWithNSubstituteFallbackReuseScoped()
        {
            var container = new Container().WithNSubstituteFallback();

            // Confirm that resolving IFooUser / IFoo give us the same object.
            container.Register<IFooUser, FooUser>();

            using(var scope = container.OpenScope())
            {
                var fooUser = scope.Resolve<IFooUser>();
                fooUser.Foo.Value = 123;

                var foo = scope.Resolve<IFoo>();

                foo.Value.ShouldBe(123);
            }
        }
        [TestMethod]
        public void NSubstituteContainerExtensionsWithNSubstituteFallbackReuseSingleton()
        {
            var container = new Container().WithNSubstituteFallback();

            // Confirm that resolving IFooUser / IFoo give us the same object.
            container.Register<IFooUser, FooUser>();

            var fooUser = container.Resolve<IFooUser>();
            fooUser.Foo.Value = 123;

            var foo = container.Resolve<IFoo>();

            foo.Value.ShouldBe(123);
        }

        [TestMethod]
        public void NSubstituteContainerExtensionsWithNSubstituteSeparation()
        {
            var container = new Container().WithNSubstituteFallback(Reuse.Scoped);

            container.Register<IFooUser, FooUser>();

            var scope1 = container.OpenScope();
            var scope2 = container.OpenScope();

            var fooUser1 = scope1.Resolve<IFooUser>();
            fooUser1.Foo.Value = 123;

            var fooUser2 = scope2.Resolve<IFooUser>();
            fooUser2.Foo.Value = 456;

            fooUser1.Foo.ShouldNotBe(fooUser2.Foo);

            fooUser1.Foo.Value.ShouldBe(123);
            fooUser2.Foo.Value.ShouldBe(456);

            scope2.Dispose();
            scope1.Dispose();
        }

        public interface IFoo
        {
            int Value { get; set; }
        }

        public class FooUser : IFooUser
        {
            public FooUser(IFoo foo)
            {
                this.Foo = foo;
            }

            public IFoo Foo { get; }
        }

        public interface IFooUser
        {
            IFoo Foo { get; }
        }
    }
}
