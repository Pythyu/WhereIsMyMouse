using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace WhereIsMyMouse;

internal class ConfigWindow : Window, IDisposable
{
    private readonly Configuration _configuration;

    private bool _cursorOn;
    private bool _foregroundCursor;
    private bool _enableInCombatOnly;
    private bool _rainbow;
    private float _size;
    private float _thickness;
    private float _cycleSpeed;
    private Vector4 _color;

    public ConfigWindow(Plugin plugin) : base("Where'sMyMouse Configuration###wheresmymouseconfig")
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(375, 600);
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 600),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        SizeCondition = ImGuiCond.Appearing;

        _configuration = plugin.Configuration;

        _thickness = _configuration.Thickness;
        _color = _configuration.Color;
        _size = _configuration.Size;
        _cycleSpeed = _configuration.CycleSpeed;
        _cursorOn = _configuration.CursorOn;
        _foregroundCursor = _configuration.ForegroundCursor;
        _enableInCombatOnly = _configuration.EnableInCombatOnly;
        _rainbow = _configuration.Rainbow;
    }

    public void Dispose() { }

    public override void PreDraw() { }

    public override void Draw()
    {
        ImGui.Text("Cursor Aura : ");
        ImGui.SameLine();
        ImGui.Checkbox("###CursorAura", ref _cursorOn);
        ImGui.Text("Circle drawn in foreground : ");
        ImGui.SameLine();
        ImGui.Checkbox("###ForegroundCursor", ref _foregroundCursor);
        ImGui.Text("Circle Size : ");
        ImGui.SameLine();
        ImGui.SliderFloat("###sizeslide", ref _size, 0f, 100f);
        ImGui.Text("Thickness : ");
        ImGui.SameLine();
        ImGui.SliderFloat("###thickslide", ref _thickness, 0f, 50f);
        ImGui.ColorPicker4("###ColorPicker", ref _color);
        ImGui.Text("Enable only in combat : ");
        ImGui.SameLine();
        ImGui.Checkbox("###CombatOnlyEnable", ref _enableInCombatOnly);
        ImGui.Text("Rainbow : ");
        ImGui.SameLine();
        ImGui.Checkbox("###Rainbow", ref _rainbow);
        ImGui.Text("Rainbow Cycle Speed : ");
        ImGui.SliderFloat("###cyclespeedslide", ref _cycleSpeed, 0.01f, 1f);
        // ReSharper disable once InvertIf
        if (ImGui.Button("Save Settings"))
        {
            _configuration.Color = _color;
            _configuration.Size = _size;
            _configuration.Thickness = _thickness;
            _configuration.CycleSpeed = _cycleSpeed;
            _configuration.CursorOn = _cursorOn;
            _configuration.ForegroundCursor = _foregroundCursor;
            _configuration.EnableInCombatOnly = _enableInCombatOnly;
            _configuration.Rainbow = _rainbow;
            _configuration.Save();
        }
    }
}