using ImGuiNET;
using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Logging;
using Dalamud.Interface;
using Dalamud.Plugin;

namespace WhereIsMyMouse
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;
        
        private DalamudPluginInterface wmmInterface;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;

        private bool CursorOn = false;

        private bool ForegroundCursor = false;

        private float size = 15;

        private float thickness = 2;

        private Vector4 color = new Vector4(1, 0, 0, 1);
        
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }
        
        public PluginUI(Configuration configuration, DalamudPluginInterface wmmInterface)
        {
            this.configuration = configuration;
            this.wmmInterface = wmmInterface;
            this.thickness = this.configuration.Thickness;
            this.color = this.configuration.Color;
            this.size = this.configuration.Size;
            this.CursorOn = this.configuration.CursorOn;
            this.ForegroundCursor = this.configuration.ForegroundCursor;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            DrawMainWindow();
            CursorAura();
        }
        
        private uint ToUint(Vector4 c)
        {
            uint total = ((uint)(c.W*255) << 24) + ((uint)(c.Z*255) << 16) + ((uint)(c.Y*255) << 8) + (uint)(c.X*255);
            return total;
        }

        public void CursorAura()
        {
            if (!CursorOn)
            {
                return;
            }

            Vector2 cursorPos = ImGui.GetMousePos();
            ImGui.SetNextWindowSize(new Vector2(300, 300));
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGui.Begin("CursorWindow", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs);
            ImGui.SetWindowPos(cursorPos - new Vector2(150,150));
            
            ImDrawListPtr draw;
            draw = this.ForegroundCursor ? ImGui.GetForegroundDrawList() : ImGui.GetWindowDrawList();
            
            draw.AddCircle(cursorPos, size, this.ToUint(this.color), 0, this.thickness);
            ImGui.End();
        }

        public void SaveSettings()
        {
            this.configuration.Color = this.color;
            this.configuration.Size = this.size;
            this.configuration.Thickness = this.thickness;
            this.configuration.CursorOn = this.CursorOn;
            this.configuration.ForegroundCursor = this.ForegroundCursor;
            wmmInterface.SavePluginConfig(this.configuration);
        }
        

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }
            ImGui.SetNextWindowSize(new Vector2(375, 500), ImGuiCond.Appearing);
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
                if (ImGui.Button("Save Settings"))
                {
                    SaveSettings();
                }
            }
            ImGui.End();
        }
        
    }
}
