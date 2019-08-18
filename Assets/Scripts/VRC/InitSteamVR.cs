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

            while (SteamVR.initializedState == SteamVR.InitializedStates.None || SteamVR.initializedState == SteamVR.InitializedStates.Initializing)
                yield return null;
            
        }

    }
}
