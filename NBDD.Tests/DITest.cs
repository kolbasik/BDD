using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using NBDD.V2;
using Ploeh.AutoFixture;
using Xunit;

namespace NBDD.Tests
{
    public sealed class DITest
    {
        public class TestBase
        {
            public TestBase()
            {
                Fixture = new Fixture();
                Container = new DI(new CompositionContainer());
            }

            public Fixture Fixture { get; }
            public DI Container { get; }
        }

        public sealed class ScopeMethod : TestBase
        {
            [Fact]
            public void Should_create_a_new_container()
            {
                // arrage
                var expect = Container;

                // act
                var actual = Container.Scope();

                // assert
                Assert.NotEqual(expect, actual);
            }

            [Fact]
            public void Should_import_the_registration_from_parent_scope()
            {
                // arrage
                var expect = new object();
                Container.Composition.ComposeExportedValue(expect);

                // act
                var actual = Container.Scope().Composition.GetExportedValue<object>();

                // assert
                Assert.Same(expect, actual);
            }
        }

        public sealed class RegisterMethod : TestBase
        {
            [Fact]
            public void Should_register_the_instance_by_type()
            {
                // arrage
                var expect = Fixture.CreateMany<int>().ToList();

                // act
                var actual = Container.Register<IList<int>>(expect).Composition.GetExportedValue<IList<int>>();

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_register_the_instance_by_name()
            {
                // arrage
                var name = Fixture.Create<string>();
                var expect = Fixture.CreateMany<int>().ToList();

                // act
                var actual = Container.Register<IList<int>>(name, expect).Composition.GetExportedValue<IList<int>>(name);

                // assert
                Assert.Same(expect, actual);
            }
        }

        public sealed class ResolveMethod : TestBase
        {
            [Fact]
            public void Should_resolve_the_instance_by_type()
            {
                // arrage
                var expect = Fixture.CreateMany<int>().ToList();
                Container.Composition.ComposeExportedValue<IList<int>>(expect);

                // act
                var actual = Container.Resolve<IList<int>>();

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_resolve_the_instance_by_name()
            {
                // arrage
                var name = Fixture.Create<string>();
                var expect = Fixture.CreateMany<int>().ToList();
                Container.Composition.ComposeExportedValue<IList<int>>(name, expect);

                // act
                var actual = Container.Resolve<IList<int>>(name);

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_raise_an_exception_if_unset_the_registration_by_type()
            {
                // act & assert
                Assert.Throws<ImportCardinalityMismatchException>(() => Container.Resolve<IList<int>>());
            }

            [Fact]
            public void Should_raise_an_exception_if_unset_the_registration_by_name()
            {
                // arrage
                var name = Fixture.Create<string>();

                // act & assert
                Assert.Throws<ImportCardinalityMismatchException>(() => Container.Resolve<IList<int>>(name));
            }
        }

        public sealed class ResolveOrDeafultMethod : TestBase
        {
            [Fact]
            public void Should_return_the_default_value_if_unset_the_registration_by_type()
            {
                // arrage
                var expect = default(IList<int>);

                // act
                var actual = Container.ResolveOrDefault<IList<int>>();

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_return_the_default_value_if_unset_the_registration_by_name()
            {
                // arrage
                var name = Fixture.Create<string>();
                var expect = default(IList<int>);

                // act
                var actual = Container.ResolveOrDefault<IList<int>>(name);

                // assert
                Assert.Same(expect, actual);
            }
        }
    }
}