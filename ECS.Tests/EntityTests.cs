using System.Linq;
using Xunit;

namespace ECS.Tests
{
    [Collection("ECS")]
    public class EntityTests
    {
        readonly EntityComponentSystem ecs;

        public EntityTests()
        {
            ecs = EntityComponentSystem.Instance;
        }

        [Fact]
        public void RemoveTest()
        {
            var ent = ecs.CreateEntity();
            ecs.Update(1f);
            Assert.True(ecs.FindEntities(Aspect.Any()).Contains(ent));
            ent.Remove();
            ecs.Update(1f);
            Assert.False(ecs.FindEntities(Aspect.Any()).Contains(ent));
        }

        [Fact]
        public void AddComponentTest()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();
            ent.AddComponent(tc);
            ecs.Update(1f);
            Assert.Equal(ent, ecs.FindEntities(Aspect.All(typeof(TestComponentOne))).FirstOrDefault());
            Assert.Equal(null, ecs.FindEntities(Aspect.All(typeof(TestComponentTwo))).FirstOrDefault());

            ent.Remove();
            ecs.Update(1f);
        }

        [Fact]
        public void RemoveComponentByTypeTest()
        {
            var ent = ecs.CreateEntity();
            ent.AddComponent(new TestComponentOne());
            ent.RemoveComponent<TestComponentOne>();
            ecs.Update(1f);

            Assert.Equal(null, ecs.FindEntities(Aspect.All(typeof(TestComponentTwo))).FirstOrDefault());

            ent.Remove();
            ecs.Update(1f);
        }

        [Fact]
        public void RemoveComponentByInstanceTest()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();
            ent.AddComponent(tc);
            ent.RemoveComponent(tc);
            ecs.Update(1f);

            Assert.Equal(null, ecs.FindEntities(Aspect.All(typeof(TestComponentOne))).FirstOrDefault());

            ent.Remove();
            ecs.Update(1f);
        }

        [Fact]
        public void HasComponentSmall()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();

            Assert.False(ent.HasComponent<TestComponentOne>());

            ent.AddComponent(tc);
            ecs.FlushChanges();

            Assert.True(ent.HasComponent<TestComponentOne>());

            ent.RemoveComponent(tc);
            ecs.FlushChanges();

            Assert.False(ent.HasComponent<TestComponentOne>());

            ecs.Update(1f);
            ent.Remove();
            ecs.Update(1f);
        }

        [Fact]
        public void GetComponentTest()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();

            ecs.Update(1f);

            Assert.Equal(null, ent.GetComponent<TestComponentOne>());

            ent.AddComponent(tc);

            ecs.FlushChanges();

            Assert.Equal(tc, ent.GetComponent<TestComponentOne>());

            ent.Remove();
            ecs.Update(1f);
        }

        [Fact]
        public void GetComponents()
        {
            var ent = ecs.CreateEntity();
            var tc1 = new TestComponentOne();
            var tc2 = new TestComponentTwo();

            ecs.Update(1f);
            Assert.Empty(ent.Components);

            ent.AddComponent(tc1);
            ecs.Update(1f);
            Assert.Equal(tc1, ent.Components.ElementAt(0));

            ent.AddComponent(tc2);
            ecs.FlushChanges();
            Assert.Equal(tc1, ent.Components.ElementAt(0));
            Assert.Equal(tc2, ent.Components.ElementAt(1));

            ent.Remove();
            ecs.Update(1f);

            Assert.Empty(ent.Components);
        }

        [Fact]
        public void HasComponentBig()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();

            ecs.Update(1f);

            Assert.False(ent.HasComponent<TestComponentOne>());
            Assert.False(ent.HasComponent(tc));

            ent.AddComponent(tc);

            Assert.False(ent.HasComponent<TestComponentOne>());
            Assert.False(ent.HasComponent(tc));

            ecs.FlushChanges();

            Assert.True(ent.HasComponent<TestComponentOne>());
            Assert.True(ent.HasComponent(tc));

            ent.Remove();
            ecs.Update(1f);

            Assert.False(ent.HasComponent<TestComponentOne>());
            Assert.False(ent.HasComponent(tc));
        }

        [Fact]
        public void EqualsSameEcsTest()
        {
            var ent = ecs.CreateEntity();
            var ent2 = ecs.CreateEntity();

            ecs.Update(1f);

            Assert.NotEqual(ent, ent2);
            Assert.NotEqual(ent2, ent);
            Assert.Equal(ent, ent);
            Assert.Equal(ent2, ent2);

            ent.Remove();
            ent2.Remove();
            ecs.Update(1f);
        }

        [Fact]
        public void GetHashCodeTest()
        {
            var ent = ecs.CreateEntity();
            var ent2 = ecs.CreateEntity();

            ecs.Update(1f);

            Assert.NotEqual(ent.GetHashCode(), ent2.GetHashCode());
            Assert.Equal(ent.GetHashCode(), ent.GetHashCode());

            ent.Remove();
            ent2.Remove();
            ecs.Update(1f);
        }
    }
}
