using System.Text;
using UnityEditor;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class OverlayController : MonoBehaviour
    {
        private static string vrDriver => OpenVR.System == null ? 
            "No Driver" : 
            GetStringTrackedDeviceProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String);

        private static string vrDisplay => OpenVR.System == null ? 
            "No Display" : 
            GetStringTrackedDeviceProperty(ETrackedDeviceProperty.Prop_SerialNumber_String);

        private void OnEnable()
        {
            Init();
            SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);
        }

        private void OnDisable()
        {
            Shutdown();
            SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);
        }

        private void OnQuit(VREvent_t ev)
        {
            enabled = false;

            Debug.Log("OpenVR Quit event received, quitting");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }

        private void Init()
        {
            if (!ConnectToVrRuntime())
            {
                enabled = false;
                return;
            }

            Debug.Log("Connected to VR Runtime");
            Debug.Log(
                "VR Driver: " + vrDriver + "\n" +
                "VR Display: " + vrDisplay
            );
            
            Utils.IdentifyActionsFile();
        }

        private static void Shutdown()
        {
            DisconnectFromVrRuntime();
        }

        private static bool ConnectToVrRuntime()
        {
            var error = EVRInitError.None;
            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);
            if (error != EVRInitError.None)
            {
                Debug.LogWarning(error);
                return false;
            }

            SteamVR_Events.Initialized.Send(true);

            return true;
        }

        private static void DisconnectFromVrRuntime()
        {
            SteamVR_Events.Initialized.Send(false);
            OpenVR.Shutdown();
        }

        private static string GetStringTrackedDeviceProperty(ETrackedDeviceProperty prop,
            uint deviceId = OpenVR.k_unTrackedDeviceIndex_Hmd)
        {
            var error = ETrackedPropertyError.TrackedProp_Success;
            // @todo Use a per-thread shared string builder
            var result = new StringBuilder((int) OpenVR.k_unMaxPropertyStringSize);
            var capacity = OpenVR.System.GetStringTrackedDeviceProperty(deviceId, prop, result,
                OpenVR.k_unMaxPropertyStringSize, ref error);
            if (error == ETrackedPropertyError.TrackedProp_Success)
                return result.ToString(0, (int) capacity - 1);
            return "Error Getting String: " + error;
        }
    }
}