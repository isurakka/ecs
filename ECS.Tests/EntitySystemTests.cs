using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Xunit;
namespace ECS.Tests
{
    public class EntitySystemTests : IClassFixture<EntityComponentSystem>
    {
        EntityComponentSystem ecs;

        public EntitySystemTests(EntityComponentSystem ecs)
        {
            this.ecs = ecs;
        }

        [Fact()]
        public void ProcessCorrectEntitiesTest()
        {
            int processings = 0;
            var sys = new ClosureEntitySystem(Aspect.Empty(), null)
            {
                ProcessorAction = (e, dt) => processings++,
            };
            ecs.AddSystem(sys);

            ecs.Update(1f);

            Assert.Equal(0, processings);

            var ent = ecs.CreateEntity();
            sys.ProcessorAction = (e, dt) => { processings++; Assert.Equal(e, ent); };
            ecs.Update(1f);
            Assert.Equal(1, processings);

            var ent2 = ecs.CreateEntity();
            bool processedEnt = false;
            bool processedEnt2 = false;
            sys.ProcessorAction = (e, dt) => 
            { 
                processings++;
                if (e == ent)
                {
                    Assert.False(processedEnt);
                    processedEnt = true;
                }
                if (e == ent2)
                {
                    Assert.False(processedEnt2);
                    processedEnt2 = true;
                }
            };
            ecs.Update(1f);
            Assert.Equal(3, processings);
            Assert.True(processedEnt);
            Assert.True(processedEnt2);

            ent.Remove();
            sys.ProcessorAction = (e, dt) => { processings++; Assert.Equal(e, ent2); };
            ecs.Update(1f);
            Assert.Equal(4, processings);

            ent2.Remove();
            ecs.RemoveSystem(sys, 0);
            ecs.Update(1f);
            Assert.Equal(4, processings);
        }

        [Fact()]
        public void ProcessCorrectEntitiesWithComponentsTest()
        {
            int processings1 = 0;
            var sys1 = new ClosureEntitySystem(Aspect.All(typeof(TestComponentOne)), null)
            {
                ProcessorAction = (e, dt) => processings1++,
            };
            ecs.AddSystem(sys1);

            int processings2 = 0;
            var sys2 = new ClosureEntitySystem(Aspect.All(typeof(TestComponentTwo)), null)
            {
                ProcessorAction = (e, dt) => processings2++,
            };
            ecs.AddSystem(sys2);

            ecs.Update(1f);
            Assert.Equal(0, processings1);
            Assert.Equal(0, processings2);

            var ent = ecs.CreateEntity();
            ent.AddComponent(new TestComponentOne());
            ecs.Update(1f);
            Assert.Equal(1, processings1);
            Assert.Equal(0, processings2);

            ent.AddComponent(new TestComponentTwo());
            ecs.Update(1f);
            Assert.Equal(2, processings1);
            Assert.Equal(1, processings2);

            ent.Remove();
            ecs.RemoveSystem(sys1, 0);
            ecs.RemoveSystem(sys2, 0);
            ecs.Update(1f);
            Assert.Equal(2, processings1);
            Assert.Equal(1, processings2);
        }

        [Fact()]
        public void OnAddedTest()
        {
            int onAdded = 0;
            var sys = new ClosureEntitySystem(Aspect.All(typeof(TestComponentOne)), null)
            {
                OnAddedAction = (e) => onAdded++,
            };
            ecs.AddSystem(sys);
            ecs.Update(1f);
            Assert.Equal(0, onAdded);

            var ent = ecs.CreateEntity();
            ecs.Update(1f);
            Assert.Equal(0, onAdded);

            ent.AddComponent(new TestComponentOne());
            sys.OnAddedAction = (e) => { onAdded++; Assert.Equal(ent, e); };
            ecs.Update(1f);
            Assert.Equal(1, onAdded);

            var ent2 = ecs.CreateEntity();
            ent2.AddComponent(new TestComponentOne());
            sys.OnAddedAction = (e) => { onAdded++; Assert.Equal(ent2, e); };
            ecs.Update(1f);
            Assert.Equal(2, onAdded);

            ent.Remove();
            ent2.Remove();
            ecs.RemoveSystem(sys, 0);
            ecs.Update(1f);
            Assert.Equal(2, onAdded);
        }

        [Fact()]
        public void OnRemovedComponentOnlyTest()
        {
            int onRemoved = 0;
            var sys = new ClosureEntitySystem(Aspect.All(typeof(TestComponentOne)), null)
            {
                OnRemovedAction = (e) => onRemoved++,
            };
            ecs.AddSystem(sys);
            ecs.Update(1f);
            Assert.Equal(0, onRemoved);

            var ent = ecs.CreateEntity();
            ent.AddComponent(new TestComponentOne());
            ecs.Update(1f);
            Assert.Equal(0, onRemoved);

            var ent2 = ecs.CreateEntity();
            ent2.AddComponent(new TestComponentOne());
            ecs.Update(1f);
            Assert.Equal(0, onRemoved);

            ent.Remove();
            sys.OnRemovedAction = (e) => { onRemoved++; Assert.Equal(ent, e); };
            ecs.Update(1f);
            Assert.Equal(1, onRemoved);

            ent2.Remove();
            sys.OnRemovedAction = (e) => { onRemoved++; Assert.Equal(ent2, e); };
            ecs.Update(1f);
            Assert.Equal(2, onRemoved);

            ecs.RemoveSystem(sys, 0);
            ecs.Update(1f);
            Assert.Equal(2, onRemoved);
        }

        [Fact()]
        public void OnRemovedWholeEntityTest()
        {
            int onRemoved = 0;
            var sys = new ClosureEntitySystem(Aspect.All(typeof(TestComponentOne)), null)
            {
                OnRemovedAction = (e) => onRemoved++,
            };
            ecs.AddSystem(sys);
            var ent = ecs.CreateEntity();
            ent.AddComponent(new TestComponentOne());
            ecs.Update(1f);
            Assert.Equal(0, onRemoved);

            ent.Remove();
            sys.OnRemovedAction = (e) => { onRemoved++; Assert.Equal(ent, e); };
            var ent2 = ecs.CreateEntity();
            ent2.AddComponent(new TestComponentOne());
            ecs.Update(1f);
            Assert.Equal(1, onRemoved);

            ent2.Remove();
            sys.OnRemovedAction = (e) => { onRemoved++; Assert.Equal(ent2, e); };
            ecs.Update(1f);
            Assert.Equal(2, onRemoved);

            ecs.RemoveSystem(sys, 0);
            ecs.Update(1f);
        }

        [Fact()]
        public void BeginEndTest()
        {
            int begin = 0;
            int end = 0;
            var sys = new ClosureEntitySystem(Aspect.All(typeof(TestComponentOne)), null)
            {
                BeginAction = () => begin++,
                EndAction = () => end++,
            };
            ecs.AddSystem(sys);
            
            ecs.Update(1f);
            Assert.Equal(1, begin);
            Assert.Equal(1, end);

            var ent = ecs.CreateEntity();
            ent.AddComponent(new TestComponentOne());
            ecs.Update(1f);
            Assert.Equal(2, begin);
            Assert.Equal(2, end);

            ent.Remove();
            ecs.RemoveSystem(sys, 0);
            ecs.Update(1f);

            Assert.Equal(2, begin);
            Assert.Equal(2, end);
        }
    }
}
