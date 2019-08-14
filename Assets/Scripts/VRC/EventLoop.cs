using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class EventLoop : MonoBehaviour
    {
        public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];
        public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

        private void Update()
        {
            var vr = OpenVR.System;
            if (vr == null) return;

            var ev = new VREvent_t();
            var size = (uint) Marshal.SizeOf(typeof(VREvent_t));
            for (var limit = 64; limit > 0; --limit)
            {
                if (!vr.PollNextEvent(ref ev, size)) break;
                var type = (EVREventType) ev.eventType;
                SteamVR_Events.System(type).Send(ev);

                if (type != EVREventType.VREvent_Quit) continue;
                enabled = false;
                break;
            }
        }

        private void LateUpdate()
        {
            var compositor = OpenVR.Compositor;
            if (compositor == null) return;

            compositor.GetLastPoses(poses, gamePoses);
            SteamVR_Events.NewPoses.Send(poses);
            SteamVR_Events.NewPosesApplied.Send();
        }
    }
}