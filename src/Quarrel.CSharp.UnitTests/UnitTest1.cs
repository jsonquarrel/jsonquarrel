using System;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Quarrel.CSharp.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestOfElements()
        {
            using var d1 = JsonDocument.Parse("true");
            using var d2 = JsonDocument.Parse("false");
            var e1 = d1.RootElement;
            var e2 = d2.RootElement;
            var diffs = JsonDiff.OfElements(e1, e2).ToList();
            Assert.NotEmpty(diffs);
            var kindDiff = (Diff.Kind) diffs.Single();
            Assert.Equal("$", kindDiff.Item.Path);
            Assert.Equal(true, kindDiff.Item.Left.GetBoolean());
            Assert.Equal(false, kindDiff.Item.Right.GetBoolean());
        }

        [Fact]
        public void TestOfDocuments()
        {
            using var d1 = JsonDocument.Parse("true");
            using var d2 = JsonDocument.Parse("false");
            var diffs = JsonDiff.OfDocuments(d1, d2).ToList();
            Assert.NotEmpty(diffs);
            var kindDiff = (Diff.Kind) diffs.Single();
            Assert.Equal("$", kindDiff.Item.Path);
            Assert.Equal(true, kindDiff.Item.Left.GetBoolean());
            Assert.Equal(false, kindDiff.Item.Right.GetBoolean());
        }

        [Fact]
        public void TestOfStrings()
        {
            var diffs = JsonDiff.OfStrings("{}", "{}");
            Assert.Empty(diffs);
        }
        
        [Fact]
        public void TestOfStrings2()
        {
            var diffs = JsonDiff.OfStrings("1", "2").ToList();
            Assert.NotEmpty(diffs);
        }

    }
}
