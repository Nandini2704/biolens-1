using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    public Button photosynthesisButton;
    public Button plantCycleButton;

    void Start()
    {
        if (photosynthesisButton != null)
            photosynthesisButton.onClick.AddListener(OpenPhotosynthesis);

        if (plantCycleButton != null)
            plantCycleButton.onClick.AddListener(OpenPlantCycle);
    }

    void OpenPhotosynthesis()
    {
        SceneManager.LoadScene("photo");
    }

    void OpenPlantCycle()
    {
        SceneManager.LoadScene("plantcycle");
    }
}
