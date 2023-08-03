using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//From https://docs.unity3d.com/Manual/performance-garbage-collection-best-practices.html
public class ObjectPool : MonoBehaviour {

    
    public GameObject prefabToPool;
    public int maxPoolSize;
    private Stack<GameObject> inactiveObjects = new Stack<GameObject>();
    
    
    void Start() {
        if (prefabToPool != null) {
            for (int i = 0; i < maxPoolSize; ++i) {
                var newObj = Instantiate(prefabToPool);
                newObj.SetActive(false);
                inactiveObjects.Push(newObj);
            }
        }
    }

    public GameObject GetObjectFromPool() {
        while (inactiveObjects.Count > 0) {
            var obj = inactiveObjects.Pop();
          
            if (obj != null) {
                obj.SetActive(true); //TODO: think if this makes sense or should be done after everything else
                return obj;
            }
            else {
                Debug.LogWarning("Found a null object in the pool. Has some code outside the pool destroyed it?");
            }
        }
        
        Debug.LogError("All pooled objects are already in use or have been destroyed");
        return null;
    }
  
    public void ReturnObjectToPool(GameObject objectToDeactivate) {
        if (objectToDeactivate != null) {
            objectToDeactivate.SetActive(false);
            inactiveObjects.Push(objectToDeactivate);
        }
    }
    public IEnumerator ReturnObjectToPoolAfterTime(GameObject objectToDeactivate, float timeTilRemove)
    {
        yield return new WaitForSeconds(timeTilRemove);
        if (objectToDeactivate != null) {
            objectToDeactivate.SetActive(false);
            inactiveObjects.Push(objectToDeactivate);
        }
    }
}