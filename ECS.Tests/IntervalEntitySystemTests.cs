using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    [Collection("ECS")]
    public class IntervalEntitySystemTests
    {
        class TestIntervalEntitySystem : IntervalEntityProcessingSystem
        {
            public int Processings = 0;

            public TestIntervalEntitySystem()
                : base(Aspect.Any(), 1f)
            { }

            protected override void Process(Entity entity, float deltaTime)
            {
                Processings++;
            }
        }

        EntityComponentSystem ecs;

        public IntervalEntitySystemTests()
        {
            this.ecs = EntityComponentSystem.Instance;
        }

        [Fact()]
        public void ProcessCorrectIntervalTest()
        {
            var sys = new TestIntervalEntitySystem();
            ecs.AddSystem(sys);
            var ent = ecs.CreateEntity();

            ecs.Update(0.5f);

            Assert.Equal(0, sys.Processings);

            ecs.Update(0.5f);

            Assert.Equal(1, sys.Processings);

            ecs.Update(1f);

            Assert.Equal(1f, sys.Interval);
            Assert.Equal(2, sys.Processings);

            ecs.Update(8f);

            Assert.Equal(10, sys.Processings);

            Assert.Equal(1f, sys.Interval);

            ecs.RemoveSystem(sys, 0);
            ent.Remove();
            ecs.Update(0f);
        }
    }
}
