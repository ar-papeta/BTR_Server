using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTR_Server.Data
{
    public class StabData
    {
		public float EncoderVertData { get; set; }
		public float EncoderHorData { get; set; }
		public float CorrectionVertData { get; set; }
		public float CorrectionHorData { get; set; }
		public float TrackingVertData { get; set; }
		public float TrackingHorData { get; set; }
		public float ServiceInfo_1 { get; set; }
		public float ServiceInfo_2 { get; set; }

		/*Service info 1*/
		//1-Manual; 2-Stabilization; 3-Tracking;
		public float MotorControlMode { get; set; }
		//1-NO; 2-YES;
		public float ShotStatus { get; set; }

		//1-NO; 2-YES;
		public float ResetGyroscopesStatus { get; set; }

		//1-single; 2-burst; 3-semi-burst;
		public float ShootMode { get; set; }

		//1-gun; 2-grenade launcher;
		public float WeaponType { get; set; }

		//1-NO; 2-YES;
		public float ShotgunShotStatus { get; set; }

		//1-wide; 2-narrow; 3-thermalvisoz;
		public float CameraViewMode { get; set; }

		/*Service info 2*/
		//1-NO; 2-YES;
		public float ResetMotorStatus { get; set; }
	}
}
