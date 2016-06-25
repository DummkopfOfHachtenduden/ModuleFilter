using Silkroad.Framework.Common.Objects.Certification;
using Silkroad.Framework.Common.Objects.SecurityDesc;
using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;
using System.Collections.Generic;

namespace Silkroad.Framework.Common.Objects
{
    public class CertifiactionManager
    {
        public string RequestName { get; set; }
        public string RequestIP { get; set; }

        public Dictionary<byte, srServiceType> ServiceTypes { get; private set; }
        public Dictionary<byte, srOperationType> OperationTypes { get; private set; }
        public List<srGlobalService> GlobalServices { get; private set; }
        public Dictionary<byte, srGlobalOperation> GlobalOperations { get; private set; }
        public List<srUnknown> Unknown { get; private set; }
        public Dictionary<ushort, srShard> Shards { get; private set; }
        public Dictionary<uint, srNodeType> NodeTypes { get; private set; }
        public Dictionary<ushort, srNodeData> NodeData { get; private set; }
        public Dictionary<uint, srNodeLink> NodeLinks { get; private set; }

        public Dictionary<byte, SecurityDescriptionGroup> SecurityDescriptionGroups { get; private set; }
        public Dictionary<uint, SecurityDescription> SecurityDescriptions { get; private set; }
        public List<SecurityDescriptionGroupAssign> SecurityDescriptionGroupAssigns { get; private set; }

        public CertifiactionManager()
        {
            ServiceTypes = new Dictionary<byte, srServiceType>();
            OperationTypes = new Dictionary<byte, srOperationType>();
            GlobalServices = new List<srGlobalService>();
            GlobalOperations = new Dictionary<byte, srGlobalOperation>();
            Unknown = new List<srUnknown>();
            Shards = new Dictionary<ushort, srShard>();
            NodeTypes = new Dictionary<uint, srNodeType>();
            NodeData = new Dictionary<ushort, srNodeData>();
            NodeLinks = new Dictionary<uint, srNodeLink>();

            SecurityDescriptionGroups = new Dictionary<byte, SecurityDescriptionGroup>();
            SecurityDescriptions = new Dictionary<uint, SecurityDescription>();
            SecurityDescriptionGroupAssigns = new List<SecurityDescriptionGroupAssign>();
        }

        public void ReadReq(Packet packet)
        {
            this.RequestName = packet.ReadAscii();
            this.RequestIP = packet.ReadAscii();
        }

        public void WriteReq(Packet packet)
        {
            packet.WriteAscii(this.RequestName);
            packet.WriteAscii(this.RequestIP);
        }

        public void ReadAck(Packet packet)
        {
            var result = packet.ReadBool();
            if (result)
            {
                this.ReadDict<byte, srServiceType>(packet, ServiceTypes);
                this.ReadDict<byte, srOperationType>(packet, OperationTypes);
                this.ReadList<srGlobalService>(packet, GlobalServices);
                this.ReadDict<byte, srGlobalOperation>(packet, GlobalOperations);
                this.ReadList<srUnknown>(packet, Unknown);
                this.ReadDict<ushort, srShard>(packet, Shards);
                this.ReadDict<uint, srNodeType>(packet, NodeTypes);
                this.ReadDict<ushort, srNodeData>(packet, NodeData);
                this.ReadDict<uint, srNodeLink>(packet, NodeLinks);

                var hasSecurityDescription = packet.ReadBool();
                if (hasSecurityDescription)
                {
                    this.ReadDict<byte, SecurityDescriptionGroup>(packet, SecurityDescriptionGroups);
                    this.ReadDict<uint, SecurityDescription>(packet, SecurityDescriptions);
                    this.ReadList<SecurityDescriptionGroupAssign>(packet, SecurityDescriptionGroupAssigns);
                }
            }
        }

        public void ReadDict<TKey, TStruct>(Packet packet, IDictionary<TKey, TStruct> dict) where TStruct : Unmanaged.IUnmanagedStruct, IKeyStruct
        {
            dict.Clear();

            var unkByte0 = packet.ReadByte();
            while (true)
            {
                var entryFlag = packet.ReadByte();
                if (entryFlag == 1)
                {
                    var structure = packet.ReadStruct<TStruct>();
                    dict.Add(structure.Key, structure);
                }
                else if (entryFlag == 2)
                {
                    break;
                }
                else
                {
                    //TODO: Proper exception
                    StaticLogger.Instance.Error($"{nameof(CertifiactionManager)}->{Caller.GetMemberName()}: entry missmatch!");
                    break;
                }
            }
        }

        public void ReadList<TStruct>(Packet packet, IList<TStruct> list) where TStruct : Unmanaged.IUnmanagedStruct
        {
            list.Clear();

            var unkByte0 = packet.ReadByte();
            while (true)
            {
                var entryFlag = packet.ReadByte();
                if (entryFlag == 1)
                {
                    list.Add(packet.ReadStruct<TStruct>());
                }
                else if (entryFlag == 2)
                {
                    break;
                }
                else
                {
                    //TODO: Proper exception
                    StaticLogger.Instance.Error($"{nameof(CertifiactionManager)}->{Caller.GetMemberName()}: entry missmatch!");
                    break;
                }
            }
        }

        public void WriteAck(Packet packet, bool writeCertification, bool writeSecurityDesc)
        {
            packet.WriteBool(writeCertification);
            if (writeCertification)
            {
                this.WriteDict<byte, srServiceType>(packet, ServiceTypes);
                this.WriteDict<byte, srOperationType>(packet, OperationTypes);
                this.WriteList<srGlobalService>(packet, GlobalServices);
                this.WriteDict<byte, srGlobalOperation>(packet, GlobalOperations);
                this.WriteList<srUnknown>(packet, Unknown);
                this.WriteDict<ushort, srShard>(packet, Shards);
                this.WriteDict<uint, srNodeType>(packet, NodeTypes);
                this.WriteDict<ushort, srNodeData>(packet, NodeData);
                this.WriteDict<uint, srNodeLink>(packet, NodeLinks);
            }

            packet.WriteBool(writeSecurityDesc);
            if (writeSecurityDesc)
            {
                this.WriteDict<byte, SecurityDescriptionGroup>(packet, SecurityDescriptionGroups);
                this.WriteDict<uint, SecurityDescription>(packet, SecurityDescriptions);
                this.WriteList<SecurityDescriptionGroupAssign>(packet, SecurityDescriptionGroupAssigns);
            }
        }

        public void WriteDict<TKey, TStruct>(Packet packet, IDictionary<TKey, TStruct> dict) where TStruct : Unmanaged.IUnmanagedStruct
        {
            packet.WriteByte(0); //unkByte1
            foreach (KeyValuePair<TKey, TStruct> kvp in dict)
            {
                packet.WriteByte(1);
                packet.WriteStruct(kvp.Value);
            }
            packet.WriteByte(2);
        }

        public void WriteList<TStruct>(Packet packet, IList<TStruct> list) where TStruct : Unmanaged.IUnmanagedStruct
        {
            packet.WriteByte(0); //unkByte1
            foreach (TStruct structure in list)
            {
                packet.WriteByte(1);
                packet.WriteStruct(structure);
            }
            packet.WriteByte(2);
        }
    }
}