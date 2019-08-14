using UnityEngine;

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
    }
}