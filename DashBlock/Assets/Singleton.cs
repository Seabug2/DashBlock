using UnityEngine;

public class Singleton : MonoBehaviour
{
    protected virtual void Awake()
    {
        Locator.RegisterComponent(GetType(), this);
    }
}
