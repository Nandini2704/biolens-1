using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SceneLoader : MonoBehaviour
{
    public ARSession arSession;

    public void LoadScene(string sceneName)
    {
        if (arSession != null)
        {
            arSession.enabled = false; // ✅ Stop ARSession before switching scenes
        }

        SceneManager.LoadScene(sceneName);
    }
}
