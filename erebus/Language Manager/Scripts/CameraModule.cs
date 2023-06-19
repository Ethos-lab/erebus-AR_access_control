#if !(PLATFORM_LUMIN && !UNITY_EDITOR)

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UtilsModule;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rect = OpenCVForUnity.CoreModule.Rect;

/// <summary>
/// WebCamTextureToMat Example
/// An example of converting a WebCamTexture image to OpenCV's Mat format.
/// </summary>
public class CameraModule : MonoBehaviour
{
    /// <summary>
    /// Set the name of the device to use.
    /// </summary>
    [SerializeField, TooltipAttribute("Set the name of the device to use.")]
    public string requestedDeviceName = null;

    /// <summary>
    /// Set the width of WebCamTexture.
    /// </summary>
    [SerializeField, TooltipAttribute("Set the width of WebCamTexture.")]
    public int requestedWidth = 640;

    /// <summary>
    /// Set the height of WebCamTexture.
    /// </summary>
    [SerializeField, TooltipAttribute("Set the height of WebCamTexture.")]
    public int requestedHeight = 480;

    /// <summary>
    /// Set FPS of WebCamTexture.
    /// </summary>
    [SerializeField, TooltipAttribute("Set FPS of WebCamTexture.")]
    public int requestedFPS = 30;

    /// <summary>
    /// Set whether to use the front facing camera.
    /// </summary>
    [SerializeField, TooltipAttribute("Set whether to use the front facing camera.")]
    public bool requestedIsFrontFacing = true;

    /// <summary>
    /// The webcam texture.
    /// </summary>
    WebCamTexture webCamTexture;

    /// <summary>
    /// The webcam device.
    /// </summary>
    WebCamDevice webCamDevice;

    /// <summary>
    /// The rgba mat.
    /// </summary>
    Mat rgbaMat;

    /// <summary>
    /// The colors.
    /// </summary>
    Color32[] colors;

    /// <summary>
    /// The texture.
    /// </summary>
    Texture2D texture;

    /// <summary>
    /// Indicates whether this instance is waiting for initialization to complete.
    /// </summary>
    bool isInitWaiting = false;

    /// <summary>
    /// Indicates whether this instance has been initialized.
    /// </summary>
    bool hasInitDone = false;

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes webcam texture.
    /// </summary>
    private void Initialize()
    {
        if (isInitWaiting)
            return;

#if UNITY_ANDROID && !UNITY_EDITOR
            // Set the requestedFPS parameter to avoid the problem of the WebCamTexture image becoming low light on some Android devices (e.g. Google Pixel, Pixel2).
            // https://forum.unity.com/threads/android-webcamtexture-in-low-light-only-some-models.520656/
            // https://forum.unity.com/threads/released-opencv-for-unity.277080/page-33#post-3445178
            if (requestedIsFrontFacing)
            {
                int rearCameraFPS = requestedFPS;
                requestedFPS = 15;
                StartCoroutine(_Initialize());
                requestedFPS = rearCameraFPS;
            }
            else
            {
                StartCoroutine(_Initialize());
            }
#else
        StartCoroutine(_Initialize());
#endif
    }

