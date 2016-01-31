using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    [Collection("ECS")]
    public class AspectTests
    {
        private readonly BigInteger none;
        private readonly BigInteger c1;
        private readonly BigInteger c2;
        private readonly BigInteger c1c2;

        public AspectTests()
        {
            none = BigInteger.Zero;
            c1 = AspectMapper.TypesToBigInteger(typeof (TestComponentOne));
            c2 = AspectMapper.TypesToBigInteger(typeof (TestComponentTwo));
            c1c2 = AspectMapper.TypesToBigInteger(typeof (TestComponentOne), typeof (TestComponentTwo));
        }

        [Fact()]
        public void EmptyTest()
        {
            var asp1 = Aspect.Empty().Cache;
            //asp1.Cache(mapper)

            Assert.True(asp1.Interested(none));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AllTestOne()
        {
            var asp1 = Aspect.All(typeof (TestComponentOne)).Cache;
            Assert.False(asp1.Interested(none));
            Assert.False(asp1.Interested(c2));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AllTestTwo()
        {
            var asp1 = Aspect.All(typeof (TestComponentOne), typeof (TestComponentTwo)).Cache;
            Assert.False(asp1.Interested(none));
            Assert.False(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AnyTestOne()
        {
            var asp1 = Aspect.Any(typeof (TestComponentOne)).Cache;
            Assert.False(asp1.Interested(none));
            Assert.False(asp1.Interested(c2));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AnyTestTwo()
        {
            var asp1 = Aspect.Any(typeof (TestComponentOne), typeof (TestComponentTwo)).Cache;
            Assert.False(asp1.Interested(none));
            Assert.True(asp1.Interested(c2));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }
    }
}
