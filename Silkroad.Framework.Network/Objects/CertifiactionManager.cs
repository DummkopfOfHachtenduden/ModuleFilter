using Silkroad.Framework.Common.Objects.Certification;
using Silkroad.Framework.Common.Objects.SecurityDesc;
using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Silkroad.Framework.Common.Objects
{
    public class CertifiactionManager
    {
        //TODO: THREAD SAFTY!!!

        private Service _service;

        public List<srServiceType> ServiceTypes { get; private set; }
        public List<srOperationType> OperationTypes { get; private set; }
        public List<srGlobalService> GlobalServices { get; private set; }
        public List<srGlobalOperation> GlobalOperations { get; private set; }
        public List<srUnknown> Unknown { get; private set; }
        public List<srShard> Shards { get; private set; }
        public List<srNodeType> NodeTypes { get; private set; }
        public List<srNodeData> NodeData { get; private set; }
        public List<srNodeLink> NodeLinks { get; private set; }

        public List<SecurityDescriptionGroup> SecurityDescriptionGroups { get; private set; }
        public List<SecurityDescription> SecurityDescriptions { get; private set; }
        public List<SecurityDescriptionGroupAssign> SecurityDescriptionGroupAssigns { get; private set; }

        public CertifiactionManager(Service service)
        {
            _service = service;

            ServiceTypes = new List<srServiceType>();
            OperationTypes = new List<srOperationType>();
            GlobalServices = new List<srGlobalService>();
            GlobalOperations = new List<srGlobalOperation>();
            Unknown = new List<srUnknown>();
            Shards = new List<srShard>();
            NodeTypes = new List<srNodeType>();
            NodeData = new List<srNodeData>();
            NodeLinks = new List<srNodeLink>();

            SecurityDescriptionGroups = new List<SecurityDescriptionGroup>();
            SecurityDescriptions = new List<SecurityDescription>();
            SecurityDescriptionGroupAssigns = new List<SecurityDescriptionGroupAssign>();
        }

        public void Read(Packet packet)
        {
            var hasCertification = packet.ReadBool();
            if (hasCertification)
            {
                this.ReadList<srServiceType>(packet, ServiceTypes);
                this.ReadList<srOperationType>(packet, OperationTypes);
                this.ReadList<srGlobalService>(packet, GlobalServices);
                this.ReadList<srGlobalOperation>(packet, GlobalOperations);
                this.ReadList<srUnknown>(packet, Unknown);
                this.ReadList<srShard>(packet, Shards);
                this.ReadList<srNodeType>(packet, NodeTypes);
                this.ReadList<srNodeData>(packet, NodeData);
                this.ReadList<srNodeLink>(packet, NodeLinks);
            }

            var hasSecurityDescription = packet.ReadBool();
            if (hasSecurityDescription)
            {
                this.ReadList<SecurityDescriptionGroup>(packet, SecurityDescriptionGroups);
                this.ReadList<SecurityDescription>(packet, SecurityDescriptions);
                this.ReadList<SecurityDescriptionGroupAssign>(packet, SecurityDescriptionGroupAssigns);
            }
        }

        public void ReadList<T>(Packet packet, IList<T> list) where T : struct
        {
            list.Clear();

            var unkByte0 = packet.ReadByte();
            while (true)
            {
                var entryFlag = packet.ReadByte();
                if (entryFlag == 1)
                {
                    list.Add(packet.ReadStruct<T>());
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

        public void Write(Packet packet, bool writeCertification, bool writeSecurityDesc)
        {
            packet.WriteBool(writeCertification);
            if (writeCertification)
            {
                this.WriteList<srServiceType>(packet, ServiceTypes);
                this.WriteList<srOperationType>(packet, OperationTypes);
                this.WriteList<srGlobalService>(packet, GlobalServices);
                this.WriteList<srGlobalOperation>(packet, GlobalOperations);
                this.WriteList<srUnknown>(packet, Unknown);
                this.WriteList<srShard>(packet, Shards);
                this.WriteList<srNodeType>(packet, NodeTypes);
                this.WriteList<srNodeData>(packet, NodeData);
                this.WriteList<srNodeLink>(packet, NodeLinks);
            }

            packet.WriteBool(writeSecurityDesc);
            if (writeSecurityDesc)
            {
                this.WriteList<SecurityDescriptionGroup>(packet, SecurityDescriptionGroups);
                this.WriteList<SecurityDescription>(packet, SecurityDescriptions);
                this.WriteList<SecurityDescriptionGroupAssign>(packet, SecurityDescriptionGroupAssigns);
            }
        }

        public void WriteList<T>(Packet packet, IList<T> list) where T : struct
        {
            packet.WriteByte(0); //unkByte1
            foreach (T structure in list)
            {
                packet.WriteByte(1);
                packet.WriteStruct(structure);
            }
            packet.WriteByte(2);
        }

        public void ReadOld(Packet packet)
        {
            //bool -> read certification?
            var unkByte0 = packet.ReadByte();

            ServiceTypes.Clear();
            OperationTypes.Clear();
            GlobalServices.Clear();
            GlobalOperations.Clear();
            Unknown.Clear();
            Shards.Clear();
            NodeTypes.Clear();
            NodeData.Clear();
            NodeLinks.Clear();

            //ServiceTypes
            var unkByte1 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                ServiceTypes.Add(packet.ReadStruct<srServiceType>());
            }

            //OperationTypes
            var unkByte2 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                OperationTypes.Add(packet.ReadStruct<srOperationType>());
            }

            //GlobalService
            var unkByte3 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                GlobalServices.Add(packet.ReadStruct<srGlobalService>());
            }

            //GlobalOperation
            var unkByte4 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                GlobalOperations.Add(packet.ReadStruct<srGlobalOperation>());
            }

            //Unknown
            var unkByte5 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                Unknown.Add(packet.ReadStruct<srUnknown>());
            }

            //Shards
            var unkByte6 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                Shards.Add(packet.ReadStruct<srShard>());
            }

            //NodeTypes
            var unkByte7 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                var buffer = packet.ReadByteArray(Marshal.SizeOf(typeof(srNodeType)));
                var nodeType = Unmanaged.BufferToStruct<srNodeType>(buffer);
                NodeTypes.Add(nodeType);
            }

            //NodeData
            var unkByte8 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                NodeData.Add(packet.ReadStruct<srNodeData>());
            }

            //NodeLinks
            var unkByte9 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                NodeLinks.Add(packet.ReadStruct<srNodeLink>());
            }

            //bool read previliges?
            var unkByte10 = packet.ReadByte();

            SecurityDescriptionGroups.Clear();
            SecurityDescriptions.Clear();
            SecurityDescriptionGroupAssigns.Clear();

            //securityDescriptionGroups
            var unkByte11 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                SecurityDescriptionGroups.Add(packet.ReadStruct<SecurityDescriptionGroup>());
            }

            //securityDescriptions
            var unkByte12 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                SecurityDescriptions.Add(packet.ReadStruct<SecurityDescription>());
            }

            //securityDescriptionGroupAssigns
            var unkByte13 = packet.ReadByte();
            while (true)
            {
                var flag = packet.ReadByte();
                if (flag == 2)
                    break;

                SecurityDescriptionGroupAssigns.Add(packet.ReadStruct<SecurityDescriptionGroupAssign>());
            }
        }
    }
}