using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;

namespace WhereIsMyMouse;

internal class MouseWindow : Window, IDisposable
{
    private readonly Plugin _plugin;

    public MouseWindow(Plugin plugin) : base("Mouse Window###CursorWindow", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(300, 300);

        _plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (!_plugin.Configuration.CursorOn)
        {
            return;
        }

        if (_plugin.Configuration.EnableInCombatOnly && !Plugin.Condition[ConditionFlag.InCombat])
        {
            return;
        }

        var color = _plugin.Configuration.Color;
        if (_plugin.Configuration.Rainbow)
        {
            float h = 0, s = 0, v = 0;
            float outr = 0, outg = 0, outb = 0;
            ImGui.ColorConvertRGBtoHSV(color.X, color.Y, color.Z, ref h, ref s, ref v);
            if (h >= 1)
            {
                h = 0;
            }
            h += _plugin.Configuration.CycleSpeed * ImGui.GetIO().DeltaTime;
            ImGui.ColorConvertHSVtoRGB(h, s, v, ref outr, ref outg, ref outb);
            color.X = outr;
            color.Y = outg;
            color.Z = outb;
            _plugin.Configuration.Color = color;
        }

        var cursorPos = ImGui.GetMousePos();
        ImGuiHelpers.ForceNextWindowMainViewport();
        ImGui.SetWindowPos(cursorPos - new Vector2(150,150));

        var draw = _plugin.Configuration.ForegroundCursor ? ImGui.GetForegroundDrawList() : ImGui.GetWindowDrawList();

        draw.AddCircle(cursorPos, _plugin.Configuration.Size, ImGui.ColorConvertFloat4ToU32(color), 0, _plugin.Configuration.Thickness);
    }
}