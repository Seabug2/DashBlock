using UnityEngine;

public class Trail : MonoBehaviour
{
    public Transform target;
    public float t = 1f;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position,target.position,t * Time.deltaTime);    
    }
}
