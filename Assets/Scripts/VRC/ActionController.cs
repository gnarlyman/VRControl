using System.Collections;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class ActionController : MonoBehaviour
    {
        public SteamVR_Action_Single forwardThrust = 
            SteamVR_Input.GetAction<SteamVR_Action_Single>("vrcontrol", "ForwardThrust");
    
        public SteamVR_Action_Single backwardThrust =
            SteamVR_Input.GetAction<SteamVR_Action_Single>("vrcontrol", "BackwardThrust");

        public SteamVR_Action_Vector2 hvThrust =
            SteamVR_Input.GetAction<SteamVR_Action_Vector2>("vrcontrol", "HVThrust");

        public SteamVR_Action_Vector2 yawRotation =
            SteamVR_Input.GetAction<SteamVR_Action_Vector2>("vrcontrol", "Yaw");

        public SteamVR_Action_Boolean boost =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "Boost");
    
        public VJoyInterface output;
        
        private void ActionBoostChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, bool activeBinding)
        {
            Debug.Log($"Boost inputSource:{booleanAction.activeDevice} {activeBinding}");
            output.SetButton((uint)ActionMappings.Boost, activeBinding);
        }

        private void ActionForwardThrustChange(ISteamVR_Action_In_Source singleAction,
            SteamVR_Input_Sources inputSource, float axis, float delta)
        {
            Debug.Log($"Forward Thrust inputSource:{singleAction.activeDevice} {axis}");
            output.SetZAxis(axis);
        }
    
        // Start is called before the first frame update
        private void Start()
        {
            boost.AddOnChangeListener(ActionBoostChange, SteamVR_Input_Sources.LeftHand);
            forwardThrust.AddOnChangeListener(ActionForwardThrustChange, SteamVR_Input_Sources.LeftHand);
        }
    }
}
