using UnityEngine;

namespace Classes
{
    public static class GameHelper
    {
        ///Sets the layer of every child to either the default or an invisible layer and set active state
        internal static void SetVisibility(Transform obj, bool visible)
        {
            SetVisibility(obj.gameObject, visible);
        }

        internal static void SetVisibility(GameObject obj, bool visible)
        {
            foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = visible ? 0 : 3;
            }

            obj.SetActive(visible);
        }
    }
}