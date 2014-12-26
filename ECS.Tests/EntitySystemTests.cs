using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    public class TestComponentOne : IComponent { }
    public class TestComponentTwo : IComponent { }

    public class TestSystemOne : EntitySystem
    {
        public TestSystemOne()
            : base(Aspect.All(typeof(TestComponentTwo)))
        {

        }

        protected override void process(Entity entity, float deltaTime)
        {
            throw new NotImplementedException();
        }
    }

    public class EntitySystemTests
    {
        [Fact()]
        public void ProcessTest()
        {
            var ecs = new EntityComponentSystem();
            int systemUpdates = 0;
            ecs.AddSystem(new SmallEntitySystem(
                Aspect.All(typeof(TestComponentOne)),
                (e, dt) =>
                {
                    systemUpdates++;
                }));

            var ent = ecs.CreateEntity();

            ecs.Update(1f);
            Assert.Equal(0, systemUpdates);

            ent.AddComponent(new TestComponentOne());

            ecs.Update(1f);
            Assert.Equal(1, systemUpdates);
        }
    }
}
