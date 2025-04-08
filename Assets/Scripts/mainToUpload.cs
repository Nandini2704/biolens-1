using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class mainToUpload : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadMainSceneAfterDelay(3f)); // 3 seconds delay
    }

    IEnumerator LoadMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1); // Loads the Main Scene (index 1)
    }
}
