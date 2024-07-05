using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Diagnostics;
using System.Numerics;

namespace WhereIsMyMouse
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool CursorOn = false;

        public bool ForegroundCursor = false;
        
        public bool EnableInCombatOnly = false;

        public bool Rainbow = false;

        public float Size = 15;

        public float Thickness = 2;

        public float CycleSpeed = 0.10f;

        public Vector4 Color = new Vector4(1, 0, 0, 1);

        public void Save()
        {
            Trace.WriteLine("saving settings");
            Plugin.PluginInterface!.SavePluginConfig(this);
        }
    }
}
