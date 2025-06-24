using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField] private float selfDestructTime = 1.5f;


    private void Start()
    {
        Destroy(gameObject, selfDestructTime);
    }
}
