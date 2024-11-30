using UnityEngine;

public class TargetTest : MonoBehaviour
{
    public static TargetTest Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}


