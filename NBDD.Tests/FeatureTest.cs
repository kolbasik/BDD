using System;
using NBDD.V2;
using Ploeh.AutoFixture;
using Xunit;

namespace NBDD.Tests
{
    public sealed class FeatureTest
    {
        public class TestBase
        {
            public TestBase()
            {
                Fixture = new Fixture();
                Fixture.Customize<Feature>(ctx => ctx.OmitAutoProperties()
                    .With(x => x.AsA)
                    .With(x => x.IWant)
                    .With(x => x.SoThat));

                Feature = Fixture.Create<Feature>();
            }

            public Fixture Fixture { get; }
            public Feature Feature { get; }
        }

        public sealed class ScenarioMethod : TestBase
        {
            [Fact]
            public void Should_create_and_return_scenarios()
            {
                // act
                var actual1 = Feature.Scenario();
                var actual2 = Feature.Scenario();

                // assert
                Assert.NotEqual(actual1, actual2);
            }

            [Fact]
            public void Should_remember_scenarios()
            {
                // act
                var actual = Feature.Scenario();

                // assert
                Assert.NotNull(actual);
                Assert.Contains(actual, Feature.Scenarios);
            }
        }

        public sealed class DescribeMethod : TestBase
        {
            [Fact]
            public void Should_return_the_feature()
            {
                // arrange
                var expect = Feature;

                // act
                var actual = Feature.Describe(a => { });

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_describe_the_feature()
            {
                // arrange
                Feature expect = Feature;
                Feature actual = null;

                // act
                Feature.Describe(a => actual = a);

                // assert
                Assert.NotNull(actual);
                Assert.Same(expect, actual);
            }
        }

        public sealed class DisposeMethod : TestBase
        {
            [Fact]
            public void Should_not_raise_any_exceptions()
            {
                // act
                Feature.Dispose();
            }

            [Fact]
            public void Should_dispose_the_MEF_container()
            {
                // act
                Feature.Dispose();

                // assert
                Assert.Throws<ObjectDisposedException>(() => Feature.Container.Providers);
            }

            [Fact]
            public void Should_dispose_all_scenarios()
            {
                // arrange
                var scenario1 = Feature.Scenario();
                var scenario2 = Feature.Scenario();

                // act
                Feature.Dispose();

                // assert
                Assert.Throws<ObjectDisposedException>(() => scenario1.Container.Providers);
                Assert.Throws<ObjectDisposedException>(() => scenario2.Container.Providers);
            }
        }
    }
}
