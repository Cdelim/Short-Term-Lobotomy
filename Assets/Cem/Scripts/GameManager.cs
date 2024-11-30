using UnityEngine;

public enum GameState
{
    Started,
    Playing,
    GameEnd,
    Paused,
    Resume,
}

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Game state variables
    public GameState GameState { get; set; }

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
        GameState = GameState.Started;
    }



}

