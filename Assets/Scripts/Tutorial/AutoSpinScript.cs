using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Tutorial
{
    public class AutoSpinScript : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.Rotate(Vector3.up, -10*Time.fixedDeltaTime);
        }
    }
}
