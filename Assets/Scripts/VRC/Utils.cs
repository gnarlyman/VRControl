using System.IO;
using UnityEngine;
using Valve.VR;

namespace VRC
{
    public static class Utils
    {
        public static T Singleton<T>(ref T inst, string name, bool create = true) where T : MonoBehaviour
        {
            if (inst != null) return inst;
            inst = Object.FindObjectOfType<T>();

            if (inst != null) return inst;
            if (create)
            {
                var hmd = new GameObject(name);
                inst = hmd.AddComponent<T>();
            }
            else
            {
                Debug.LogErrorFormat("Instance for {0} ({1}) does not exist in the scene", name,
                    typeof(T).Name);
                return null;
            }

            return inst;
        }

        public static void IdentifyActionsFile(bool showLogs = true)
        {
            var currentPath = Application.dataPath;
            var lastIndex = currentPath.LastIndexOf('/');
            currentPath = currentPath.Remove(lastIndex, currentPath.Length - lastIndex);

            var fullPath = Path.Combine(currentPath, SteamVR_Settings.instance.actionsFilePath);
            fullPath = fullPath.Replace("\\", "/");

            if (File.Exists(fullPath))
            {
                if (OpenVR.Input == null)
                {
                    Debug.LogError("<b>[SteamVR]</b> Could not instantiate OpenVR Input interface.");
                    return;
                }

                var err = OpenVR.Input.SetActionManifestPath(fullPath);
                if (err != EVRInputError.None)
                {
                    Debug.LogError("<b>[SteamVR]</b> Error loading action manifest into SteamVR: " + err);
                }
                else
                {
                    if (SteamVR_Input.actions != null)
                    {
                        var numActions = SteamVR_Input.actions.Length;

                        if (showLogs)
                            Debug.Log(
                                $"<b>[SteamVR]</b> Successfully loaded {numActions} actions from action manifest into SteamVR ({fullPath})");
                    }
                    else
                    {
                        if (showLogs)
                            Debug.LogWarning(
                                "<b>[SteamVR]</b> No actions found, but the action manifest was loaded. This usually means you haven't generated actions. Window -> SteamVR Input -> Save and Generate.");
                    }
                }
            }
            else
            {
                if (showLogs)
                    Debug.LogError("<b>[SteamVR]</b> Could not find actions file at: " + fullPath);
            }
        }
    }
}