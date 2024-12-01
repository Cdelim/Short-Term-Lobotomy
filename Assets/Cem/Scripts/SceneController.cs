using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneNameEnum
{
    StartScene = 0,
    LevelScene = 1,
}

namespace Utility
{
    public class SceneController : MonoBehaviour
    {

        public static SceneController Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(this);

        }


        public void LoadScene(SceneNameEnum sceneNameEnum)
        {
            SceneManager.LoadScene((int)sceneNameEnum);
        }
    }

}
