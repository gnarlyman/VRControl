using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class TrackedHand : MonoBehaviour
    {
        public enum Hand
        {
            Left,
            Right
        }

        private uint deviceIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

        public Hand hand = Hand.Left;

        private readonly SteamVR_Events.Action newPosesAction;

        private TrackedHand()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        private ETrackedControllerRole controllerRole =>
            hand == Hand.Right
                ? ETrackedControllerRole.RightHand
                : ETrackedControllerRole.LeftHand;

        private bool hasValidDevice => deviceIndex != OpenVR.k_unTrackedDeviceIndexInvalid;

        private void OnEnable()
        {
            SteamVR_Events.Initialized.Listen(OnRuntimeInitialized);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceActivated).Listen(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceDeactivated).Listen(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceUpdated).Listen(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Listen(OnTrackedDeviceEvent);
            newPosesAction.enabled = true;

            RescanDevices();
        }

        private void OnDisable()
        {
            SteamVR_Events.Initialized.Remove(OnRuntimeInitialized);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceActivated).Remove(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceDeactivated).Remove(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceUpdated).Remove(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Remove(OnTrackedDeviceEvent);
            newPosesAction.enabled = false;
        }

        private void OnRuntimeInitialized(bool initialized)
        {
            if (initialized) RescanDevices();
        }

        private void OnTrackedDeviceEvent(VREvent_t ev)
        {
            RescanDevices();
        }

        // Scan the tracked device list to find the left or right hand
        private void RescanDevices()
        {
            deviceIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

            var vr = OpenVR.System;
            if (vr == null) return;

            deviceIndex = vr.GetTrackedDeviceIndexForControllerRole(controllerRole);
        }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            if (!hasValidDevice) return;

            if (poses.Length < deviceIndex) return;

            if (!poses[deviceIndex].bDeviceIsConnected) return;

            if (!poses[deviceIndex].bPoseIsValid) return;

            var pose = new SteamVR_Utils.RigidTransform(poses[deviceIndex].mDeviceToAbsoluteTracking);
            // When the application is using a seated universe convert it to a standing universe transform
            if (OpenVR.Compositor.GetTrackingSpace() == ETrackingUniverseOrigin.TrackingUniverseSeated)
            {
                var seatedTransformMatrix = OpenVR.System.GetSeatedZeroPoseToStandingAbsoluteTrackingPose();
                var seatedTransform = new SteamVR_Utils.RigidTransform(seatedTransformMatrix);
                pose = seatedTransform * pose;
            }

            var t = transform;
            
            t.localPosition = pose.pos;
            t.localRotation = pose.rot;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Vector3.zero, 0.01f);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.05f);
        }
    }
}