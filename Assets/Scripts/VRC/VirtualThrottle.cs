using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class VirtualThrottle: MonoBehaviour
    {
        [Range(0f, 1f)]
        public float throttleDeadzoneDegrees;
        
        public SteamVR_Behaviour_Pose hand;
        
        private Vector3 zeroPoint;

        private bool localGripped;

        public bool gripped
        {
            set
            {
                if (value)
                {
                    zeroPoint = hand.transform.position;
                }
                else
                {
                    output.SetAxisX(0f, -1f, 1f);
                    output.SetAxisY(0f, -1f, 1f);
                    output.SetAxisZ(0f, -1f, 1f);
                }
                localGripped = value;
            }
        }
        public VJoyInterface output;

        private void Update()
        {
            if (!localGripped) return;

            var relPos = zeroPoint - hand.transform.position;

            // apply dead zone
            if (Mathf.Abs(relPos.x) < throttleDeadzoneDegrees) relPos.x = 0f;
            if (Mathf.Abs(relPos.y) < throttleDeadzoneDegrees) relPos.y = 0f;
            if (Mathf.Abs(relPos.z) < throttleDeadzoneDegrees) relPos.z = 0f;
            
            output.SetAxisX(relPos.x, -0.2f, 0.2f);
            output.SetAxisY(relPos.y, -0.2f, 0.2f);
            output.SetAxisZ(relPos.z, -0.2f, 0.2f);
        }
    }
}