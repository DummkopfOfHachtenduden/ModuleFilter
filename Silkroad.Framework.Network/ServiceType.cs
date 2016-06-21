namespace Silkroad.Framework.Common
{
    public enum ServiceType : byte
    {
        None = 0,
        Certification = 1,
        GlobalManager = 2,
        DownloadServer = 3,
        GatewayServer = 4,
        FarmManager = 5,
        AgentServer = 6,
        SR_ShardManager = 7,
        SR_GameServer = 8,
        SR_Client = 9,
        ServiceManager = 10,
        MachineManager = 11,
        JmxMsgSvr = 12,
        JmxMessenger = 13,
        SMC = 14,
        CPRJ_Client = 15,
        CPRJ_GameServer = 16,
        CPRJ_ShardManager = 17,
    }
}