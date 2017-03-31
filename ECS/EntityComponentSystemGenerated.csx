#r "System.Core"

using System.Linq;

var componentCount = 16;

// namespace start
Output.Write($@"
using System.Threading;

namespace ECS
{{");

for (int i = 1; i < componentCount; i++)
{
    // class start
    Output.Write($@"
    public sealed class EntityComponentSystem{$"<{string.Join(", ", Enumerable.Range(0, i).Select(v => "T" + v))}>"}
    {{");

    // same for each class

    // fields
    Output.Write($@"
        private int nextEntityId;
    ");

    // for each component type start
    for (int j = 0; j < i; j++)
    {
        Output.Write($@"
        private T{j}[] component{j};");
    }

    // create entity
    Output.Write($@"

        public int CreateEntity()
        {{
            var entityId = Interlocked.Increment(ref nextEntityId);
            return entityId;
        }}
    ");

    // get components

    // class end
    Output.Write($@"
    }}");
}

// namespace end
Output.Write($@"}}");