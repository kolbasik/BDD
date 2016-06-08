using System;
using System.Linq;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Xunit;

namespace NBDD.V2.Tests
{
    public sealed class RunnerTest
    {
        public class TestBase
        {
            public TestBase()
            {
                Fixture = new Fixture();
                Feature = Runner.Feature();
            }

            public Fixture Fixture { get; }
            public Feature Feature { get; }
        }

        public sealed class TestAsyncMethod : TestBase
        {
            [Fact]
            public async Task Should_restore_an_exception()
            {
                // arrage
                var expect = Fixture.Create<NotSupportedException>();

                Feature.Describe(a =>
                {
                    a.Scenario()
                        .Step(expect.Message, () => { throw expect; });
                });

                // act
                var actual = await Assert.ThrowsAsync<NotSupportedException>(Feature.TestAsync);

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public async Task Should_combine_the_exceptions()
            {
                // arrage
                var expect = Fixture.CreateMany<NotSupportedException>(2).ToArray();

                Feature.Describe(a =>
                {
                    a.Scenario().Step(Fixture.Create<string>(), () => { throw expect[0]; });
                    a.Scenario().Step(Fixture.Create<string>(), () => { throw expect[1]; });
                });

                // act
                var exception = await Assert.ThrowsAsync<AggregateException>(Feature.TestAsync);
                var actual = exception.InnerExceptions;

                // assert
                Assert.Equal(expect, actual);
            }
        }

        public sealed class PlayAsyncMethod : TestBase
        {
            [Fact]
            public async Task Should_dispose_the_feature()
            {
                // act
                await Feature.PlayAsync();

                // assert
                Assert.Throws<ObjectDisposedException>(() => Feature.Container.Composition.Providers);
            }

            [Fact]
            public async Task Should_dispose_all_scenarios()
            {
                // arrange
                var scenario1 = Feature.Scenario();
                var scenario2 = Feature.Scenario();

                // act
                await Feature.PlayAsync();

                // assert
                Assert.Throws<ObjectDisposedException>(() => scenario1.Container.Composition.Providers);
                Assert.Throws<ObjectDisposedException>(() => scenario2.Container.Composition.Providers);
            }

            [Fact]
            public async Task Should_return_the_feature_result()
            {
                // arrange
                var feature = Feature;

                // act
                var actual = await feature.PlayAsync();

                // assert
                Assert.NotNull(actual);
                Assert.Same(feature, actual.Feature);
            }

            [Fact]
            public async Task Should_return_the_scenario_result()
            {
                // arrange
                var scenario = Feature.Scenario();

                // act
                var actual = await Feature.PlayAsync();

                // assert
                Assert.NotNull(actual);
                Assert.Contains(scenario, actual.Scenarios.Select(x => x.Scenario));
            }

            [Fact]
            public async Task Should_return_the_step_result()
            {
                // arrange
                var name = Fixture.Create<string>();
                Feature.Describe(a => a.Scenario().Step(name, () => Task.CompletedTask));

                // act
                var actual = await Feature.PlayAsync();

                // assert
                Assert.NotNull(actual);
                Assert.Contains(name, actual.Scenarios.SelectMany(scenario => scenario.Steps.Select(step => step.Name)));
            }
        }
    }
}