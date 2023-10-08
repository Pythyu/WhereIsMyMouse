using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Plugin.Services;

namespace WhereIsMyMouse
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Where's my mouse";

        private const string commandName = "/wmm";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }
        private ICondition Condition { get; init;  }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] ICondition condition)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.Condition = condition;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            
            this.PluginUi = new PluginUI(this.Configuration, this.PluginInterface, this.Condition);

            this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open Where's my mouse setting menu"
            });
            
            this.PluginInterface.UiBuilder.Draw += DrawUI;
        }

        public void Dispose()
        {
            // Save config
            PluginInterface.SavePluginConfig(this.Configuration);
            this.PluginInterface.UiBuilder.Draw -= DrawUI;
            this.CommandManager.RemoveHandler(commandName);
            this.PluginUi.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            this.PluginUi.Visible = true;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }
        
    }
}
