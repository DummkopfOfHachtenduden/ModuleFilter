namespace Silkroad.Framework.Common.Plugin
{
    public interface IPlugin
    {
        int Index { get; }

        string Name { get; }

        Service Service { get; }

        void Register(string Name, Service service);

        IPluginControl Control { get; }
    }
}