    /// <summary>
    /// Initializes webcam texture by coroutine.
    /// </summary>
    private IEnumerator _Initialize()
    {
        if (hasInitDone)
            Dispose();

        isInitWaiting = true;

        // Checks camera permission state.
#if UNITY_IOS && UNITY_2018_1_OR_NEWER
            UserAuthorization mode = UserAuthorization.WebCam;
            if (!Application.HasUserAuthorization(mode))
            {
                isUserRequestingPermission = true;
                yield return Application.RequestUserAuthorization(mode);

                float timeElapsed = 0;
                while (isUserRequestingPermission)
                {
                    if (timeElapsed > 0.25f)
                    {
                        isUserRequestingPermission = false;
                        break;
                    }
                    timeElapsed += Time.deltaTime;

                    yield return null;
                }
            }

            if (!Application.HasUserAuthorization(mode))
            {

                isInitWaiting = false;
                yield break;
            }
#elif UNITY_ANDROID && UNITY_2018_3_OR_NEWER
        string permission = UnityEngine.Android.Permission.Camera;
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
        {
            isUserRequestingPermission = true;
            UnityEngine.Android.Permission.RequestUserPermission(permission);

            float timeElapsed = 0;
            while (isUserRequestingPermission)
            {
                if (timeElapsed > 0.25f)
                {
                    isUserRequestingPermission = false;
                    break;
                }
                timeElapsed += Time.deltaTime;

                yield return null;
            }
        }

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
        {
            isInitWaiting = false;
            yield break;
        }
#endif

        // Creates the camera
        var devices = WebCamTexture.devices;
        if (!String.IsNullOrEmpty(requestedDeviceName))
        {
            int requestedDeviceIndex = -1;
            if (Int32.TryParse(requestedDeviceName, out requestedDeviceIndex))
            {
                if (requestedDeviceIndex >= 0 && requestedDeviceIndex < devices.Length)
                {
                    webCamDevice = devices[requestedDeviceIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                }
            }
            else
            {
                for (int cameraIndex = 0; cameraIndex < devices.Length; cameraIndex++)
                {
                    if (devices[cameraIndex].name == requestedDeviceName)
                    {
                        webCamDevice = devices[cameraIndex];
                        webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                        break;
                    }
                }
            }
            if (webCamTexture == null)
                Debug.Log("Cannot find camera device " + requestedDeviceName + ".");
        }

        if (webCamTexture == null)
        {
            // Checks how many and which cameras are available on the device
            for (int cameraIndex = 0; cameraIndex < devices.Length; cameraIndex++)
            {
#if UNITY_2018_3_OR_NEWER
                if (devices[cameraIndex].kind != WebCamKind.ColorAndDepth && devices[cameraIndex].isFrontFacing == requestedIsFrontFacing)
#else
                    if (devices[cameraIndex].isFrontFacing == requestedIsFrontFacing)
#endif
                {
                    webCamDevice = devices[cameraIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
                    break;
                }
            }
        }

        if (webCamTexture == null)
        {
            if (devices.Length > 0)
            {
                webCamDevice = devices[0];
                webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
            }
            else
            {
                Debug.LogError("Camera device does not exist.");
                isInitWaiting = false;
                yield break;
            }
        }

        // Starts the camera.
        webCamTexture.Play();

        while (true)
        {
            if (webCamTexture.didUpdateThisFrame)
            {
                Debug.Log("name:" + webCamTexture.deviceName + " width:" + webCamTexture.width + " height:" + webCamTexture.height + " fps:" + webCamTexture.requestedFPS);
                Debug.Log("videoRotationAngle:" + webCamTexture.videoRotationAngle + " videoVerticallyMirrored:" + webCamTexture.videoVerticallyMirrored + " isFrongFacing:" + webCamDevice.isFrontFacing);

                isInitWaiting = false;
                hasInitDone = true;
                IsPaused = false;

                OnInited();

                break;
            }
            else
            {
                yield return null;
            }
        }
    }

#if (UNITY_IOS && UNITY_2018_1_OR_NEWER) || (UNITY_ANDROID && UNITY_2018_3_OR_NEWER)
    bool isUserRequestingPermission;

    IEnumerator OnApplicationFocus(bool hasFocus)
    {
        yield return null;

        if (isUserRequestingPermission && hasFocus)
            isUserRequestingPermission = false;
    }
#endif

    /// <summary>
    /// Releases all resource.
    /// </summary>
    private void Dispose()
    {
        isInitWaiting = false;
        hasInitDone = false;

        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            WebCamTexture.Destroy(webCamTexture);
            webCamTexture = null;
        }
        if (rgbaMat != null)
        {
            rgbaMat.Dispose();
            rgbaMat = null;
        }
        if (texture != null)
        {
            Texture2D.Destroy(texture);
            texture = null;
        }
    }

    public GameObject renderCanvas;
    public GameObject bboxGobj;

    /// <summary>
    /// Raises the webcam texture initialized event.
    /// </summary>
    private void OnInited()
    {
        if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height)
            colors = new Color32[webCamTexture.width * webCamTexture.height];
        if (texture == null || texture.width != webCamTexture.width || texture.height != webCamTexture.height)
            texture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);

        rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
        Utils.matToTexture2D(rgbaMat, texture, colors);

        renderCanvas.GetComponent<RawImage>().texture = texture;

        //gameObject.transform.localScale = new Vector3(webCamTexture.width, webCamTexture.height, 1);
        print(webCamTexture.width + ":" + webCamTexture.height);
        Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

        float width = rgbaMat.width();
        float height = rgbaMat.height();

