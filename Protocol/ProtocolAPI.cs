using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTR_Server.Pages;
using BTR_Server.Data;

namespace BTR_Server.Protocol
{
    public class ProtocolAPI
    {
        private const int Port = 12786;
        private const string StabServer = "192.168.1.3";
        private const string UtilityServer = "192.168.1.2";
        private const string PowerServer = "192.168.1.200";

        private List<float> StabFloats = new List<float>();
        private byte[] ResponcePacket = new byte[255];
        private byte[] StabPacket = new byte [32];
        private byte[] PowerPacket = new byte[255];
        private byte[] UtilityPacket = new byte[255];

        private byte[] StuffedPacket = new byte[255];

        Crc16xmodem crc = new Crc16xmodem();


        public static MyTCPClient StabClient;
        public static MyTCPClient UtilityClient;
        public static MyTCPClient PowerClient;
        public ProtocolAPI()
        {
            if (StabClient == null)
            {
                //StabClient = new MyTCPClient(StabServer, Port);
            }
            if (UtilityClient == null)
            {
                UtilityClient = new MyTCPClient(UtilityServer, Port);
            }
            if (PowerClient == null)
            {
                //PowerClient = new MyTCPClient(PowerServer, Port);
            }


        }
        
        public void UpdateAndSendPacket(Object DataInstance)
        {
            
            if (DataInstance is StabData)
            {
                StabData data = DataInstance as StabData;
                  //перенести  
                
                data.ServiceInfo_1 = 0;
                data.ServiceInfo_2 = 0;
                

                data.ServiceInfo_1 += data.MotorControlMode;
                data.ServiceInfo_1 += (data.ShotStatus != 0 ? data.ShotStatus : 10);
                data.ServiceInfo_1 += (data.ResetGyroscopesStatus != 0 ? data.ResetGyroscopesStatus : 100);
                data.ServiceInfo_1 += (data.ShootMode !=0 ? data.ShootMode : 1000);
                data.ServiceInfo_1 += (data.WeaponType !=0 ? data.WeaponType : 10000);
                data.ServiceInfo_1 += (data.ShotgunShotStatus != 0 ? data.ShotgunShotStatus : 100000);
                data.ServiceInfo_1 += (data.CameraViewMode != 0 ? data.CameraViewMode : 1000000);
                data.ServiceInfo_2 += (data.ResetMotorStatus != 0 ? data.ResetMotorStatus : 1);

                StabFloats.Add(data.EncoderVertData);
                StabFloats.Add(data.EncoderVertData);
                StabFloats.Add(data.TrackingVertData);
                StabFloats.Add(data.TrackingHorData);
                StabFloats.Add(data.CorrectionVertData);
                StabFloats.Add(data.CorrectionHorData);
                StabFloats.Add(data.ServiceInfo_1);
                StabFloats.Add(data.ServiceInfo_2);
                
                int i = 0;
                foreach(float f in StabFloats)
                {
                    byte[] bytes = BitConverter.GetBytes(f);
                    foreach(byte b in bytes)
                    {
                        StabPacket[i++] = b;
                        
                    }
                    
                }
                int dataLength = i;
                StabFloats.Clear();
                ResponcePacket = StabClient.TCPrequest(BuildPacket(0x01, 0x11, (byte)dataLength, 0x2b, StabPacket, out dataLength), dataLength, out int bytesCount);

            }
            else if (DataInstance is UtilityData )
            {
                UtilityData data = DataInstance as UtilityData;
                MyTCPClient StabClient = new MyTCPClient(UtilityServer, Port);

            }
            else if (DataInstance is PowerData)
            {
                PowerData data = DataInstance as PowerData;
                //MyTCPClient StabClient = new MyTCPClient(StabServer, Port);
            }
        }
        public  void UtilityDataProcesing(ref UtilityData dataInstance, byte CMD, byte eventAdr=0x00, byte _event = 0x00)
        {
            
            
            if(CMD == 0x1b)
            {
                int dataLength;
                float[] dataFloats = new float[12];
                int _bytes;
                ResponcePacket = UtilityClient.TCPrequest(BuildPacket(0x01, 0x11, 0, 0x1b, StabPacket, out dataLength), dataLength, out _bytes);
                if (_bytes > 7)//if(_bytes == 55) => 7 + 12*4
                {
                    dataFloats = ParsePacket(ResponcePacket, _bytes);
                    dataInstance.GPSx = dataFloats[0];
                    /*
                    dataInstance.GPSy = dataFloats[1];
                    dataInstance.Yg = dataFloats[2];
                    dataInstance.Pg = dataFloats[3];
                    dataInstance.Rg = dataFloats[4];
                    dataInstance.Eview = dataFloats[5];
                    dataInstance.Ev = dataFloats[6];
                    dataInstance.Eg = dataFloats[7];
                    dataInstance.Patm = dataFloats[8];
                    dataInstance.T = dataFloats[9];
                    dataInstance.Sw = dataFloats[10];
                    dataInstance.Dw = dataFloats[11];
                    */
                }                    
            }
            else if(CMD == 0x3b)
            {
                int dataLength;
                float[] dataFloats = new float[3];
                int _bytes;
                ResponcePacket = UtilityClient.TCPrequest(BuildPacket(0x01, 0x11, 0, CMD, StabPacket, out dataLength), dataLength, out _bytes);
                if (_bytes > 7)//if(_bytes == 19) => 7 + 3*4
                {
                    dataFloats = ParsePacket(ResponcePacket, _bytes);
                    dataInstance.Length_1 = dataFloats[0];
                    /*
                    dataInstance.Length_1 = dataFloats[1];
                    dataInstance.Length_1 = dataFloats[2];
                    */
                }
            }
            else if(CMD == 0x4b)
            {
                byte[] _dataBytes = {eventAdr, _event };
                UtilityClient.TCPrequest(BuildPacket(0x01, 0x11, 2, CMD, _dataBytes, out int dataLength), dataLength, out int _bytes);
            }
            else if (CMD == 0x5b)
            {
                byte[] _dataBytes = { eventAdr, _event };
                UtilityClient.TCPrequest(BuildPacket(0x01, 0x11, 2, CMD, _dataBytes, out int dataLength), dataLength, out int _bytes);
            }


        }

