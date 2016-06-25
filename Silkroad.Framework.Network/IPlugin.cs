namespace Silkroad.Framework.Common
{
    public interface IPlugin
    {
        string Name { get; }

        Service Service { get; }

        void Register(string Name, Service service);
    }
}