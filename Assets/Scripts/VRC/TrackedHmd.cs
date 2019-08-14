using JetBrains.Annotations;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class TrackedHmd : MonoBehaviour
    {
        private static TrackedHmd privinst;

        private readonly SteamVR_Events.Action newPosesAction;

        private TrackedHmd()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        private static TrackedHmd instance => Utils.Singleton(ref privinst, "[HMD]");

        [UsedImplicitly] 
        public static Transform trans => instance.transform;

        private void OnEnable()
        {
            newPosesAction.enabled = true;
        }

        private void OnDisable()
        {
            newPosesAction.enabled = false;

            if (privinst == this) privinst = null;
        }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            var deviceIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;

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
            Gizmos.color = Color.black;
            var aspectRatio = 16f / 9f;
            Gizmos.DrawFrustum(Vector3.zero, 110f / aspectRatio, 1f, .05f, aspectRatio);
        }
    }
}