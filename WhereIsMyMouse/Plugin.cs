using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Plugin.Services;
using static FFXIVClientStructs.FFXIV.Client.UI.UIModule.Delegates;

namespace WhereIsMyMouse
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Where's my mouse";

        private const string commandName = "/wmm";

        [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }
        [PluginService] internal static ICondition Condition { get; private set; } = null!;

        public Plugin()
        {
            this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            
            this.PluginUi = new PluginUI(this.Configuration);

            CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open Where's my mouse setting menu"
            });


            PluginInterface.UiBuilder.OpenConfigUi += ToggleUI;
            PluginInterface.UiBuilder.Draw += Draw;
        }

        public void Dispose()
        {
            // Save config
            PluginInterface.SavePluginConfig(this.Configuration);

            PluginInterface.UiBuilder.OpenConfigUi -= ToggleUI;
            PluginInterface.UiBuilder.Draw -= Draw;

            CommandManager.RemoveHandler(commandName);
            this.PluginUi.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            this.PluginUi.Visible = true;
        }

        private void Draw()
        {
            this.PluginUi.Draw();
        }

        private void ToggleUI()
        {
            this.PluginUi.ToggleUI();
        }
    }
}
