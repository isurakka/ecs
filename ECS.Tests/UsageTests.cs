using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    class FlagComponent : IComponent { }

    class Position : IComponent
    {
        public float X;
        public float Y;
    }

    class SimpleSystem : EntityProcessingSystem
    {
        public SimpleSystem() 
            : base(Aspect.All(typeof(Position)))
        {

        }

        protected override void Process(Entity entity, float deltaTime)
        {
            var pos = entity.GetComponent<Position>();
            pos.X += deltaTime;
        }
    }

    [Collection("ECS")]
    public class UsageTests
    {
        EntityComponentSystem ecs;

        public UsageTests()
        {
            this.ecs = EntityComponentSystem.Instance;
        }

        [Fact()]
        public void SimpleUsage()
        {
            var entity = ecs.CreateEntity();
            entity.AddComponent<FlagComponent>();
            var pos = entity.AddComponent<Position>();
            pos.X = 100;

            var simpleSystem = ecs.AddSystem<SimpleSystem>();

            ecs.Update(1f);
            ecs.Update(1f);

            Assert.Equal(102f, pos.X);

            ecs.RemoveSystem(simpleSystem, 0);
            entity.Remove();
            ecs.Update(0f);
        }
    }
}
