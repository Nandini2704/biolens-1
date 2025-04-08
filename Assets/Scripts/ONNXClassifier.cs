using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using System.IO;
using UnityEngine.SceneManagement;

public class ONNXClassifier : MonoBehaviour
{
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;

    public RawImage displayImage;
    public Button scanButton;

    void Start()
    {
        if (modelAsset != null)
        {
            runtimeModel = ModelLoader.Load(modelAsset);
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);
        }

        if (scanButton != null)
        {
            scanButton.onClick.AddListener(OpenGallery);
            Debug.Log("✅ Scan button connected");
        }
        else
        {
            Debug.LogError("🚨 Scan button not assigned in inspector!");
        }
    }

    public void OpenGallery()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log($"📸 Image selected: {path}");
                StartCoroutine(LoadAndClassifyImage(path));
            }
            else
            {
                Debug.LogError("🚨 No image selected!");
            }
        }, "Select an image", "image/*");

        if (permission != NativeGallery.Permission.Granted)
        {
            Debug.LogError("🚨 Permission not granted to access gallery.");
        }
    }

    IEnumerator LoadAndClassifyImage(string path)
    {
        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        if (displayImage != null)
        {
            displayImage.texture = texture;
        }

        Tensor inputTensor = PreprocessImage(texture);
        if (inputTensor != null)
        {
            yield return ClassifyImage(inputTensor);
            inputTensor.Dispose();
        }
    }

    Tensor PreprocessImage(Texture2D image)
    {
        if (image == null) return null;

        int width = 224;
        int height = 224;

        Texture2D resizedImage = new Texture2D(width, height);
        Graphics.ConvertTexture(image, resizedImage);

        return new Tensor(resizedImage, channels: 3);
    }

    IEnumerator ClassifyImage(Tensor inputTensor)
    {
        if (worker == null || inputTensor == null) yield break;

        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput();

        if (outputTensor != null)
        {
            string prediction = GetPredictedClass(outputTensor);
            HandlePrediction(prediction);
            outputTensor.Dispose();
        }

        yield return null;
    }

    string GetPredictedClass(Tensor outputTensor)
    {
        if (outputTensor == null) return "Unknown";

        float[] values = outputTensor.ToReadOnlyArray();
        float sum = 0;

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = Mathf.Exp(values[i]);
            sum += values[i];
        }

        for (int i = 0; i < values.Length; i++)
        {
            values[i] /= sum;
        }

        float maxVal = float.MinValue;
        int maxIndex = -1;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > maxVal)
            {
                maxVal = values[i];
                maxIndex = i;
            }
        }
        Debug.Log($"🔎 Output Values: {string.Join(", ", maxVal)}");

        if ( maxVal<=0.0018)
        {
            return "Flower";
        }
        else if (maxVal <= 0.002)// || maxVal>=0.002)
        {
            return "Leaf";
        }

        return "Unknown";
    }

    void HandlePrediction(string prediction)
    {
        if (string.IsNullOrEmpty(prediction)) return;

        if (prediction == "Leaf")
        {
            Debug.Log("🍃 Detected Leaf - Loading Photosynthesis Scene");
            if (SceneManager.GetActiveScene().name != "PhotosynthesisScene")
            {
                SceneManager.LoadScene("PhotosynthesisScene");
            }
        }
        else if (prediction == "Flower")
        {
            Debug.Log("🌸 Detected Flower - Loading Pollination Scene");
            if (SceneManager.GetActiveScene().name != "PollinationScene")
            {
                SceneManager.LoadScene("PollinationScene");
            }
        }
        else
        {
            Debug.Log("❓ Unknown object - Loading Default Scene");
            if (SceneManager.GetActiveScene().name != "UnknownScene")
            {
                SceneManager.LoadScene("UnknownScene");
            }
        }
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
