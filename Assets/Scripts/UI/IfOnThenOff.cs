using UnityEngine;

public class IfOnThenOff : MonoBehaviour
{
    [SerializeField] private GameObject ifOnObject;
    [SerializeField] private GameObject thenOffObject;

    private void Update()
    {
        if (ifOnObject == null || thenOffObject == null)
        {
            Debug.LogError("IfOnObject or ThenOffObject is not assigned in the inspector.", this);
            return;
        }
        // Check if the ifOnObject is active
        if (ifOnObject.activeSelf)
        {
            // If it is, deactivate the thenOffObject
            thenOffObject.SetActive(false);
        }
        else
        {
            // If it is not, activate the thenOffObject
            thenOffObject.SetActive(true);
        }
    }


}
