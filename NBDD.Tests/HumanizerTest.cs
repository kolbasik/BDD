using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;

namespace NBDD.V2.Tests
{
    public sealed class HumanizerTest
    {
        public sealed class HumanizeMethod
        {
            [Theory]
            [InlineData("", null)]
            [InlineData("", "")]
            [InlineData("custom test parameter", "customTestParameter")]
            [InlineData("i have run the test", "IHaveRunTheTest")]
            [InlineData("i have run the test", "I_Have_Run_The_Test")]
            [InlineData("i have run the test", "I__have__Run__the__Test")]
            public void Should_humanize_the_text(string expect, string title)
            {
                // act
                var actual = Humanizer.Humanize(title);

                // assert
                Assert.Equal(expect, actual);
            }
        }

        public sealed class PrefixMethod
        {
            [Theory]
            [InlineData("", null, null)]
            [InlineData("", "", "")]
            [InlineData("Given the number", "Given ", "the number")]
            [InlineData("Given the number", "Given ", "Given the number")]
            [InlineData("Given the number", "Given ", "given the number")]
            public void Should_prepend_the_prefix_if_not_exist(string expect, string prefix, string title)
            {
                // act
                var actual = Humanizer.Prefix(prefix, title);

                // assert
                Assert.Equal(expect, actual);
            }
        }

        public sealed class ToStringMethod
        {
            [Fact]
            public void Should_humanize_the_method_name_be_default()
            {
                // arrange
                var expect = @"it should take the method name";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_take_the_method_name());

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public void Should_return_the_description_text_if_defined()
            {
                // arrange
                var expect = @"~ It should take the description text ~";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_take_the_description_text());

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public void Should_humanize_the_method_name_be_default_async()
            {
                // arrange
                var expect = @"it should take the method name async";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_take_the_method_name_async());

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public void Should_return_the_description_text_if_defined_async()
            {
                // arrange
                var expect = @"~ It should take the description text ~";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_take_the_description_text_async());

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public void Should_include_the_list_of_parameters_binded_using_Prop_method()
            {
                // arrange
                var expect = @"it should take the parameters (arg 1: '$ARG1', arg 2: '$ARG2')";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_take_the_parameters(s.Prop<int>(@"ARG1"), s.Prop<object>(@"ARG2")));

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public void Should_include_the_list_of_parameters_binded_using_Props()
            {
                // arrange
                var expect = @"it should take the parameters (arg 1: '$ARG1', arg 2: '$ARG2')";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_take_the_parameters((int) s.Props[@"ARG1"], s.Props[@"ARG2"]));

                // assert
                Assert.Equal(expect, actual);
            }

            [Fact]
            public void It_should_redefine_the_parameters_according_to_bindings()
            {
                // arrange
                var expect = @"it should be defined as arg1=$T1 and arg2=$T2";

                // act
                var actual = Humanizer.ToString<Actions>((x, s) => x.It_should_redefine_the_parameters_according_to_bindings(s.Props[@"T1"], s.Props[@"T2"]));

                // assert
                Assert.Equal(expect, actual);
            }
        }

        private class Actions
        {
            public void It_should_take_the_method_name()
            {
            }

            [Description(@"~ It should take the description text ~")]
            public void It_should_take_the_description_text()
            {
            }

            public Task It_should_take_the_method_name_async()
            {
                return Task.CompletedTask;
            }

            [Description(@"~ It should take the description text ~")]
            public Task It_should_take_the_description_text_async()
            {
                return Task.CompletedTask;
            }

            public void It_should_take_the_parameters(int arg1, object arg2)
            {
            }

            [Description(@"it should be defined as arg1=$arg1 and arg2=$arg2")]
            public void It_should_redefine_the_parameters_according_to_bindings(object arg1, object arg2)
            {
            }
        }
    }
}