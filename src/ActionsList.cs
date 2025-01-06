using Conesoft.Hosting;
using System;
using System.Collections.Generic;

namespace Conesoft.Services.HomeDevicesControl;

public class ActionsList
{
    readonly List<string> actions = ["hi", "there"];
    public IReadOnlyList<string> Actions => actions;

    public ActionsList(HostEnvironment environment)
    {
        if(environment.IsInHostedEnvironment)
        {
            actions.Clear();
        }
    }

    public void Add(string action)
    {
        actions.Add(action);
        StateChanged?.Invoke();

#pragma warning disable IDE0079
#pragma warning disable CA1416
        try
        {
            Console.Beep(600 * (int.Parse(action[^1..]) - 3), 100);
            Console.Beep(450 * (int.Parse(action[^1..]) - 3), 100);
            Console.Beep(1200 * (int.Parse(action[^1..]) - 3), 100);
        }
        catch (Exception)
        {
        }
#pragma warning restore CA1416
#pragma warning restore IDE0079

    }

    public void Clear()
    {
        actions.Clear();
        StateChanged?.Invoke();
    }

    public delegate void StateChangedEventHandler();

    public event StateChangedEventHandler? StateChanged;
}
