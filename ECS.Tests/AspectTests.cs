using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    public class AspectTests
    {
        [Fact()]
        public void EmptyTest()
        {
            var asp1 = Aspect.Empty();
            Assert.True(asp1.Interested(new List<Type>()));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne), typeof(TestComponentTwo) }));
        }

        [Fact()]
        public void AllTestOne()
        {
            var asp1 = Aspect.All(typeof(TestComponentOne));
            Assert.False(asp1.Interested(new List<Type>()));
            Assert.False(asp1.Interested(new List<Type>() { typeof(TestComponentTwo) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne), typeof(TestComponentTwo) }));
        }

        [Fact()]
        public void AllTestTwo()
        {
            var asp1 = Aspect.All(typeof(TestComponentOne), typeof(TestComponentTwo));
            Assert.False(asp1.Interested(new List<Type>()));
            Assert.False(asp1.Interested(new List<Type>() { typeof(TestComponentOne) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne), typeof(TestComponentTwo) }));
        }

        [Fact()]
        public void AnyTestOne()
        {
            var asp1 = Aspect.Any(typeof(TestComponentOne));
            Assert.False(asp1.Interested(new List<Type>()));
            Assert.False(asp1.Interested(new List<Type>() { typeof(TestComponentTwo) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne), typeof(TestComponentTwo) }));
        }

        [Fact()]
        public void AnyTestTwo()
        {
            var asp1 = Aspect.Any(typeof(TestComponentOne), typeof(TestComponentTwo));
            Assert.False(asp1.Interested(new List<Type>()));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentTwo) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne) }));
            Assert.True(asp1.Interested(new List<Type>() { typeof(TestComponentOne), typeof(TestComponentTwo) }));
        }
    }
}