        private float[] ParsePacket(byte[] _data, int _packetLength)
        {
            float[] _floats = new float[(_packetLength - 7) / 4];
            //byte[] crcBytes = crc.MakeCRC16(StuffedPacket, stuffLength);
            //StuffedPacket[stuffLength] = crcBytes[0];
            //StuffedPacket[stuffLength + 1] = crcBytes[1];

            for(int i = 0; i < (_packetLength - 7) / 4; i++)
            {
                    _floats[i] = BitConverter.ToSingle(_data, 4 * i + 5);
            }

            return _floats;
        }

        private byte[] BuildPacket(byte Dst, byte Src, byte Len, byte CMD, byte[] data, out int length)
        {
            byte[] DataToSend = new byte[255];
            DataToSend[0] = 0x7e;
            DataToSend[1] = Dst;
            DataToSend[2] = Src;
            DataToSend[3] = Len;
            DataToSend[4] = CMD;
            for (int i = 5; i < Len + 5; i++)
            {
                DataToSend[i] = data[i-5];
            }          
            int stuffLength = ByteStaffingBuild(DataToSend, Len + 5);
            byte[] crcBytes = crc.MakeCRC16(StuffedPacket, stuffLength);
            StuffedPacket[stuffLength] = crcBytes[0];
            StuffedPacket[stuffLength +1] = crcBytes[1];
            length = stuffLength + 2;
            return StuffedPacket;
        } 



        
        private int ByteStaffingBuild(byte[] _data , int length)
        {
            int j = 1;
            StuffedPacket[0] = 0x7e;
            for (int i = 1; i < length; i++)
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
