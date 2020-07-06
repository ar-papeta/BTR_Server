using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTR_Server.Pages;
using SharpDX.DirectInput;

namespace BTR_Server.SpeedLinkJoystick
{
    public class JoystickAPI
    {

        private static DirectInput directInput = new DirectInput();
        private static Guid joystickGuid = Guid.Empty;
        private static Joystick joystick;
        public void JoystickInit()
        {
            directInput = new DirectInput();
            joystickGuid = Guid.Empty;
            foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Gamepad,
                DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
                        DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("No joystick/Gamepad found.");
            }

            // Instantiate the joystick
            joystick = new Joystick(directInput, joystickGuid);

            

            
            

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 256;

            // Acquire the joystick
            joystick.Acquire();
            TimerCallback tm = new TimerCallback(Timer_tick);
            
            Timer timer = new Timer(tm, null, 0, 50);
            

        }
        float coef = 1;
        float prevX = 0;
        float prevY = 0;
        public void Timer_tick(object obj)
        {
            
            try
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();


                foreach (var state in datas) 
                {
                    //speed coef
                    if (Convert.ToString(state.Offset) == "Z")
                    {
                        coef = map((65535 - state.Value), 0, 65535, 0, 5);
                        
                    }

                    // left/right
                    else if (Convert.ToString(state.Offset) == "Y")
                    {
                        Stabilization.StabData.EncoderVertData = (int)(map((65535 - state.Value), 0, 65535, -140, 140));
                        prevY = Stabilization.StabData.EncoderVertData;
                    }

                    //Up/down
                    else if (Convert.ToString(state.Offset) == "X")
                    {
                        Stabilization.StabData.EncoderHorData = (int)(map((65535 - state.Value), 0, 65535, -70, 70));
                        prevX = Stabilization.StabData.EncoderHorData;
                    }

                    
                    
                    
                }
                Stabilization.StabData.EncoderHorData *= (int)(coef);
                Stabilization.StabData.EncoderVertData *= (int)(coef);
                Console.WriteLine(Stabilization.StabData.EncoderHorData);
                Console.WriteLine(Stabilization.StabData.EncoderVertData);
                Stabilization.protocol.UpdateAndSendPacket(Stabilization.StabData);
                Stabilization.StabData.EncoderHorData  = prevX;
                Stabilization.StabData.EncoderVertData = prevY;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

        }

        private float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }


    }
}
