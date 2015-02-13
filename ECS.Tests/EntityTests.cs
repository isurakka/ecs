using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    public class EntityTests : IClassFixture<EntityComponentSystem>
    {
        EntityComponentSystem ecs;

        public EntityTests(EntityComponentSystem ecs)
        {
            this.ecs = ecs;
        }

        [Fact()]
        public void RemoveTest()
        {
            var ent = ecs.CreateEntity();
            ecs.Update(1f);
            Assert.Equal(ent, ecs.FindEntities(Aspect.Any()).First());
            ent.Remove();
            ecs.Update(1f);
            Assert.Equal(null, ecs.FindEntities(Aspect.Any()).FirstOrDefault());
        }

        [Fact()]
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

        [Fact()]
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

        [Fact()]
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

        [Fact()]
        public void HasComponentTest()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();

            Assert.False(ent.HasComponent<TestComponentOne>());

            ent.AddComponent(tc);

            Assert.True(ent.HasComponent<TestComponentOne>());

            ent.RemoveComponent(tc);

            Assert.False(ent.HasComponent<TestComponentOne>());

            ecs.Update(1f);
            ent.Remove();
            ecs.Update(1f);
        }

        [Fact()]
        public void GetComponentTest()
        {
            var ent = ecs.CreateEntity();
            var tc = new TestComponentOne();

            ecs.Update(1f);

            Assert.Equal(null, ent.GetComponent<TestComponentOne>());

            ent.AddComponent(tc);

            Assert.Equal(tc, ent.GetComponent<TestComponentOne>());

            ent.Remove();
            ecs.Update(1f);
        }

        [Fact()]
        public void EqualsSameECSTest()
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

        [Fact()]
        public void EqualsDifferentECSTest()
        {
            var ecs1 = new EntityComponentSystem();
            var ecs2 = new EntityComponentSystem();

            var ent1 = ecs1.CreateEntity();
            var ent2 = ecs2.CreateEntity();

            ecs1.Update(1f);
            ecs2.Update(1f);

            Assert.NotEqual(ent1, ent2);
            Assert.NotEqual(ent2, ent1);
        }

        [Fact()]
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
