using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class VirtualJoystick: MonoBehaviour
    {
        [Range(0f, 90f)]
        public float joystickDeadzoneDegrees;
        
        public SteamVR_Behaviour_Pose hand;
        
        private Transform zeroPoint;
        private Transform rotationPoint;
        
        private bool localGripped;

        public bool gripped
        {
            set
            {
                if (value)
                {
                    var rot = hand.transform.rotation;
                    zeroPoint.rotation = rot;
                    rotationPoint.rotation = rot;
                }
                else
                {
                    output.SetAxisXRot(0f, -1f, 1f);
                    output.SetAxisYRot(0f, -1f, 1f);
                    output.SetAxisZRot(0f, -1f, 1f);
                }
                localGripped = value;
            }
        }
        public VJoyInterface output;

        private void Start()
        {
            var zeroPointObject = new GameObject("[ZeroPoint-Joystick]");
            zeroPoint = zeroPointObject.transform;
            zeroPoint.SetParent(transform);
            zeroPoint.localPosition = Vector3.zero;
            zeroPoint.localRotation = Quaternion.identity;

            var rotationPointObject = new GameObject("[RotationPoint-Joystick]");
            rotationPoint = rotationPointObject.transform;
            rotationPoint.SetParent(zeroPoint);
            rotationPoint.localPosition = Vector3.zero;
            rotationPoint.localRotation = Quaternion.identity;
        }
        
        private void Update()
        {
            if (!localGripped) return;
            
            rotationPoint.rotation = hand.transform.rotation;
            var angles = rotationPoint.localEulerAngles;
            
            // center on 0
            if (angles.x > 180f) angles.x -= 360f;
            if (angles.y > 180f) angles.y -= 360f;
            if (angles.z > 180f) angles.z -= 360f;
            
            // apply dead zone
            if (Mathf.Abs(angles.x) < joystickDeadzoneDegrees) angles.x = 0f;
            if (Mathf.Abs(angles.y) < joystickDeadzoneDegrees) angles.y = 0f;
            if (Mathf.Abs(angles.z) < joystickDeadzoneDegrees) angles.z = 0f;
            
//            Debug.Log($"Joystick Gripped:{localGripped}, Rot:{rotationPoint.localEulerAngles} Normal:{angles}");
            output.SetAxisXRot(angles.x, -15f, 15f);
            output.SetAxisYRot(angles.y, -15f, 15f);
            output.SetAxisZRot(angles.z, -15f, 15f);
        }
    }
}