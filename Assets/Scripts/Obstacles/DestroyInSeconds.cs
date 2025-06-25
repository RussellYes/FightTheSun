using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField] private float selfDestructTime = 1.5f;


    private void Start()
    {
        Destroy(gameObject, selfDestructTime);
    }

    private void OnDisable()
    {
        // Ensure the object is destroyed if it is disabled before the timer ends
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
