using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private SceneNameEnum nextScene;

    /// <summary>
    /// Button Event
    /// </summary>
    public void LoadScene()
    {
        Utility.SceneController.Instance.LoadScene(nextScene);
    }
}
