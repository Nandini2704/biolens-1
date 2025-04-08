using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSessionFix : MonoBehaviour
{
    private ARSession session;

    void Awake()
    {
        session = FindObjectOfType<ARSession>();
    }

    void Update()
    {
        if (session != null && session.subsystem != null)
        {
            try
            {
                session.subsystem.Update(new UnityEngine.XR.ARSubsystems.XRSessionUpdateParams());
            }
            catch (MissingReferenceException e)
            {
                Debug.LogWarning($"❗ ARSession caught missing reference: {e.Message}");
            }
        }
    }
}
