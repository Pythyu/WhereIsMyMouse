using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
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

        public float CycleSpeed = 0.005f;

        public Vector4 Color = new Vector4(1, 0, 0, 1);

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
