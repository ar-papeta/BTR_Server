using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTR_Server.Data;

namespace BTR_Server.Protocol
{
    public class ProtocolAPI
    {
        private const int Port = 12786;
        private const string StabServer = "192.168.1.202";
        private const string UtilityServer = "192.168.1.201";
        private const string PowerServer = "192.168.1.200";

        private List<float> StabFloats = new List<float>();
        private byte[] ResponcePacket = new byte[255];
        private byte[] StabPacket = new byte [255];
        private byte[] PowerPacket = new byte[255];
        private byte[] UtilityPacket = new byte[255];

        private byte[] StuffedPacket = new byte[255];

        Crc16xmodem crc = new Crc16xmodem();
        



        public void UpdateAndSendPacket(Object DataInstance)
        {
            
            if (DataInstance is StabData)
            {
                MyTCPClient StabClient = new MyTCPClient(StabServer, Port);
                StabData data = DataInstance as StabData;
                data.ServiceInfo_1 = 0;
                data.ServiceInfo_2 = 0;
                
                StabFloats.Add(data.EncoderVertData);
                StabFloats.Add(data.EncoderVertData);
                StabFloats.Add(data.TrackingVertData);
                StabFloats.Add(data.TrackingHorData);
                StabFloats.Add(data.CorrectionVertData);
                StabFloats.Add(data.CorrectionHorData);

                data.ServiceInfo_1 += data.MotorControlMode;
                data.ServiceInfo_1 += data.ShotStatus;
                data.ServiceInfo_1 += data.ResetGyroscopesStatus;
                data.ServiceInfo_1 += data.ShootMode;
                data.ServiceInfo_1 += data.WeaponType;
                data.ServiceInfo_1 += data.ShotgunShotStatus;
                data.ServiceInfo_1 += data.CameraViewMode;
                data.ServiceInfo_2 += data.ResetMotorStatus;

                StabFloats.Add(data.ServiceInfo_1);
                StabFloats.Add(data.ServiceInfo_2);
                int i = 0;
                foreach(float f in StabFloats)
                {
                    byte[] bytes = BitConverter.GetBytes(f);
                    foreach(byte b in bytes)
                    {
                        StabPacket[i++] = b;
                        Console.WriteLine(StabPacket[i - 1]);
                    }
                    
                }
                int dataLength = 0;
                StabClient.TCPrequest(BuildPacket(0x01, 0x11, (byte)i, 0x2b, StabPacket, out dataLength), dataLength, out ResponcePacket);

            }
            else if (DataInstance is UtilityData )
            {
                UtilityData data = DataInstance as UtilityData;

            }
            else if (DataInstance is PowerData)
            {
                PowerData data = DataInstance as PowerData;
            }
        }

        private byte[] BuildPacket(byte Dst, byte Src, byte Len, byte CMD, byte[] data, out int length)
        {
            byte[] DataToSend = new byte[Len + 7];
            DataToSend[0] = 0x7e;
            DataToSend[1] = Dst;
            DataToSend[2] = Src;
            DataToSend[3] = Len;
            DataToSend[4] = CMD;
            length = Len + 7;
            for (int i = 5; i < Len + 5; i++)
            {
                DataToSend[i] = data[i];
            }
            return DataToSend;
        } 
        
        private int ByteStaffingBuild(byte[] _data , int length)
        {
            int j = 0;
            for (int i = 0; i < length; i++)
            {
                if (_data[i] == 0x7e)
                {
                    StuffedPacket[j++] = 0x7d;
                    StuffedPacket[j++] = 0x5e;
                }
                else if (_data[i] == 0x7d)
                {
                    StuffedPacket[j++] = 0x7d;
                    StuffedPacket[j++] = 0x5d;
                }
                else
                {
                    StuffedPacket[j++] = _data[i];
                }
                
            }
            return j;
        }
    }
}