        float widthScale = (float)Screen.width / width;
        float heightScale = (float)Screen.height / height;
        if (widthScale < heightScale)
        {
            Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
        }
        else
        {
            Camera.main.orthographicSize = height / 2;
        }
        IsPaused = false;
    }

    public void OnSaveButtonClick()
    {
        Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);

        //Save mat to rotated bytes
        Mat saveMat = new Mat();
        Core.rotate(rgbaMat, saveMat, Core.ROTATE_90_COUNTERCLOCKWISE);
        Imgproc.cvtColor(saveMat, saveMat, Imgproc.COLOR_BGR2GRAY);

        Mat resizedCamMat = new Mat();
        var renderCanvasSize = renderCanvas.GetComponent<RectTransform>().sizeDelta;
        Imgproc.resize(saveMat, resizedCamMat, new Size(renderCanvasSize.y, renderCanvasSize.x));

        var boxPos = bboxGobj.GetComponent<RectTransform>().anchoredPosition; //(PosX, PosY)
        var boxSize = bboxGobj.GetComponent<RectTransform>().sizeDelta;// new Size(250, 250); //(W,H)

        Rect roiRect = new Rect((int)boxPos.x, -(int)boxPos.y, (int)boxSize.x, (int)boxSize.y);
        roiSaveMat = new Mat(resizedCamMat, roiRect);

        ////Read bytes to Mat
        //var readBytes = File.ReadAllBytes(savePath);
        //Mat readMatBytes = new Mat(1, readBytes.Length, CvType.CV_8UC1);
        //MatUtils.copyToMat(readBytes, readMatBytes);

        //var saveJpgPath = @"C:\Users\YoonsangKim\Desktop\OpenCVTest\Assets\test.jpg";
        //var readMat = Imgcodecs.imdecode(readMatBytes, Imgcodecs.IMREAD_UNCHANGED);
        //Imgcodecs.imwrite(saveJpgPath, readMat);
        //Debug.Log("SAVED : " + DateTime.Now);
    }
    private Mat roiSaveMat = null;
    public byte[] GetCurrentFaceBytes()
    {
        MatOfByte saveBytes = new MatOfByte();
        Imgcodecs.imencode(".jpg", roiSaveMat, saveBytes);
        //var savePath = @"C:\Users\YoonsangKim\Desktop\OpenCVTest\Assets\test.bytes";
        //File.WriteAllBytes(savePath, saveBytes.toArray());
        return saveBytes.toArray();
    }
    public void SaveToImageFromStringBytes(string path, string faceDataString)
    {
        var faceDataBytes = System.Convert.FromBase64String(faceDataString);
        Mat readMatBytes = new Mat(1, faceDataBytes.Length, CvType.CV_8UC1);
        MatUtils.copyToMat(faceDataBytes, readMatBytes);

        var saveJpgPath = path;// @"C:\Users\YoonsangKim\Desktop\OpenCVTest\Assets\test.jpg";
        var readMat = Imgcodecs.imdecode(readMatBytes, Imgcodecs.IMREAD_UNCHANGED);
        Imgcodecs.imwrite(saveJpgPath, readMat);
    }
    void Update()
    {
        if (hasInitDone && webCamTexture.isPlaying && webCamTexture.didUpdateThisFrame)
        {
            Utils.webCamTextureToMat(webCamTexture, rgbaMat, colors);

            //Imgproc.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
            Utils.matToTexture2D(rgbaMat, texture, colors);
        }
    }

    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    void OnDestroy()
    {
        Dispose();
    }

    /// <summary>
    /// Raises the back button click event.
    /// </summary>
    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("OpenCVForUnityExample");
    }

    /// <summary>
    /// Raises the play button click event.
    /// </summary>
    public void OnPlayButtonClick()
    {
        if (hasInitDone)
        {
            IsPaused = false;
            webCamTexture.Play();
        }
    }

    public bool IsPaused { get; set; } = false;
    /// <summary>
    /// Raises the pause button click event.
    /// </summary>
    public void OnPauseButtonClick()
    {
        if (hasInitDone)
        {
            IsPaused = true;
            webCamTexture.Pause();
            OnSaveButtonClick();
        }
    }

    /// <summary>
    /// Raises the stop button click event.
    /// </summary>
    public void OnStopButtonClick()
    {
        if (hasInitDone)
            webCamTexture.Stop();
    }

    /// <summary>
    /// Raises the change camera button click event.
    /// </summary>
    public void OnChangeCameraButtonClick()
    {
        if (hasInitDone)
        {
            requestedDeviceName = null;
            requestedIsFrontFacing = !requestedIsFrontFacing;
            Initialize();
        }
    }
}
#endif