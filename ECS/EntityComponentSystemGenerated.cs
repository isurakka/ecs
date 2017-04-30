
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
{
    public partial class EntityComponentSystem
    {

    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>)>>
        GetComponents<
        TComponent0, 
        TComponent1>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>, 
        IEnumerable<TComponent10>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9, 
        TComponent10>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9, 
        ComponentAccess componentAccess10)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock10
        IDisposable lock10;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock10 = componentLocks[typeof(TComponent10)].TryWriterLock();
        }
        else
        {
            lock10 = componentLocks[typeof(TComponent10)].TryReaderLock();
        }

        if (lock10 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>, IEnumerable<TComponent10>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9, lock10},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>(), 
                components[typeof(TComponent10)].Cast<TComponent10>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>, 
        IEnumerable<TComponent10>, 
        IEnumerable<TComponent11>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9, 
        TComponent10, 
        TComponent11>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9, 
        ComponentAccess componentAccess10, 
        ComponentAccess componentAccess11)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock10
        IDisposable lock10;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock10 = componentLocks[typeof(TComponent10)].TryWriterLock();
        }
        else
        {
            lock10 = componentLocks[typeof(TComponent10)].TryReaderLock();
        }

        if (lock10 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock11
        IDisposable lock11;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock11 = componentLocks[typeof(TComponent11)].TryWriterLock();
        }
        else
        {
            lock11 = componentLocks[typeof(TComponent11)].TryReaderLock();
        }

        if (lock11 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>, IEnumerable<TComponent10>, IEnumerable<TComponent11>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9, lock10, lock11},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>(), 
                components[typeof(TComponent10)].Cast<TComponent10>(), 
                components[typeof(TComponent11)].Cast<TComponent11>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>, 
        IEnumerable<TComponent10>, 
        IEnumerable<TComponent11>, 
        IEnumerable<TComponent12>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9, 
        TComponent10, 
        TComponent11, 
        TComponent12>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9, 
        ComponentAccess componentAccess10, 
        ComponentAccess componentAccess11, 
        ComponentAccess componentAccess12)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock10
        IDisposable lock10;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock10 = componentLocks[typeof(TComponent10)].TryWriterLock();
        }
        else
        {
            lock10 = componentLocks[typeof(TComponent10)].TryReaderLock();
        }

        if (lock10 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock11
        IDisposable lock11;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock11 = componentLocks[typeof(TComponent11)].TryWriterLock();
        }
        else
        {
            lock11 = componentLocks[typeof(TComponent11)].TryReaderLock();
        }

        if (lock11 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock12
        IDisposable lock12;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock12 = componentLocks[typeof(TComponent12)].TryWriterLock();
        }
        else
        {
            lock12 = componentLocks[typeof(TComponent12)].TryReaderLock();
        }

        if (lock12 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>, IEnumerable<TComponent10>, IEnumerable<TComponent11>, IEnumerable<TComponent12>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9, lock10, lock11, lock12},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>(), 
                components[typeof(TComponent10)].Cast<TComponent10>(), 
                components[typeof(TComponent11)].Cast<TComponent11>(), 
                components[typeof(TComponent12)].Cast<TComponent12>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>, 
        IEnumerable<TComponent10>, 
        IEnumerable<TComponent11>, 
        IEnumerable<TComponent12>, 
        IEnumerable<TComponent13>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9, 
        TComponent10, 
        TComponent11, 
        TComponent12, 
        TComponent13>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9, 
        ComponentAccess componentAccess10, 
        ComponentAccess componentAccess11, 
        ComponentAccess componentAccess12, 
        ComponentAccess componentAccess13)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock10
        IDisposable lock10;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock10 = componentLocks[typeof(TComponent10)].TryWriterLock();
        }
        else
        {
            lock10 = componentLocks[typeof(TComponent10)].TryReaderLock();
        }

        if (lock10 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock11
        IDisposable lock11;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock11 = componentLocks[typeof(TComponent11)].TryWriterLock();
        }
        else
        {
            lock11 = componentLocks[typeof(TComponent11)].TryReaderLock();
        }

        if (lock11 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock12
        IDisposable lock12;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock12 = componentLocks[typeof(TComponent12)].TryWriterLock();
        }
        else
        {
            lock12 = componentLocks[typeof(TComponent12)].TryReaderLock();
        }

        if (lock12 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock13
        IDisposable lock13;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock13 = componentLocks[typeof(TComponent13)].TryWriterLock();
        }
        else
        {
            lock13 = componentLocks[typeof(TComponent13)].TryReaderLock();
        }

        if (lock13 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            lock12.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>, IEnumerable<TComponent10>, IEnumerable<TComponent11>, IEnumerable<TComponent12>, IEnumerable<TComponent13>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9, lock10, lock11, lock12, lock13},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>(), 
                components[typeof(TComponent10)].Cast<TComponent10>(), 
                components[typeof(TComponent11)].Cast<TComponent11>(), 
                components[typeof(TComponent12)].Cast<TComponent12>(), 
                components[typeof(TComponent13)].Cast<TComponent13>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>, 
        IEnumerable<TComponent10>, 
        IEnumerable<TComponent11>, 
        IEnumerable<TComponent12>, 
        IEnumerable<TComponent13>, 
        IEnumerable<TComponent14>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9, 
        TComponent10, 
        TComponent11, 
        TComponent12, 
        TComponent13, 
        TComponent14>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9, 
        ComponentAccess componentAccess10, 
        ComponentAccess componentAccess11, 
        ComponentAccess componentAccess12, 
        ComponentAccess componentAccess13, 
        ComponentAccess componentAccess14)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock10
        IDisposable lock10;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock10 = componentLocks[typeof(TComponent10)].TryWriterLock();
        }
        else
        {
            lock10 = componentLocks[typeof(TComponent10)].TryReaderLock();
        }

        if (lock10 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock11
        IDisposable lock11;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock11 = componentLocks[typeof(TComponent11)].TryWriterLock();
        }
        else
        {
            lock11 = componentLocks[typeof(TComponent11)].TryReaderLock();
        }

        if (lock11 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock12
        IDisposable lock12;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock12 = componentLocks[typeof(TComponent12)].TryWriterLock();
        }
        else
        {
            lock12 = componentLocks[typeof(TComponent12)].TryReaderLock();
        }

        if (lock12 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock13
        IDisposable lock13;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock13 = componentLocks[typeof(TComponent13)].TryWriterLock();
        }
        else
        {
            lock13 = componentLocks[typeof(TComponent13)].TryReaderLock();
        }

        if (lock13 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            lock12.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock14
        IDisposable lock14;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock14 = componentLocks[typeof(TComponent14)].TryWriterLock();
        }
        else
        {
            lock14 = componentLocks[typeof(TComponent14)].TryReaderLock();
        }

        if (lock14 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            lock12.Dispose();
            lock13.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>, IEnumerable<TComponent10>, IEnumerable<TComponent11>, IEnumerable<TComponent12>, IEnumerable<TComponent13>, IEnumerable<TComponent14>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9, lock10, lock11, lock12, lock13, lock14},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>(), 
                components[typeof(TComponent10)].Cast<TComponent10>(), 
                components[typeof(TComponent11)].Cast<TComponent11>(), 
                components[typeof(TComponent12)].Cast<TComponent12>(), 
                components[typeof(TComponent13)].Cast<TComponent13>(), 
                components[typeof(TComponent14)].Cast<TComponent14>()),
        };
    }
    
    public async Task<ComponentAccess<(
        IEnumerable<TComponent0>, 
        IEnumerable<TComponent1>, 
        IEnumerable<TComponent2>, 
        IEnumerable<TComponent3>, 
        IEnumerable<TComponent4>, 
        IEnumerable<TComponent5>, 
        IEnumerable<TComponent6>, 
        IEnumerable<TComponent7>, 
        IEnumerable<TComponent8>, 
        IEnumerable<TComponent9>, 
        IEnumerable<TComponent10>, 
        IEnumerable<TComponent11>, 
        IEnumerable<TComponent12>, 
        IEnumerable<TComponent13>, 
        IEnumerable<TComponent14>, 
        IEnumerable<TComponent15>)>>
        GetComponents<
        TComponent0, 
        TComponent1, 
        TComponent2, 
        TComponent3, 
        TComponent4, 
        TComponent5, 
        TComponent6, 
        TComponent7, 
        TComponent8, 
        TComponent9, 
        TComponent10, 
        TComponent11, 
        TComponent12, 
        TComponent13, 
        TComponent14, 
        TComponent15>(            
        ComponentAccess componentAccess0, 
        ComponentAccess componentAccess1, 
        ComponentAccess componentAccess2, 
        ComponentAccess componentAccess3, 
        ComponentAccess componentAccess4, 
        ComponentAccess componentAccess5, 
        ComponentAccess componentAccess6, 
        ComponentAccess componentAccess7, 
        ComponentAccess componentAccess8, 
        ComponentAccess componentAccess9, 
        ComponentAccess componentAccess10, 
        ComponentAccess componentAccess11, 
        ComponentAccess componentAccess12, 
        ComponentAccess componentAccess13, 
        ComponentAccess componentAccess14, 
        ComponentAccess componentAccess15)
    {
        var freedEvent = new AsyncCountdownEvent(0);
        var guid = Guid.NewGuid();
        var success = componentsFreed.TryAdd(guid, freedEvent);
        if (!success) throw new InvalidOperationException();

        start:
        await freedEvent.WaitAsync();
        // Deadlock possible here?
        freedEvent.AddCount();
        await componentLockAcquirer.WaitAsync().ConfigureAwait(false);

        // Try acquire lock0
        IDisposable lock0;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock0 = componentLocks[typeof(TComponent0)].TryWriterLock();
        }
        else
        {
            lock0 = componentLocks[typeof(TComponent0)].TryReaderLock();
        }

        if (lock0 == null)
        {
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock1
        IDisposable lock1;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock1 = componentLocks[typeof(TComponent1)].TryWriterLock();
        }
        else
        {
            lock1 = componentLocks[typeof(TComponent1)].TryReaderLock();
        }

        if (lock1 == null)
        {
            lock0.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock2
        IDisposable lock2;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock2 = componentLocks[typeof(TComponent2)].TryWriterLock();
        }
        else
        {
            lock2 = componentLocks[typeof(TComponent2)].TryReaderLock();
        }

        if (lock2 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock3
        IDisposable lock3;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock3 = componentLocks[typeof(TComponent3)].TryWriterLock();
        }
        else
        {
            lock3 = componentLocks[typeof(TComponent3)].TryReaderLock();
        }

        if (lock3 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock4
        IDisposable lock4;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock4 = componentLocks[typeof(TComponent4)].TryWriterLock();
        }
        else
        {
            lock4 = componentLocks[typeof(TComponent4)].TryReaderLock();
        }

        if (lock4 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock5
        IDisposable lock5;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock5 = componentLocks[typeof(TComponent5)].TryWriterLock();
        }
        else
        {
            lock5 = componentLocks[typeof(TComponent5)].TryReaderLock();
        }

        if (lock5 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock6
        IDisposable lock6;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock6 = componentLocks[typeof(TComponent6)].TryWriterLock();
        }
        else
        {
            lock6 = componentLocks[typeof(TComponent6)].TryReaderLock();
        }

        if (lock6 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock7
        IDisposable lock7;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock7 = componentLocks[typeof(TComponent7)].TryWriterLock();
        }
        else
        {
            lock7 = componentLocks[typeof(TComponent7)].TryReaderLock();
        }

        if (lock7 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock8
        IDisposable lock8;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock8 = componentLocks[typeof(TComponent8)].TryWriterLock();
        }
        else
        {
            lock8 = componentLocks[typeof(TComponent8)].TryReaderLock();
        }

        if (lock8 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock9
        IDisposable lock9;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock9 = componentLocks[typeof(TComponent9)].TryWriterLock();
        }
        else
        {
            lock9 = componentLocks[typeof(TComponent9)].TryReaderLock();
        }

        if (lock9 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock10
        IDisposable lock10;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock10 = componentLocks[typeof(TComponent10)].TryWriterLock();
        }
        else
        {
            lock10 = componentLocks[typeof(TComponent10)].TryReaderLock();
        }

        if (lock10 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock11
        IDisposable lock11;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock11 = componentLocks[typeof(TComponent11)].TryWriterLock();
        }
        else
        {
            lock11 = componentLocks[typeof(TComponent11)].TryReaderLock();
        }

        if (lock11 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock12
        IDisposable lock12;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock12 = componentLocks[typeof(TComponent12)].TryWriterLock();
        }
        else
        {
            lock12 = componentLocks[typeof(TComponent12)].TryReaderLock();
        }

        if (lock12 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock13
        IDisposable lock13;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock13 = componentLocks[typeof(TComponent13)].TryWriterLock();
        }
        else
        {
            lock13 = componentLocks[typeof(TComponent13)].TryReaderLock();
        }

        if (lock13 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            lock12.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock14
        IDisposable lock14;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock14 = componentLocks[typeof(TComponent14)].TryWriterLock();
        }
        else
        {
            lock14 = componentLocks[typeof(TComponent14)].TryReaderLock();
        }

        if (lock14 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            lock12.Dispose();
            lock13.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        

        // Try acquire lock15
        IDisposable lock15;
        if (componentAccess0 == ComponentAccess.Write)
        {
            lock15 = componentLocks[typeof(TComponent15)].TryWriterLock();
        }
        else
        {
            lock15 = componentLocks[typeof(TComponent15)].TryReaderLock();
        }

        if (lock15 == null)
        {
            lock0.Dispose();
            lock1.Dispose();
            lock2.Dispose();
            lock3.Dispose();
            lock4.Dispose();
            lock5.Dispose();
            lock6.Dispose();
            lock7.Dispose();
            lock8.Dispose();
            lock9.Dispose();
            lock10.Dispose();
            lock11.Dispose();
            lock12.Dispose();
            lock13.Dispose();
            lock14.Dispose();
            componentLockAcquirer.Release();
            goto start;
        }
        
        // We got all read/write locks here

        componentLockAcquirer.Release();

        success = componentsFreed.TryRemove(guid, out var _);
        if (!success) throw new InvalidOperationException();
        return new ComponentAccess<(IEnumerable<TComponent0>, IEnumerable<TComponent1>, IEnumerable<TComponent2>, IEnumerable<TComponent3>, IEnumerable<TComponent4>, IEnumerable<TComponent5>, IEnumerable<TComponent6>, IEnumerable<TComponent7>, IEnumerable<TComponent8>, IEnumerable<TComponent9>, IEnumerable<TComponent10>, IEnumerable<TComponent11>, IEnumerable<TComponent12>, IEnumerable<TComponent13>, IEnumerable<TComponent14>, IEnumerable<TComponent15>)>
        {
            FreedEvents = componentsFreed,
            Acquires = new List<IDisposable> {lock0, lock1, lock2, lock3, lock4, lock5, lock6, lock7, lock8, lock9, lock10, lock11, lock12, lock13, lock14, lock15},
            Components = (
                components[typeof(TComponent0)].Cast<TComponent0>(), 
                components[typeof(TComponent1)].Cast<TComponent1>(), 
                components[typeof(TComponent2)].Cast<TComponent2>(), 
                components[typeof(TComponent3)].Cast<TComponent3>(), 
                components[typeof(TComponent4)].Cast<TComponent4>(), 
                components[typeof(TComponent5)].Cast<TComponent5>(), 
                components[typeof(TComponent6)].Cast<TComponent6>(), 
                components[typeof(TComponent7)].Cast<TComponent7>(), 
                components[typeof(TComponent8)].Cast<TComponent8>(), 
                components[typeof(TComponent9)].Cast<TComponent9>(), 
                components[typeof(TComponent10)].Cast<TComponent10>(), 
                components[typeof(TComponent11)].Cast<TComponent11>(), 
                components[typeof(TComponent12)].Cast<TComponent12>(), 
                components[typeof(TComponent13)].Cast<TComponent13>(), 
                components[typeof(TComponent14)].Cast<TComponent14>(), 
                components[typeof(TComponent15)].Cast<TComponent15>()),
        };
    }
    
    }
}