using System;
using System.Collections;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class ActionController : MonoBehaviour
    {
        public SteamVR_Action_Single forwardThrust = 
            SteamVR_Input.GetAction<SteamVR_Action_Single>("vrcontrol", "ForwardThrust");
        
        public SteamVR_Action_Single gripLeft =
            SteamVR_Input.GetAction<SteamVR_Action_Single>("vrcontrol", "GripLeft");

        public SteamVR_Action_Single gripRight =
            SteamVR_Input.GetAction<SteamVR_Action_Single>("vrcontrol", "GripRight");
        
        public SteamVR_Action_Vector2 hvThrust =
            SteamVR_Input.GetAction<SteamVR_Action_Vector2>("vrcontrol", "HVThrust");
        
        public SteamVR_Action_Vector2 pov1Pos =
            SteamVR_Input.GetAction<SteamVR_Action_Vector2>("vrcontrol", "POV1_POS");
        
        public SteamVR_Action_Vector2 pov2Pos =
            SteamVR_Input.GetAction<SteamVR_Action_Vector2>("vrcontrol", "POV2_POS");
        
        public SteamVR_Action_Vector2 pitchRoll =
            SteamVR_Input.GetAction<SteamVR_Action_Vector2>("vrcontrol", "PitchRoll");

        public SteamVR_Action_Boolean boost =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "Boost");

        public SteamVR_Action_Boolean grabJoystick =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "GrabJoystick");
        
        public SteamVR_Action_Boolean pov1Touch =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "POV1_TOUCH");
        
        public SteamVR_Action_Boolean pov2Touch =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "POV2_TOUCH");
        
        public SteamVR_Action_Boolean uiEnter =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "UI_ENTER");

        public SteamVR_Action_Boolean uiBack =
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("vrcontrol", "UI_BACK");
        
        public VJoyInterface output;
        public VirtualJoystick joy;
        public VirtualThrottle throttle;

        private void ActionBoostChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, bool activeBinding)
        {
//            Debug.Log($"Boost inputSource:{booleanAction.activeDevice} {activeBinding}");
            output.SetButton((uint)ActionMappings.Boost, activeBinding);
        }

        private void ActionGrabJoystickChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, bool activeBinding)
        {
//            Debug.Log($"Grab Joystick inputSource:{booleanAction.activeDevice} {activeBinding}");
            joy.gripped = activeBinding;
        }
        
        private void ActionUiEnterChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, bool activeBinding)
        {
//            Debug.Log($"UI Enter inputSource:{booleanAction.activeDevice} {activeBinding}");
            output.SetButton((uint)ActionMappings.UiEnter, activeBinding);
        }
        
        private void ActionGripLeftChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, float axis, float delta)
        {
            if (axis > 0.8f)
            {
                throttle.gripped = true;
            } else if (axis < 0.2f)
            {
                throttle.gripped = false;
            }
        }
        
        private void ActionGripRightChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, float axis, float delta)
        {
            if (axis > 0.8f)
            {
                joy.gripped = true;
            } else if (axis < 0.2f)
            {
                joy.gripped = false;
            }
        }
        
        private void ActionUiBackChange(ISteamVR_Action_In_Source booleanAction, 
            SteamVR_Input_Sources inputSource, bool activeBinding)
        {
//            Debug.Log($"UI Back inputSource:{booleanAction.activeDevice} {activeBinding}");
            output.SetButton((uint)ActionMappings.UiBack, activeBinding);
        }

        private void ActionForwardThrustChange(ISteamVR_Action_In_Source singleAction,
            SteamVR_Input_Sources inputSource, float axis, float delta)
        {
//            Debug.Log($"Forward Thrust inputSource:{singleAction.activeDevice} {axis}");
            output.SetAxisZ(axis, 0f, 1f);
        }
        
        private void ActionHvThrustChange(ISteamVR_Action_In_Source vectorAction,
            SteamVR_Input_Sources inputSource, Vector2 axis, Vector2 delta)
        {
//            Debug.Log($"HV Thrust inputSource:{vectorAction.activeDevice} X:{axis.x} Y:{axis.y}");
            output.SetAxisY(axis.y, -1f, 1f);
            output.SetAxisX(axis.x, -1f, 1f);
        }
        
        private void ActionPitchRollChange(ISteamVR_Action_In_Source vectorAction,
            SteamVR_Input_Sources inputSource, Vector2 axis, Vector2 delta)
        {
            output.SetAxisXRot(axis.y, -1f, 1f);
            output.SetAxisYRot(axis.x, -1f, 1f);
        }
        
        private IEnumerator UnpressPov1Button(float time, ActionMappings button)
        {
            yield return new WaitForSeconds(time);

            currPov1Pos = Vector2.zero;
            lastPov1Pos = Vector2.zero;
            output.SetButton((uint)button, false);
        }
        
        private IEnumerator UnpressPov2Button(float time, ActionMappings button)
        {
            yield return new WaitForSeconds(time);

            currPov2Pos = Vector2.zero;
            lastPov2Pos = Vector2.zero;
            output.SetButton((uint)button, false);
        }

        private bool pov1Touched;
        private Vector2 lastPov1Pos;
        private Vector2 currPov1Pos;
        private bool pov2Touched;
        private Vector2 lastPov2Pos;
        private Vector2 currPov2Pos;

        private void UpdatePov1()
        {
            if (pov1Touch.GetState(SteamVR_Input_Sources.LeftHand))
            {
                currPov1Pos = pov1Pos.GetAxis(SteamVR_Input_Sources.LeftHand);
                if (pov1Touched) return;
                
                pov1Touched = true;
                lastPov1Pos = pov1Pos.GetAxis(SteamVR_Input_Sources.LeftHand);

            }
            else
            {
                pov1Touched = false;
                var dir = (currPov1Pos - lastPov1Pos).normalized;

                if (dir == Vector2.zero) return;
                
                if (Math.Abs(dir.x) > Math.Abs(dir.y))
                {
                    if (dir.x >= 0)
                    {
                        output.SetButton((uint)ActionMappings.Pov1Right, true);
                        StartCoroutine(UnpressPov1Button(0.1f, ActionMappings.Pov1Right));
                    }
                    else
                    {
                        output.SetButton((uint)ActionMappings.Pov1Left, true);
                        StartCoroutine(UnpressPov1Button(0.1f, ActionMappings.Pov1Left));
                    }
                }
                else
                {
                    if (dir.y >= 0)
                    {
                        output.SetButton((uint)ActionMappings.Pov1Up, true);      
                        StartCoroutine(UnpressPov1Button(0.1f, ActionMappings.Pov1Up));
                    }
                    else
                    {
                        output.SetButton((uint)ActionMappings.Pov1Down, true);
                        StartCoroutine(UnpressPov1Button(0.1f, ActionMappings.Pov1Down));
                    }
                }
                
            }
        }

        private void UpdatePov2()
        {
            if (pov2Touch.GetState(SteamVR_Input_Sources.RightHand))
            {
                currPov2Pos = pov2Pos.GetAxis(SteamVR_Input_Sources.RightHand);
                if (pov2Touched) return;
                
                pov2Touched = true;
                lastPov2Pos = pov2Pos.GetAxis(SteamVR_Input_Sources.RightHand);

            }
            else
            {
                pov2Touched = false;
                var dir = (currPov2Pos - lastPov2Pos).normalized;

                if (dir == Vector2.zero) return;
                
                if (Math.Abs(dir.x) > Math.Abs(dir.y))
                {
                    if (dir.x >= 0)
                    {
                        output.SetButton((uint)ActionMappings.Pov2Right, true);
                        StartCoroutine(UnpressPov2Button(0.1f, ActionMappings.Pov2Right));
                    }
                    else
                    {
                        output.SetButton((uint)ActionMappings.Pov2Left, true);
                        StartCoroutine(UnpressPov2Button(0.1f, ActionMappings.Pov2Left));
                    }
                }
                else
                {
                    if (dir.y >= 0)
                    {
                        output.SetButton((uint)ActionMappings.Pov2Up, true);      
                        StartCoroutine(UnpressPov2Button(0.1f, ActionMappings.Pov2Up));
                    }
                    else
                    {
                        output.SetButton((uint)ActionMappings.Pov2Down, true);
                        StartCoroutine(UnpressPov2Button(0.1f, ActionMappings.Pov2Down));
                    }
                }
                
            }
        }
        
        private void Update()
        {
            UpdatePov1();
            UpdatePov2();
        }

        // Start is called before the first frame update
        private void Start()
        {
            boost.AddOnChangeListener(ActionBoostChange, SteamVR_Input_Sources.LeftHand);
            forwardThrust.AddOnChangeListener(ActionForwardThrustChange, SteamVR_Input_Sources.LeftHand);
            hvThrust.AddOnChangeListener(ActionHvThrustChange, SteamVR_Input_Sources.LeftHand);
            grabJoystick.AddOnChangeListener(ActionGrabJoystickChange, SteamVR_Input_Sources.RightHand);
            uiEnter.AddOnChangeListener(ActionUiEnterChange, SteamVR_Input_Sources.RightHand);
            uiBack.AddOnChangeListener(ActionUiBackChange, SteamVR_Input_Sources.RightHand);
            pitchRoll.AddOnChangeListener(ActionPitchRollChange, SteamVR_Input_Sources.RightHand);
            gripLeft.AddOnChangeListener(ActionGripLeftChange, SteamVR_Input_Sources.LeftHand);
            gripRight.AddOnChangeListener(ActionGripRightChange, SteamVR_Input_Sources.RightHand);
        }
    }
}
