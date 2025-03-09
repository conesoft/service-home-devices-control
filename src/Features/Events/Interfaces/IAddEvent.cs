using Conesoft.Services.HomeDevicesControl.Features.Events.Types;
using System.Threading.Tasks;

namespace Conesoft.Services.HomeDevicesControl.Features.Events.Interfaces;

public interface IAddEvent
{
    Task Add(Event item);
}
