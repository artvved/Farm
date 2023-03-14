using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class RestartService : MonoBehaviour
    {
        public void Restart()
        {
            SceneManager.LoadScene(0);
        }
    }
}