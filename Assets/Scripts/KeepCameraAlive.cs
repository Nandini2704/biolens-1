using UnityEngine;

public class KeepCameraAlive : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
