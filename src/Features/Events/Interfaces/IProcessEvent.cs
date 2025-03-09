using Conesoft.Services.HomeDevicesControl.Features.Events.Types;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;

public interface IProcessEvent
{
    string Type { get; }
    Task Process(Event item);
}

public interface IProcessEvent<T> : IProcessEvent where T : Event
{
    string IProcessEvent.Type => typeof(T).Name;
    Task IProcessEvent.Process(Event item) => item switch
    {
        T => Process((T)item),
        _ => Task.CompletedTask
    };

    Task Process(T item);
}