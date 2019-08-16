using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class TrackedDevice : MonoBehaviour
    {
        public enum EIndex
        {
            None = -1,
            Hmd = (int)OpenVR.k_unTrackedDeviceIndex_Hmd,
            Device1,
            Device2,
            Device3,
            Device4,
            Device5,
            Device6,
            Device7,
            Device8,
            Device9,
            Device10,
            Device11,
            Device12,
            Device13,
            Device14,
            Device15
        }
    
        public enum EType
        {
            None = -1,
            Hmd,
            LeftController,
            RightController
        }
    
        public EType Type;
        public EIndex Index;
        public Transform Origin; // if not set, relative to parent
        public bool IsValid;
    }
}
