using eu.iamia.Util.Extensions;
using Xunit;

namespace eu.iamia.Util.UnitTest.Extensions
{
    public class CompareableExtensionShould
    {
        [Theory]
        [InlineData(false, -2f, -1f, 1f)]
        [InlineData(true, -1f, -1f, 1f)]
        [InlineData(true, 0f, -1f, 1f)]
        [InlineData(true, 1f, -1f, 1f)]
        [InlineData(false, 2f, -1f, 1f)]
        public void ReturnExpectedForIsWithinRange(bool expected, float value, float lower, float upper)
        {
            Assert.Equal(expected, value.IsWithinRange(lower, upper));
        }

        [Theory]
        [InlineData(true, -2f, -1f, 1f)]
        [InlineData(false, -1f, -1f, 1f)]
        [InlineData(false, 0f, -1f, 1f)]
        [InlineData(false, 1f, -1f, 1f)]
        [InlineData(true, 2f, -1f, 1f)]
        public void ReturnExpectedForIsOutsideRange(bool expected, float value, float lower, float upper)
        {
            Assert.Equal(expected, value.IsOutsideRange(lower, upper));
        }
    }
}
