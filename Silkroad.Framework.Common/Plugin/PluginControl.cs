using System.Windows.Forms;

namespace Silkroad.Framework.Common.Plugin
{
    public partial class PluginControl : UserControl, IPluginControl
    {
        private IPlugin _plugin;

        public PluginControl(IPlugin plugin)
        {
            _plugin = plugin;

            InitializeComponent();

            label1.Text = $"My name is {_plugin.Name}, position {_plugin.Index} serving for {_plugin.Service.Settings.Name}";            
        }
    }
}