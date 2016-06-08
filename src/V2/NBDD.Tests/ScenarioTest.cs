using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Ploeh.AutoFixture;
using Xunit;

namespace NBDD.V2.Tests
{
    public sealed class ScenarioTest
    {
        public class TestBase
        {
            public TestBase()
            {
                Fixture = new Fixture();
                Fixture.Customize<Feature>(ctx => ctx.OmitAutoProperties());
                Fixture.Customize<Scenario>(ctx => ctx.OmitAutoProperties());
                Scenario = Fixture.Create<Scenario>();
            }

            public Fixture Fixture { get; }
            public Scenario Scenario { get; }
        }

        public sealed class BindMethod : TestBase
        {
            [Fact]
            public void Should_return_scenario()
            {
                // arrange
                var expect = Scenario;

                // act
                var actual = Scenario.Bind(() => Task.CompletedTask);

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_register_the_action_in_the_scenario_units()
            {
                // arrange
                var expect = new Func<Task>(() => Task.CompletedTask);

                // act
                var unit = Scenario.Bind(expect).Units.Single();
                var actual = Assert.IsType<Bind>(unit);

                // assert
                Assert.Same(expect, actual.Action);
            }
        }

        public sealed class StepMethod : TestBase
        {
            [Fact]
            public void Should_return_scenario()
            {
                // arrange
                var expect = Scenario;

                // act
                var actual = Scenario.Step(Fixture.Create<string>(), () => Task.CompletedTask);

                // assert
                Assert.Same(expect, actual);
            }

            [Fact]
            public void Should_register_the_action_in_the_scenario_units()
            {
                // arrange
                var title = Fixture.Create<string>();
                var expect = new Func<Task>(() => Task.CompletedTask);

                // act
                var unit = Scenario.Step(title, expect).Units.Single();
                var actual = Assert.IsType<Step>(unit);

                // assert
                Assert.Same(expect, actual.Action);
                Assert.Same(title, actual.Title);
            }
        }

        public sealed class DisposeMethod : TestBase
        {
            [Fact]
            public void Should_not_raise_any_exceptions()
            {
                // act
                Scenario.Dispose();
            }

            [Fact]
            public void Should_dispose_the_MEF_container()
            {
                // act
                Scenario.Dispose();

                // assert
                Assert.Throws<ObjectDisposedException>(() => Scenario.Container.Composition.Providers);
            }

            [Fact]
            public void Should_dispose_all_components()
            {
                // arrange
                var component = A.Fake<Component>();
                Scenario.Components.Add(component);

                // act
                Scenario.Dispose();

                // assert
                A.CallTo(() => component.Dispose()).MustHaveHappened();
            }
        }
    }
}