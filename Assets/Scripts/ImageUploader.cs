using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.Barracuda;
using UnityEngine.Android;

public class ImageUploader : MonoBehaviour
{
    public Button uploadButton;
    public Text resultText;
    public GameObject leafOptionsPanel;
    public GameObject flowerOptionsPanel;
    public NNModel modelAsset;

    private Model runtimeModel;
    private IWorker worker;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
        Debug.Log("✅ ONNX Model Loaded!");

        uploadButton.onClick.AddListener(PickImage);
        
        // Request permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
    }

    void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Debug.Log("📸 Image Path: " + path);
                byte[] imageBytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                
                ClassifyImage(texture);
            }
        }, "Select an image", "image/*");
    }

    void ClassifyImage(Texture2D image)
    {
        Tensor inputTensor = PreprocessImage(image);
        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput();

        string predictedClass = GetPredictedClass(outputTensor);
        resultText.text = "Prediction: " + predictedClass;
        Debug.Log("🌿 Predicted Class: " + predictedClass);

        HandlePrediction(predictedClass);
    }

    Tensor PreprocessImage(Texture2D image)
    {
        int width = 224;
        int height = 224;
        Texture2D resizedImage = new Texture2D(width, height);
        Graphics.ConvertTexture(image, resizedImage);

        float[] inputTensorData = new float[width * height * 3];
        Color[] pixels = resizedImage.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            inputTensorData[i * 3] = pixels[i].r;
            inputTensorData[i * 3 + 1] = pixels[i].g;
            inputTensorData[i * 3 + 2] = pixels[i].b;
        }

        return new Tensor(1, 3, width, height, inputTensorData);
    }

    string GetPredictedClass(Tensor outputTensor)
    {
        float maxVal = float.MinValue;
        int maxIndex = 0;

        for (int i = 0; i < outputTensor.length; i++)
        {
            if (outputTensor[i] > maxVal)
            {
                maxVal = outputTensor[i];
                maxIndex = i;
            }
        }

        if (maxIndex == 0) return "Leaf";
        if (maxIndex == 1) return "Flower";
        return "Unknown";
    }

    void HandlePrediction(string predictedClass)
    {
        leafOptionsPanel.SetActive(false);
        flowerOptionsPanel.SetActive(false);

        if (predictedClass == "Leaf")
        {
            leafOptionsPanel.SetActive(true);
        }
        else if (predictedClass == "Flower")
        {
            flowerOptionsPanel.SetActive(true);
        }
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}
