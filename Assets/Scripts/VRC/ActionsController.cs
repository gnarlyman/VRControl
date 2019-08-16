using System;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    using Events = SteamVR_Events;
    public class ActionsController : MonoBehaviour
    {
        private enum Hand
        {
            Unknown,
            Left,
            Right,
        }
        
        private void OnEnable()
        {
            Events.System(EVREventType.VREvent_ButtonPress).Listen(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonUnpress).Listen(OnButtonUnpress);
            Events.System(EVREventType.VREvent_ButtonTouch).Listen(OnButtonTouch);
            Events.System(EVREventType.VREvent_ButtonUntouch).Listen(OnButtonUntouch);
        }
        
        private void OnDisable()
        {
            Events.System(EVREventType.VREvent_ButtonPress).Remove(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonUnpress).Remove(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonTouch).Remove(OnButtonTouch);
            Events.System(EVREventType.VREvent_ButtonUntouch).Remove(OnButtonUntouch);
        }

        private void OnButtonUntouch(VREvent_t ev)
        {
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;
            Debug.Log($"OnButtonUntouch {hand.ToString()} {button.ToString()}");
        }

        private void OnButtonTouch(VREvent_t ev)
        {
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;
            Debug.Log($"OnButtonTouch {hand.ToString()} {button.ToString()}");
        }

        private void OnButtonUnpress(VREvent_t ev)
        {
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;
            Debug.Log($"OnButtonUnpress {hand.ToString()} {button.ToString()}");
        }

        private void OnButtonPress(VREvent_t ev)
        {
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;
            Debug.Log($"OnButtonPress {hand.ToString()} {button.ToString()}");
        }

        private static Hand GetHandForDevice(uint deviceIndex)
        {
            var role = OpenVR.System.GetControllerRoleForTrackedDeviceIndex(deviceIndex);
            switch (role)
            {
                case ETrackedControllerRole.LeftHand: return Hand.Left;
                case ETrackedControllerRole.RightHand: return Hand.Right;
                default: return Hand.Unknown;
            }
        }

    }

}
