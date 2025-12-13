using Dalamud.Configuration;
using System;
using System.Numerics;

namespace WhereIsMyMouse;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool CursorOn { get; set; }
    public bool ForegroundCursor { get; set; }
    public bool EnableInCombatOnly { get; set; }
    public bool Rainbow { get; set; }
    public float Size { get; set; } = 15;
    public float Thickness { get; set; } = 2;
    public float CycleSpeed { get; set; } = 0.10f;
    public Vector4 Color { get; set; } = new(1, 0, 0, 1);

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}