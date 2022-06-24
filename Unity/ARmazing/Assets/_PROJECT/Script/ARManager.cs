using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System;

public class ARManager : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;
    private Dictionary<string, GameObject> _arUICanvases = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject _arUICanvasPrefab;
    //[SerializeField] private XRReferenceImageLibrary runtimeImageLibrary;
    [SerializeField] private TMP_Text debugLog;
    [SerializeField] private TMP_Text jobLog;
    [SerializeField] private TMP_Text currentImageText;
    //[SerializeField] private XRReferenceImageLibrary runtimeImageLibrary;

    //public List<Texture2D> refImages;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        
        /*
        try
        {
            _trackedImageManager.referenceLibrary = _trackedImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
        }
        catch (Exception e)
        {
            debugLog.text += e.ToString();
        }
        */
        ShowTrackerInfo();

        //StartCoroutine(AddAllImagesToMutableReferenceImageLibraryAR());
    }
    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public void ShowTrackerInfo()
    {
        var runtimeReferenceImageLibrary = _trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

        debugLog.text += $"TextureFormat.RGBA32 supported: {runtimeReferenceImageLibrary.IsTextureFormatSupported(TextureFormat.RGBA32)}\n";
        debugLog.text += $"Supported Texture Count ({runtimeReferenceImageLibrary.supportedTextureFormatCount})\n";
        debugLog.text += $"trackImageManager.trackables.count ({_trackedImageManager.trackables.count})\n";
        debugLog.text += $"trackImageManager.maxNumberOfMovingImages ({_trackedImageManager.requestedMaxNumberOfMovingImages})\n";
        debugLog.text += $"trackImageManager.supportsMutableLibrary ({_trackedImageManager.subsystem.subsystemDescriptor.supportsMutableLibrary})\n";
        debugLog.text += $"trackImageManager.requiresPhysicalImageDimensions ({_trackedImageManager.subsystem.subsystemDescriptor.requiresPhysicalImageDimensions})\n";
    }

    /*private IEnumerator AddAllImagesToMutableReferenceImageLibraryAR()
    {
        yield return null;

        debugLog.text = string.Empty;

        debugLog.text += "Adding image\n";

        jobLog.text = "Job Starting...";

        var firstGuid = new SerializableGuid(0, 0);
        var secondGuid = new SerializableGuid(0, 0);

        XRReferenceImage newImage = new XRReferenceImage(firstGuid, secondGuid, new Vector2(0.1f, 0.1f), refImages[0].name, refImages[1]);

        try
        {
            Debug.Log(newImage.ToString());

            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = _trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

            debugLog.text += $"TextureFormat.RGBA32 supported: {mutableRuntimeReferenceImageLibrary.IsTextureFormatSupported(TextureFormat.RGBA32)}\n";

            debugLog.text += $"TextureFormat size: {refImages[0].width}px width {refImages[0].height}px height\n";

            var jobHandle = mutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(refImages[0], refImages[0].name, 0.1f);

            while (!jobHandle.jobHandle.IsCompleted)
            {
                jobLog.text = "Job Running...";
            }

            jobLog.text = "Job Completed... Added " + refImages[1].name;
            _trackedImageManager.referenceLibrary = mutableRuntimeReferenceImageLibrary;
            debugLog.text += $"Job Completed ({mutableRuntimeReferenceImageLibrary.count})\n";
            debugLog.text += $"Supported Texture Count ({mutableRuntimeReferenceImageLibrary.supportedTextureFormatCount})\n";
            debugLog.text += $"trackImageManager.trackables.count ({_trackedImageManager.trackables.count})\n";
            debugLog.text += $"trackImageManager.maxNumberOfMovingImages ({_trackedImageManager.requestedMaxNumberOfMovingImages})\n";
            debugLog.text += $"trackImageManager.supportsMutableLibrary ({_trackedImageManager.subsystem.subsystemDescriptor.supportsMutableLibrary})\n";
            debugLog.text += $"trackImageManager.requiresPhysicalImageDimensions ({_trackedImageManager.subsystem.subsystemDescriptor.requiresPhysicalImageDimensions})\n";
        }
        catch (Exception e)
        {
            debugLog.text = e.ToString();
        }
    }*/

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            GameObject arUICanvas = Instantiate(_arUICanvasPrefab);
            _arUICanvases.Add(trackedImage.referenceImage.name, arUICanvas);
            currentImageText.text = trackedImage.referenceImage.name;
            UpdateARUI(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            currentImageText.text = trackedImage.referenceImage.name;
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                UpdateARUI(trackedImage);
            }
            else
            {
                _arUICanvases[trackedImage.referenceImage.name].SetActive(false);
            }
        }
    }

    void UpdateARUI(ARTrackedImage trackedImage)
    {
        GameObject arUICanvas = null;

        foreach (KeyValuePair<string, GameObject> canvas in _arUICanvases)
        {
            if (canvas.Key == trackedImage.referenceImage.name)
            {
                canvas.Value.SetActive(true);
                arUICanvas = canvas.Value;
            }
            else
            {
                canvas.Value.SetActive(false);
            }
        }

        GameObject arPanel = arUICanvas.transform.Find("Panel").gameObject;

        TMP_Text titleText = arPanel.transform.Find("Title_Text").gameObject.GetComponent<TMP_Text>();
        titleText.text = trackedImage.referenceImage.name;
        TMP_Text descText = arPanel.transform.Find("Desc_Text").gameObject.GetComponent<TMP_Text>();
        descText.text = "Description of " + trackedImage.referenceImage.name;
    }

}
