using System.Collections;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class MainController : MonoBehaviour
    {
        public SteamVR_Action_Vector2 actionSteering = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("buggy", "Steering");
        
        public SteamVR_Action_Single actionThrottle = SteamVR_Input.GetAction<SteamVR_Action_Single>("buggy", "Throttle");
        
        public SteamVR_Action_Boolean actionBrake = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Brake");
        
        public SteamVR_Action_Boolean actionReset = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("buggy", "Reset");

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            SteamVR.InitializeStandalone(EVRApplicationType.VRApplication_Overlay);
        
            while (SteamVR.initializedState == SteamVR.InitializedStates.None || SteamVR.initializedState == SteamVR.InitializedStates.Initializing)
                yield return null;
        }

        // Update is called once per frame
        void Update()
        {
            if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
                return;
        
            var hand = SteamVR_Input_Sources.Any;
            var steer = actionSteering.GetAxis(hand);
            var throttle = actionThrottle.GetAxis(hand);
            var bBrake = actionBrake.GetState(hand);
            var bReset = actionReset.GetState(hand);
            var brake = bBrake ? 1 : 0;
//            var reset = actionReset.GetStateDown(hand);

            Debug.Log(
                $"steer Brake:{brake.ToString()} " + 
                $"Reset:{bReset.ToString()} " + 
                $"Steer X:{steer.x.ToString()} Y:{steer.y.ToString()} " +
                $"Throttle X:{throttle.ToString()}");
        }
    }
}
