using System.Linq;

Output.Write($@"
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ECS
{{
    public partial class EntityComponentSystem
    {{

    ");

var versions = 16;

for (int i = 0; i < versions; i++)
{
    var componentCount = i + 1;
    if (componentCount == 1) continue;

    Output.Write($@"
    public async Task<ComponentAccess>");

    Output.Write($@"
        GetComponents<");
    Output.Write(string.Join(", ", Enumerable.Range(0, componentCount).Select(j => $@"
        TComponent{j}")));
    Output.Write($@">(");

    Output.Write($@"            ");
    Output.Write(string.Join(", ", Enumerable.Range(0, componentCount).Select(j => $@"
        ComponentAccessMode componentAccess{j}")));
    Output.Write($@")");

    Output.Write(
            $@"
    {{
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);");

    for (int j = 0; j < componentCount; j++)
    {
        Output.Write($@"

        // Try acquire lock{j}
        IDisposable lock{j};
        if (componentAccess0 == ComponentAccessMode.Write)
        {{
            lock{j} = componentLocks[typeof(TComponent{j})].TryWriterLock();
        }}
        else
        {{
            lock{j} = componentLocks[typeof(TComponent{j})].TryReaderLock();
        }}

        if (lock{j} == null)
        {{");

        for (int k = 0; k < j; k++)
        {
            Output.Write($@"
            lock{k}.Dispose();");
        }

        Output.Write($@"
            componentLockAcquirer.Release();
            goto start;
        }}
        ");
    }

    Output.Write($@"
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();");

    Output.Write($@"
        return new ComponentAccess");

    Output.Write($@"
        {{
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {{");

    Output.Write(string.Join(", ", Enumerable.Range(0, componentCount).Select(j => $@"lock{j}")));

    Output.Write($@"}},
            Entities = GetEntities(");

    Output.Write(string.Join(", ", Enumerable.Range(0, componentCount).Select(j => $@"
                typeof(TComponent{j})")));
    
    Output.Write($@")
        }};
    }}
    ");
}

Output.Write(
$@"
    }}
}}");