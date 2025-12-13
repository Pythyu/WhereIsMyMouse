using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Windowing;

namespace WhereIsMyMouse;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static ICondition Condition { get; private set; } = null!;

    private const string CommandName = "/wmm";

    public Configuration Configuration { get; init;  }

    private readonly WindowSystem _windowSystem = new("WheresMyMouse");
    private ConfigWindow ConfigWindow { get; init; }
    private MouseWindow MouseWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            
        MouseWindow = new MouseWindow(this);

        ConfigWindow = new ConfigWindow(this);
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open Where's my mouse setting menu"
        });

        _windowSystem.AddWindow(ConfigWindow);

        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleConfigUi;
        PluginInterface.UiBuilder.Draw += _windowSystem.Draw;
        PluginInterface.UiBuilder.Draw += Draw;
    }

    public void Dispose()
    {
        // Save config
        PluginInterface.SavePluginConfig(Configuration);

        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.Draw -= _windowSystem.Draw;
        PluginInterface.UiBuilder.Draw -= Draw;

        _windowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MouseWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        ConfigWindow.Toggle();
    }

    private void Draw()
    {
        MouseWindow.Draw();
    }

    private void ToggleConfigUi() => ConfigWindow.Toggle();
}