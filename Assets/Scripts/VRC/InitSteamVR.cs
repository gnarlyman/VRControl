using System.Collections;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public class InitSteamVR : MonoBehaviour
    {
        
        // Start is called before the first frame update
        private IEnumerator Start()
        {
            SteamVR.InitializeStandalone(EVRApplicationType.VRApplication_Overlay);
            SteamVR_Settings.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;

            while (SteamVR.initializedState == SteamVR.InitializedStates.None || SteamVR.initializedState == SteamVR.InitializedStates.Initializing)
                yield return null;
            
            Debug.Log("tracking space: " + SteamVR_Settings.instance.trackingSpace);
        }

    }
}
