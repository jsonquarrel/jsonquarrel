using System.Linq;
using Xunit;

namespace Quarrel.CSharp.UnitTests
{
    public class JsonStringsTests
    {
        [Fact]
        public void TestDiffTrueVsFalse()
        {
            var diffMessages = JsonStrings.Diff("true", "false");
            var diffMessage = diffMessages.Single();
            Assert.Equal("Boolean value mismatch at $.\nExpected false but was true.", diffMessage);
        }

        [Fact]
        public void TestDiffFalseVsTrue()
        {
            var diffMessages = JsonStrings.Diff("false", "true");
            var diffMessage = diffMessages.Single();
            Assert.Equal("Boolean value mismatch at $.\nExpected true but was false.", diffMessage);
        }
        
        [Fact]
        public void TestDiffNullVsFalse()
        {
            var diffMessages = JsonStrings.Diff("null", "false");
            var diffMessage = diffMessages.Single();
            Assert.Equal("Value mismatch at $.\nExpected the boolean false but was null.", diffMessage);
        }
        
        [Fact]
        public void TestDiffFalseVsNull()
        {
            var diffMessages = JsonStrings.Diff("false", "null");
            var diffMessage = diffMessages.Single();
            Assert.Equal("Value mismatch at $.\nExpected null but was the boolean false.", diffMessage);
        }
        
        [Fact]
        public void TestDiffFalseVsZero()
        {
            var diffMessages = JsonStrings.Diff("false", "0");
            var diffMessage = diffMessages.Single();
            Assert.Equal("Value mismatch at $.\nExpected the number 0 but was the boolean false.", diffMessage);
        }

        [Fact]
        public void TestDiffZeroVsOne()
        {
            var diffMessages = JsonStrings.Diff("1", "true");
            var diffMessage = diffMessages.Single();
            Assert.Equal("Value mismatch at $.\nExpected the boolean true but was the number 1.", diffMessage);
        }
    }
}