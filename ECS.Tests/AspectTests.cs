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
    public class AspectTests : IClassFixture<ComponentTypesToBigIntegerMapper>
    {
        private readonly ComponentTypesToBigIntegerMapper mapper;
        private readonly BigInteger none;
        private readonly BigInteger c1;
        private readonly BigInteger c2;
        private readonly BigInteger c1c2;

        public AspectTests(ComponentTypesToBigIntegerMapper mapper)
        {
            this.mapper = mapper;
            none = BigInteger.Zero;
            c1 = mapper.TypesToBigInteger(typeof (TestComponentOne));
            c2 = mapper.TypesToBigInteger(typeof (TestComponentTwo));
            c1c2 = mapper.TypesToBigInteger(typeof (TestComponentOne), typeof (TestComponentTwo));
        }

        [Fact()]
        public void EmptyTest()
        {
            var asp1 = Aspect.Empty();

            Assert.True(asp1.Interested(none));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AllTestOne()
        {
            var asp1 = Aspect.All(typeof (TestComponentOne));
            Assert.False(asp1.Interested(none));
            Assert.False(asp1.Interested(c2));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AllTestTwo()
        {
            var asp1 = Aspect.All(typeof (TestComponentOne), typeof (TestComponentTwo));
            Assert.False(asp1.Interested(none));
            Assert.False(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AnyTestOne()
        {
            var asp1 = Aspect.Any(typeof (TestComponentOne));
            Assert.False(asp1.Interested(none));
            Assert.False(asp1.Interested(c2));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void AnyTestTwo()
        {
            var asp1 = Aspect.Any(typeof (TestComponentOne), typeof (TestComponentTwo));
            Assert.False(asp1.Interested(none));
            Assert.True(asp1.Interested(c2));
            Assert.True(asp1.Interested(c1));
            Assert.True(asp1.Interested(c1c2));
        }

        [Fact()]
        public void MapperTest()
        {
            var mapper2 = new ComponentTypesToBigIntegerMapper();
            Assert.NotEqual(mapper, mapper2);
            Assert.True(mapper != mapper2);
            Assert.False(mapper == mapper2);
        }
    }
}
