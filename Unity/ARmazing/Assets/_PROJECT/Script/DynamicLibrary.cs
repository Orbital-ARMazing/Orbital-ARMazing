using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARTrackedImageManager))]
public class DynamicLibrary : MonoBehaviour
{
    [Serializable]
    public class ImageData
    {
        [SerializeField, Tooltip("The source texture for the image. Must be marked as readable.")]
        Texture2D m_Texture;

        public Texture2D texture
        {
            get => m_Texture;
            set => m_Texture = value;
        }

        [SerializeField, Tooltip("The name for this image.")]
        string m_Name;

        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        [SerializeField, Tooltip("The width, in meters, of the image in the real world.")]
        float m_Width;

        public float width
        {
            get => m_Width;
            set => m_Width = value;
        }

        public AddReferenceImageJobState jobState { get; set; }
    }

    [SerializeField, Tooltip("The set of images to add to the image library at runtime")]
    List<ImageData> m_Images;

    enum State
    {
        NoImagesAdded,
        AddImagesRequested,
        AddingImages,
        Done,
        Error
    }

    State m_State;

    string m_ErrorMessage = "";

    StringBuilder m_StringBuilder = new StringBuilder();

    /**
     * Draws a GUI at runtime that will display debug messages or error messages 
     */
    void OnGUI()
    {
        var fontSize = 50;
        GUI.skin.button.fontSize = fontSize;
        GUI.skin.label.fontSize = fontSize;

        float margin = 100;

        GUILayout.BeginArea(new Rect(margin, margin, Screen.width - margin * 2, Screen.height - margin * 2));

        switch (m_State)
        {
            case State.NoImagesAdded:
                {
                    //m_State = State.AddImagesRequested;

                    break;
                }
            case State.AddingImages:
                {
                    m_StringBuilder.Clear();
                    m_StringBuilder.AppendLine("Add image status:");
                    foreach (var image in m_Images)
                    {
                        m_StringBuilder.AppendLine($"\t{image.name}: {(image.jobState.status.ToString())}");
                    }
                    GUILayout.Label(m_StringBuilder.ToString());
                    break;
                }
            case State.Done:
                {
                    GUILayout.Label("");
                    break;
                }
            case State.Error:
                {
                    GUILayout.Label(m_ErrorMessage);
                    break;
                }
        }

        GUILayout.EndArea();
    }

    void SetError(string errorMessage)
    {
        m_State = State.Error;
        m_ErrorMessage = $"Error: {errorMessage}";
    }

    /**
     * Run once on scene start
     * 1. Start the adding images sequence with specified delay
     * due compansate for some mobile devices slowness in start up camera
     */
    private void Start()
    {
        StartCoroutine(AddImages(3));
    }

    /**
     * Set the state to signal the start of loading images dynamically into Image Library
     */
    IEnumerator AddImages(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_State = State.AddImagesRequested;
    }

    /**
     * Run every frame
     * 1. If state if AddImageRequest, read images from persistent data path
     * and load them into the XR Reference Image Library at runtime
     * 2. Adding images to XR Reference Image Library is a asynchronous the state AddingImages
     * wait for images to complete loading into the XR Reference Image Library
     * 3. Once all images are adding the state is set to Done
     */
    void Update()
    {
        switch (m_State)
        {
            case State.AddImagesRequested:
                {
                    string[] files = Directory.GetFiles(Application.persistentDataPath, "*.JPG");
                    foreach (string file in files)
                    {
                        m_Images.Add(LoadImage(file));
                    }
                        
                  
                    if (m_Images == null)
                    {
                        SetError("No images to add.");
                        break;
                    }

                    var manager = GetComponent<ARTrackedImageManager>();
                    if (manager == null)
                    {
                        SetError($"No {nameof(ARTrackedImageManager)} available.");
                        break;
                    }

                    // You can either add raw image bytes or use the extension method (used below) which accepts
                    // a texture. To use a texture, however, its import settings must have enabled read/write
                    // access to the texture.
                    foreach (var image in m_Images)
                    {
                        if (!image.texture.isReadable)
                        {
                            SetError($"Image {image.name} must be readable to be added to the image library.");
                            break;
                        }
                    }

                    if (manager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
                    {
                        try
                        {
                            foreach (var image in m_Images)
                            {
                                // Note: You do not need to do anything with the returned JobHandle, but it can be
                                // useful if you want to know when the image has been added to the library since it may
                                // take several frames.
                                image.jobState = mutableLibrary.ScheduleAddImageWithValidationJob(image.texture, image.name, image.width);
                            }

                            m_State = State.AddingImages;
                        }
                        catch (InvalidOperationException e)
                        {
                            SetError($"ScheduleAddImageJob threw exception: {e.Message}");
                        }
                    }
                    else
                    {
                        SetError($"The reference image library is not mutable.");
                    }

                    break;
                }
            case State.AddingImages:
                {
                    // Check for completion
                    var done = true;
                    foreach (var image in m_Images)
                    {
                        if (!image.jobState.jobHandle.IsCompleted)
                        {
                            done = false;
                            break;
                        }
                    }

                    if (done)
                    {
                        m_State = State.Done;
                    }

                    break;
                }
        }
    }

     /**
      * Loads image data, byte array, from specified file path and return a ImageData object
      * 
      * @param filePath string of the image location in local storage
      * @return a ImageData object of the loaded image
      */
    ImageData LoadImage(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;
        ImageData imageData = new ImageData();

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2,2, TextureFormat.RGBA32,false);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            imageData.texture = tex;
            imageData.name = Path.GetFileName(filePath).Replace(".JPG", "");
            imageData.width = 0.1f;
        }
        return imageData;
    }
}

public class AssetResponse
{
    public bool status;
    public string error;
    public List<AssetData> msg;
}

public class AssetData
{
    public string id;
    public string eventID;
    public string name;
    public string description;
    public string latitude;
    public string longitude;
    public bool visible;
    public string visibleText;
    public string imagePath;
    public bool quizCompleted;
}