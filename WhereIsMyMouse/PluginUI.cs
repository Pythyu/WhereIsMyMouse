using Dalamud.Bindings.ImGui;
using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina;
using StructsCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace WhereIsMyMouse
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;

        private bool CursorOn = false;

        private bool ForegroundCursor = false;

        private bool EnableInCombatOnly = false;

        private bool Rainbow = false;

        private float size = 15;

        private float thickness = 2;

        private float cycleSpeed = 0.10f;

        private Vector4 color = new Vector4(1, 0, 0, 1);
        
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        
        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
            this.thickness = this.configuration.Thickness;
            this.color = this.configuration.Color;
            this.size = this.configuration.Size;
            this.cycleSpeed = this.configuration.CycleSpeed;
            this.CursorOn = this.configuration.CursorOn;
            this.ForegroundCursor = this.configuration.ForegroundCursor;
            this.EnableInCombatOnly = this.configuration.EnableInCombatOnly;
            this.Rainbow = this.configuration.Rainbow;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            CursorAura();
            DrawMainWindow();
        }

        public void ToggleUI()
        {
            Visible = !Visible;
        }

        public void CursorAura()
        {
            if (!CursorOn)
            {
                return;
            }

            if (EnableInCombatOnly && !Plugin.Condition[ConditionFlag.InCombat])
            {
                return;
            }

            if (Rainbow)
            {
                float h = 0, s = 0, v = 0;
                float outr = 0, outg = 0, outb = 0;
                ImGui.ColorConvertRGBtoHSV(color.X, color.Y, color.Z, ref h, ref s, ref v);
                if (h >= 1)
                {
                    h = 0;
                }
                h += cycleSpeed * ImGui.GetIO().DeltaTime;
                ImGui.ColorConvertHSVtoRGB(h, s, v, ref outr, ref outg, ref outb);
                color.X = outr;
                color.Y = outg;
                color.Z = outb;
            }

            Vector2 cursorPos = ImGui.GetMousePos();
            ImGui.SetNextWindowSize(new Vector2(300, 300));
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.Begin("CursorWindow", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoFocusOnAppearing);
            ImGui.SetWindowPos(cursorPos - new Vector2(150,150));
            
            ImDrawListPtr draw;
            draw = this.ForegroundCursor ? ImGui.GetForegroundDrawList() : ImGui.GetWindowDrawList();
            
            draw.AddCircle(cursorPos, size, ImGui.ColorConvertFloat4ToU32(this.color), 0, this.thickness);
            ImGui.End();
        }

        public void SaveSettings()
        {
            this.configuration.Color = this.color;
            this.configuration.Size = this.size;
            this.configuration.Thickness = this.thickness;
            this.configuration.CycleSpeed = this.cycleSpeed;
            this.configuration.CursorOn = this.CursorOn;
            this.configuration.ForegroundCursor = this.ForegroundCursor;
            this.configuration.EnableInCombatOnly = this.EnableInCombatOnly;
            this.configuration.Rainbow = this.Rainbow;
            Plugin.PluginInterface.SavePluginConfig(this.configuration);
        }
        

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }
            ImGui.SetNextWindowSize(new Vector2(375, 600), ImGuiCond.Appearing);
            ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Cursor Settings", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.Text("Cursor Aura : ");
                ImGui.SameLine();
                ImGui.Checkbox("###CursorAura", ref this.CursorOn);
                ImGui.Text("Circle drawn in foreground : ");
                ImGui.SameLine();
                ImGui.Checkbox("###ForegroundCursor", ref this.ForegroundCursor);
                ImGui.Text("Circle Size : ");
                ImGui.SameLine();
                ImGui.SliderFloat("###sizeslide", ref this.size, 0f, 100f);
                ImGui.Text("Thickness : ");
                ImGui.SameLine();
                ImGui.SliderFloat("###thickslide", ref this.thickness, 0f, 50f);
                ImGui.ColorPicker4("###ColorPicker", ref this.color);
                ImGui.Text("Enable only in combat : ");
                ImGui.SameLine();
                ImGui.Checkbox("###CombatOnlyEnable", ref this.EnableInCombatOnly);
                ImGui.Text("Rainbow : ");
                ImGui.SameLine();
                ImGui.Checkbox("###Rainbow", ref this.Rainbow);
                ImGui.Text("Rainbow Cycle Speed : ");
                ImGui.SliderFloat("###cyclespeedslide", ref this.cycleSpeed, 0.01f, 1f);
                if (ImGui.Button("Save Settings"))
                {
                    SaveSettings();
                }
            }
            ImGui.End();
        }
        
    }
}
