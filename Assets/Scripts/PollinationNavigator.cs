using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PollinationNavigator : MonoBehaviour
{
    public Button selfPollinationButton;
    public Button crossPollinationButton;

    void Start()
    {
        if (selfPollinationButton != null)
            selfPollinationButton.onClick.AddListener(OpenSelfPollination);

        if (crossPollinationButton != null)
            crossPollinationButton.onClick.AddListener(OpenCrossPollination);
    }

    void OpenSelfPollination()
    {
        SceneManager.LoadScene("SelfPollinationScene");
    }

    void OpenCrossPollination()
    {
        SceneManager.LoadScene("CrossPollinationScene");
    }
}
