using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    void Start()
    {
        // Load the next scene after 1 second
        Invoke("LoadNextScene", 1f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
