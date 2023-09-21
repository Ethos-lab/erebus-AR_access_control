using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using NetMQ;
using NetMQ.Sockets;
using System.Reflection;
using System.Threading;
using UnityEngine.Android;
using System.Linq;
using UnityEngine.SceneManagement;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UtilsModule;
using OpenCVForUnity.ImgcodecsModule;
using System.IO;
using System.Collections.Concurrent;
using DetectionResult;
using System.IO.Compression;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using OpenCVForUnity.ImgprocModule;
using NatML.Features;
using NatML;
using NatML.Vision;

namespace Erebus
{
    namespace AR
    {
        public class ErebusARManager : MonoBehaviour
        {
            [SerializeField] private GameObject dataControllerGobj;
            [SerializeField] private GameObject sessionOriginGobj;
            [SerializeField] private GameObject popupGobj;
            [SerializeField] private ARCameraManager arCameraManager;
            [SerializeField] private ComputeShader whitelistComputeShader;
            [SerializeField] private RawImage userCanvasImage;
            //Change this to a smaller value (1.0f) for AR Navigation; More frequent update
            [SerializeField] private float gpsUpdateInterval = 5.0f;

            private UI.PopUpController UIControl = null;
            private Data.DataController dataController = null;
            private Data.DataEntryPoint appDataController = null;
            public ARFunctionProvider.FunctionProvider FunctionProvider { get; private set; } = null;

            //Erebus AR Manager (FunctionProvider) private member 
            public ConcurrentQueue<Texture2D> ARCameraDataQueue = null;
            //public ConcurrentQueue<byte[]> ARCameraDataQueue = null;
            private const int cameraDataQueueLimit = 2;
            private bool arCamConfigInitialized = false;

            private void Awake()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                    Permission.RequestUserPermission(Permission.FineLocation);
                if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
                    Permission.RequestUserPermission(Permission.CoarseLocation);
#endif
                Instantiate(dataControllerGobj);
                UIControl = new UI.PopUpController(popupGobj, this);
            }
            private void Start()
            {
                if (ARFunctionProvider.FunctionProvider.WasInstantiated)
                    ARFunctionProvider.FunctionProvider.Instance.Dispose();
                if (Data.DataEntryPoint.WasInstantiated)
                    Data.DataEntryPoint.Instance.Dispose();

                FunctionProvider = ARFunctionProvider.FunctionProvider.Instance;
                appDataController = Data.DataEntryPoint.Instance;
                dataController = Data.DataController.Instance;

                dataController.LoadData();
                FunctionProvider.Initialize(sessionOriginGobj, this, whitelistComputeShader);
                appDataController.Initialize(gpsUpdateInterval, this, this);
                UIControl.Initialize(dataController, appDataController);
                AttachARCameraConfig();
                Debug.Log("Erebus ARManager Start");
            }
            private void OnDestroy()
            {
                ARFunctionProvider.FunctionProvider.Instance.Dispose();
            }
            public void AttachARCameraConfig()
            {
                Debug.Log($"[AttachARCameraConfig]");
                ARCameraDataQueue = new ConcurrentQueue<Texture2D>();
                arCameraManager.frameReceived += OnCameraFrameUpdate;
            }
            private Texture2D curCameraTexture = null;
            private unsafe void OnCameraFrameUpdate(ARCameraFrameEventArgs eventArgs)
            {
                if (ARCameraDataQueue.Count > cameraDataQueueLimit)
                    return;

                if (!arCamConfigInitialized)
                    InitializeARCamConfigOnFirstFrame(eventArgs);

                if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                    return;

                //Original (640x480) -> imageFeature (480x640)
                var imageFeature = new MLXRCpuImageFeature(image, world: false, orientation: ScreenOrientation.PortraitUpsideDown);
                if (curCameraTexture == null)
                    curCameraTexture = new Texture2D(imageFeature.width, imageFeature.height, TextureFormat.RGBA32, false);
                imageFeature.CopyTo(curCameraTexture, upload: true);
                image.Dispose();
                userCanvasImage.texture = curCameraTexture;

                //var conversionParams = new XRCpuImage.ConversionParams
                //{
                //    // Get the entire image.
                //    inputRect = new RectInt(0, 0, image.width, image.height),
                //    outputDimensions = new Vector2Int(image.width, image.height),

                //    // Choose RGB format.
                //    //outputFormat = TextureFormat.RGB24,
                //    outputFormat = TextureFormat.RGBA32,

                //    // Flip across the vertical axis (mirror image).
                //    transformation = XRCpuImage.Transformation.None//MirrorY
                //};

                //// See how many bytes you need to store the final image.
                //int size = image.GetConvertedDataSize(conversionParams);

                //// Allocate a buffer to store the image.
                //var buffer = new NativeArray<byte>(size, Allocator.Temp);

                //// Extract the image data
                //image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

                //// The image was converted to RGBA32 format and written into the provided buffer
                //// so you can dispose of the XRCpuImage. You must do this or it will leak resources.
                //image.Dispose();

                //// At this point, you can process the image, pass it to a computer vision algorithm, etc.
                //// In this example, you apply it to a texture to visualize it.
                //Texture2D texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
                //texture.LoadRawTextureData(buffer);
                //texture.Apply(false);

                ////NativeArray<byte> jpgBytes =
                ////   ImageConversion.EncodeNativeArrayToJPG(buffer, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm,
                ////   (uint)conversionParams.outputDimensions.x, (uint)conversionParams.outputDimensions.y);

                ////NativeArray<byte> pngBytes =
                ////    ImageConversion.EncodeNativeArrayToPNG(buffer, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm,
                ////    (uint)conversionParams.outputDimensions.x, (uint)conversionParams.outputDimensions.y);
                ////Debug.Log("TIME2:" + (start_time - Time.time));
                ////m_Texture.LoadRawTextureData(pngBytes);
                ////m_Texture.Apply();

                //// Done with your temporary data, so you can dispose it.
                //buffer.Dispose();

                ARCameraDataQueue.Enqueue(curCameraTexture);
            }
            private void InitializeARCamConfigOnFirstFrame(ARCameraFrameEventArgs eventArgs)
            {
                arCameraManager.requestedFacingDirection = CameraFacingDirection.User;
                var curRes = arCameraManager.subsystem.currentConfiguration.Value.resolution;
                arCameraManager.subsystem.currentConfiguration = arCameraManager.GetConfigurations(Allocator.Temp)[0]; //Resolution : [0]=640*480, [1]= 1280*720, [2]=1920*1080
                arCamConfigInitialized = true;
                Debug.Log($"[READY]____________________________________________________________Current resolution : {curRes}");
            }
        }
        namespace ARFunctionProvider
        {
            public class FunctionProvider
            {
                public static FunctionProvider Instance
                {
                    get
                    {
                        if (instance == null)
                            instance = new FunctionProvider();
                        return instance;
                    }
                }
                private static FunctionProvider instance = null;
                public static bool WasInstantiated => (instance != null);
                private FunctionProvider() { }
                public void Dispose()
                {
                    instance = null;
                    emptyBboxBuffer?.Dispose();
                    computeOutput?.Release();
                }
                private GameObject sessionOriginGobj;
                private ErebusARPlaneManager erebusARPlaneManager;
                private ErebusARRaycastManager erebusARRaycastManager;
                private ErebusARTrackedImageManager erebusARTrackedImageManager;
                private Data.DataEntryPoint appDataController;
                private AccessControl.AccessControlWrapper accessControlWrapper = null;
                private MonoBehaviour monoBehaviourObj = null;
                public bool IsFunctionProviderReady { get; private set; } = false;
                private bool flipCameraImage = false;
                private ComputeShader whitelistComputeShader;

                public void Initialize(GameObject sessionOriginGobj, MonoBehaviour monoBehaviourObj, ComputeShader whitelistComputeShader)
                {
                    if (Application.productName == "AR Remote Maintenance")
                        flipCameraImage = true;
                    this.monoBehaviourObj = monoBehaviourObj;
                    this.sessionOriginGobj = sessionOriginGobj;
                    this.whitelistComputeShader = whitelistComputeShader;
                    erebusARPlaneManager = new ErebusARPlaneManager(this.sessionOriginGobj);
                    erebusARRaycastManager = new ErebusARRaycastManager(this.sessionOriginGobj);
                    erebusARTrackedImageManager = new ErebusARTrackedImageManager(this.sessionOriginGobj);
                    appDataController = Data.DataEntryPoint.Instance;
                }
                public void LoadWrapper(byte[] baseAssemblyBytes, byte[] assemblyBytes)
                {
                    var baseAssembly = Assembly.GetExecutingAssembly();
                    var baseProgramName = "Erebus.AR.Data.DataEntryPoint";
                    var constructorArgs = new object[] { baseAssembly, baseProgramName };

                    accessControlWrapper = new AccessControl.AccessControlWrapper(
                            baseAssemblyBytes,
                            assemblyBytes,
                            "Erebus.AccessControl.ErebusAccessControl",
                            new Type[] { typeof(Assembly), typeof(string) },
                            constructorArgs);

                    IsFunctionProviderReady = true;
                }
                #region ARFoundation Wrapper (Exposed to app developers)
                private class ErebusARPlaneManager
                {
                    public ARPlaneManager PlaneManager { get; set; } = null;
                    //public ErebusARPlanesChanged PlanesChanged { get; set; } = null;
                    public List<Action<ARPlanesChangedEventArgs>> UserEventOnPlanesChange { get; set; } = null;

                    public ErebusARPlaneManager(GameObject sessionOriginGobj)
                    {
                        PlaneManager = sessionOriginGobj.GetComponent<ARPlaneManager>();
                        if (PlaneManager != null)
                        {
                            UserEventOnPlanesChange = new List<Action<ARPlanesChangedEventArgs>>();
                            PlaneManager.planesChanged += OnPlanesChanged;
                        }
                    }
                    ~ErebusARPlaneManager()
                    {
                        if (PlaneManager != null)
                            PlaneManager.planesChanged -= OnPlanesChanged;
                    }

                    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
                    {
                        //PlanesChanged = new ErebusARPlanesChanged(args.added, args.removed, args.updated);
                        foreach (var userEventOnPlanesChange in UserEventOnPlanesChange)
                            userEventOnPlanesChange?.Invoke(args);
                    }
                }
                private class ErebusARRaycastManager
                {
                    public ARRaycastManager RaycastManager { get; set; } = null;

                    public ErebusARRaycastManager(GameObject sessionOriginGobj)
                    {
                        RaycastManager = sessionOriginGobj.GetComponent<ARRaycastManager>();
                        //Assert.IsNotNull(RaycastManager);
                    }
                }
                private class ErebusARTrackedImageManager
                {
                    public ARTrackedImageManager TrackedImageManager { get; set; } = null;
                    public List<Action<ARTrackedImagesChangedEventArgs>> UserEventOnImageChange { get; set; } = null;

                    public ErebusARTrackedImageManager(GameObject sessionOriginGobj)
                    {
                        TrackedImageManager = sessionOriginGobj.GetComponent<ARTrackedImageManager>();
                        if (TrackedImageManager != null)
                        {
                            UserEventOnImageChange = new List<Action<ARTrackedImagesChangedEventArgs>>();
                            TrackedImageManager.trackedImagesChanged += OnImageChange;
                        }
                    }
                    ~ErebusARTrackedImageManager()
                    {
                        if (TrackedImageManager != null)
                            TrackedImageManager.trackedImagesChanged -= OnImageChange;
                    }
                    private void OnImageChange(ARTrackedImagesChangedEventArgs args)
                    {
                        foreach (var userEventOnImageChange in UserEventOnImageChange)
                            userEventOnImageChange?.Invoke(args);
                    }
                }

                #region ARPlane
                public bool RegisterEventOnPlanesChange(Action<ARPlanesChangedEventArgs> userFunction)
                {
                    if (accessControlWrapper == null)
                        return false;

                    var passedTest = accessControlWrapper.GetFunction("CheckRegisterEventOnPlanesChange", null);
                    //Debug.Log($"[{passedTest}] RegisterEventOnPlanesChange");
                    if (!passedTest)
                    {
                        return false;
                    }
                    //permissionUIs[1].color = COLOR.ActivatedColor;
                    erebusARPlaneManager.UserEventOnPlanesChange.Add(userFunction);
                    return true;
                }
                public bool UnregisterEventOnPlanesChange(Action<ARPlanesChangedEventArgs> userFunction)
                {
                    erebusARPlaneManager.UserEventOnPlanesChange.Remove(userFunction);
                    return true;
                }
                public ARPlane GetPlane(TrackableId trackableId)
                {
                    if (accessControlWrapper == null)
                        return null;

                    var passedTest = accessControlWrapper.GetFunction("CheckGetPlane", null);
                    Debug.Log($"[{passedTest}] GetPlane");
                    if (!passedTest)
                    {
                        //permissionUIs[0].color = COLOR.DeactivatedColor;
                        return null;
                    }

                    //permissionUIs[0].color = COLOR.ActivatedColor;
                    return erebusARPlaneManager.PlaneManager.GetPlane(trackableId);
                }
                #endregion

                #region ARRaycast
                public bool Raycast(Vector2 screenPoint, List<ARRaycastHit> hitResults, TrackableType trackableTypes = TrackableType.All)
                {
                    if (accessControlWrapper == null)
                        return false;

                    var passedTest = accessControlWrapper.GetFunction("CheckRayCast", null);
                    Debug.Log($"[{passedTest}] Raycast1");
                    if (!passedTest)
                    {
                        //permissionUIs[3].color = COLOR.DeactivatedColor;
                        return false;
                    }

                    //permissionUIs[3].color = COLOR.ActivatedColor;
                    return erebusARRaycastManager.RaycastManager.Raycast(screenPoint, hitResults, trackableTypes);
                }
                public bool Raycast(Ray ray, List<ARRaycastHit> hitResults, TrackableType trackableTypes = TrackableType.All)
                {
                    if (accessControlWrapper == null)
                        return false;

                    var passedTest = accessControlWrapper.GetFunction("CheckRayCast", null);
                    Debug.Log($"[{passedTest}] Raycast2");
                    if (!passedTest)
                    {
                        //permissionUIs[3].color = COLOR.DeactivatedColor;
                        return false;
                    }

                    return erebusARRaycastManager.RaycastManager.Raycast(ray, hitResults, trackableTypes);
                }
                #endregion

                #region ARPlane Trackables
                public TrackableCollection<ARPlane> ARPlaneTrackables
                {
                    get
                    {
                        if (accessControlWrapper == null)
                            return default;

                        var passedTest = accessControlWrapper.GetFunction("CheckARPlaneTrackables", null);
                        //Debug.Log($"[{passedTest}] ARPlaneTrackables");
                        if (!passedTest)
                        {
                            //permissionUIs[2].color = COLOR.DeactivatedColor;
                            return default;
                        }
                        //permissionUIs[2].color = COLOR.ActivatedColor;
                        return erebusARPlaneManager.PlaneManager.trackables;
                    }
                }
                #endregion

                #region AR Tracked Images
                public class TrackedImageAddStatus
                {
                    public TrackedImageAddStatus() { IsComplete = false; }
                    public bool IsComplete { get; set; } = false;
                }
                public TrackedImageAddStatus AddTrackedImage(Texture2D imageLibraryTexture, string imageLibraryName, GameObject trackedImagePrefab)
                {
                    if (accessControlWrapper == null)
                        return null;

                    var passedTest = accessControlWrapper.GetFunction("CheckAddTrackedImage", null);
                    Debug.Log($"[{passedTest}] AddTrackedImage");
                    if (!passedTest)
                    {
                        return null;
                    }
                    //permissionUIs[1].color = COLOR.ActivatedColor;
                    var addStatus = new TrackedImageAddStatus();
                    monoBehaviourObj.StartCoroutine(AddImage(imageLibraryTexture, imageLibraryName, trackedImagePrefab, addStatus));
                    return addStatus;
                }
                private IEnumerator AddImage(Texture2D imageToAdd, string targetImageLibraryName, GameObject trackedImagePrefab, TrackedImageAddStatus addStatus)
                {
                    //This function CANNOT be invoked from the Awake() function
                    //It fails to add an img library in the Awake()
                    var referenceLibrary = erebusARTrackedImageManager.TrackedImageManager.CreateRuntimeLibrary();
                    if (referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
                    {
                        var addJob = mutableLibrary.ScheduleAddImageWithValidationJob(imageToAdd, targetImageLibraryName, 0.2f);
                        yield return new WaitUntil(() => addJob.status.IsComplete());
                        erebusARTrackedImageManager.TrackedImageManager.referenceLibrary = mutableLibrary;
                        erebusARTrackedImageManager.TrackedImageManager.requestedMaxNumberOfMovingImages = 1;
                        erebusARTrackedImageManager.TrackedImageManager.trackedImagePrefab = trackedImagePrefab;
                        erebusARTrackedImageManager.TrackedImageManager.enabled = true;
                        Debug.Log("referenceLibrary.count = " + erebusARTrackedImageManager.TrackedImageManager.referenceLibrary.count);
                        addStatus.IsComplete = true;
                    }
                }
                public bool RegisterEventOnImageChange(Action<ARTrackedImagesChangedEventArgs> userFunction)
                {
                    if (accessControlWrapper == null)
                        return false;

                    var passedTest = accessControlWrapper.GetFunction("CheckRegisterEventOnImageChange", null);
                    //Debug.Log($"[{passedTest}] RegisterEventOnImageChange");
                    if (!passedTest)
                    {
                        return false;
                    }
                    //permissionUIs[1].color = COLOR.ActivatedColor;
                    erebusARTrackedImageManager.UserEventOnImageChange.Add(userFunction);
                    return true;
                }
                public bool UnregisterEventOnImageChange(Action<ARTrackedImagesChangedEventArgs> userFunction)
                {
                    erebusARTrackedImageManager.UserEventOnImageChange.Remove(userFunction);
                    return true;
                }
                #endregion

                #region Object Detection
                public Texture2D GetObjectRawPixels()
                {
                    if (accessControlWrapper == null)
                        return null;

                    var passedTest = accessControlWrapper.GetFunction("CheckGetObjectRawPixels", null);
                    //Debug.Log($"[{passedTest}] GetObjectRawPixels");
                    if (!passedTest)
                    {
                        //permissionUIs[0].color = COLOR.DeactivatedColor;
                        return null;
                    }

                    //permissionUIs[0].color = COLOR.ActivatedColor;
                    var allowedTexture2D = GetWhitelistedRawPixels();
                    return allowedTexture2D;
                }
                private Texture computeInput;
                private RenderTexture computeOutput = null;
                private ComputeBuffer emptyBboxBuffer = null;
                private int computeKernelIndex;
                private Texture2D GetWhitelistedRawPixels()
                {
                    appDataController = Data.DataEntryPoint.Instance;
                    var curTexture = appDataController.curFrameRawTexture;
                    var targetBboxes = appDataController.curFrameDetectionResult.Data;
                    Bbox[] curDetections = new Bbox[targetBboxes.Count];
                    for (int i = 0; i < curDetections.Length; i++)
                    {
                        var bbox = targetBboxes[i].Bbox;
                        curDetections[i] = new Bbox(bbox[0], bbox[1], bbox[2], bbox[3]);
                    }

                    computeInput = curTexture;
                    if (computeOutput == null)
                    {
                        computeOutput = new RenderTexture(computeInput.width, computeInput.height, 0, RenderTextureFormat.ARGB32);
                        computeOutput.enableRandomWrite = true;
                        computeOutput.Create();
                        computeKernelIndex = whitelistComputeShader.FindKernel("CSMain");
                        emptyBboxBuffer = new ComputeBuffer(1, sizeof(int));
                        emptyBboxBuffer.SetData(new int[] { 0 });
                    }

                    //TopLeft origin-based coordinate system
                    var hasBboxDetections = curDetections.Length > 0;
                    ComputeBuffer bboxBuffer = emptyBboxBuffer;
                    if (hasBboxDetections)
                    {
                        int stride = sizeof(int) * 4;
                        bboxBuffer = new ComputeBuffer(curDetections.Length, stride);
                        bboxBuffer.SetData(curDetections);
                    }

                    whitelistComputeShader.SetInt("bboxBufferSize", curDetections.Length);
                    whitelistComputeShader.SetInt("inputTextureHeight", computeInput.height);
                    whitelistComputeShader.SetBuffer(computeKernelIndex, "bboxBuffer", bboxBuffer);
                    whitelistComputeShader.SetTexture(computeKernelIndex, "inputTexture", computeInput);
                    whitelistComputeShader.SetTexture(computeKernelIndex, "outputTexture", computeOutput);

                    whitelistComputeShader.Dispatch(computeKernelIndex, computeOutput.width / 8, computeOutput.height / 8, 1);

                    var whitelistedTexture = SyncReadback(computeOutput, upload: true);

                    ////Async GPUReadback
                    //AsyncGPUReadback.Request(computeOutput, 0, OnCompleteReadback);

                    if (hasBboxDetections)
                        bboxBuffer.Dispose();

                    return whitelistedTexture;
                }
                private Texture2D SyncReadback(RenderTexture rt, bool upload = false)
                {
                    Texture2D inputTex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
                    var curActive = RenderTexture.active;
                    RenderTexture.active = rt;

                    inputTex.ReadPixels(new UnityEngine.Rect(0, 0, rt.width, rt.height), 0, 0);
                    if (upload)
                        inputTex.Apply(false);
                    RenderTexture.active = curActive;
                    return inputTex;
                }
                #endregion

                #region GPS Location
                public GPSLocationRes GetGPSLocation()
                {
                    if (accessControlWrapper == null)
                        return default;

                    var passedTest = accessControlWrapper.GetFunction("CheckGetGPSLocation", null);
                    Debug.Log($"[{passedTest}] GPSLocation");
                    if (!passedTest)
                    {
                        //permissionUIs[0].color = COLOR.DeactivatedColor;
                        return default;
                    }

                    //permissionUIs[0].color = COLOR.ActivatedColor;
                    if (!appDataController.IsGPSReady)
                        return default;

                    var gpsData = appDataController.GetAccurateGPSData();
                    return gpsData;
                }
                #endregion
                #endregion
            }
        }
        namespace AccessControl
        {
            public class AccessControlWrapper
            {
                public Assembly assembly = null;
                public Type program = null;
                public object instance = null;
                public AccessControlWrapper() { }
                public AccessControlWrapper(byte[] baseAssemblyBytes, byte[] assemblyBytes, string className, Type[] constructorTypes, object[] constructorArgs)
                {
                    LoadAssembly(baseAssemblyBytes);
                    LoadAssembly(assemblyBytes);
                    LoadProgram(className);
                    LoadInstance(constructorTypes, constructorArgs);
                    Debug.Log($"Assembly loaded");
                }
                public void LoadAssembly(byte[] assemblyBytes)
                {
                    assembly = Assembly.Load(assemblyBytes);
                }
                public void LoadProgram(string className)
                {
                    program = assembly.GetType(className);
                }
                public void LoadInstance(Type[] types, object[] args)
                {
                    var constructor = program.GetConstructor(types);
                    instance = constructor.Invoke(args);
                }
                public bool GetFunction(string functionName, object[] args)
                {
                    var function = program.GetMethod(functionName);
                    if (function == null)
                    {
                        //Debug.Log($"Function {functionName} not found");
                        return false;
                    }
                    var returnVal = (bool)function.Invoke(instance, args);
                    return returnVal;
                }
                //public object GetField(string fieldName)
                //{
                //    var fieldInfo = program.GetField(fieldName);
                //    return fieldInfo.GetValue(instance);
                //}
                //public void SetField(string fieldName, object arg)
                //{
                //    var fieldInfo = program.GetField(fieldName);
                //    fieldInfo.SetValue(instance, arg);
                //}
            }
        }
        namespace Data
        {
            //Language manager app will be able to invoke the functions below
            public class DataEntryPoint
            {
                public static DataEntryPoint Instance
                {
                    get
                    {
                        if (instance == null)
                            instance = new DataEntryPoint();
                        return instance;
                    }
                }
                private static DataEntryPoint instance = null;
                public static bool WasInstantiated => (instance != null);

                private DataEntryPoint() { }
                public void Dispose()
                {
                    instance = null;
                }
                private MonoBehaviour monoBehaviourObj = null;
                private ErebusARManager mainManager = null;

                public bool IsGPSReady { get; set; } = false;


                public IDetectionResult curFrameDetectionResult = null;
                public Texture2D curFrameRawTexture = null;
                private Networking.NetworkCommunicator comm = null;
                private string serverIpAddress = null;
                private string serverPort = null;
                public string CurUserFaceId = null;

                private float gpsUpdateInterval = 5.0f;//in seconds

                public class CameraData
                {
                    public Texture texture { get; set; }
                    public MLFeature imageFeature { get; set; }
                }
                private MLModelData modelData = null;
                private MLModel model;
                private YOLOXPredictor predictor;

                public void Initialize(float gpsUpdateInterval, ErebusARManager mainManager, MonoBehaviour monoBehaviourObj)
                {
                    this.gpsUpdateInterval = gpsUpdateInterval;
                    this.mainManager = mainManager;
                    this.monoBehaviourObj = monoBehaviourObj;

                    InitializePredictor();
                    InitializeLocationSensor();
                }
                private async void InitializePredictor()
                {
                    // Fetch the YOLOX model data
                    modelData = await MLModelData.FromHub("@natsuite/yolox");

                    // Create the model
                    model = modelData.Deserialize();
                    //model = new MLEdgeModel(modelData);

                    // Create the YOLOX predictor
                    predictor = new YOLOXPredictor(model, modelData.labels);
                }
                private void InitializeLocationSensor()
                {
                    NativeGPSPlugin.StartLocation();
                    Input.location.Start();
                    Input.compass.enabled = true;
                    monoBehaviourObj.StartCoroutine(GPS.GpsSensorUpdateLoop(gpsUpdateInterval));
                    monoBehaviourObj.StartCoroutine(CheckGPSInitialized());
                }
                public IEnumerator CheckGPSInitialized()
                {
                    yield return new WaitUntil(() => GPS.CurrentLocationInfo.TrackingState == GPSState.Good);
                    IsGPSReady = true;
                }
                private Texture2D GetRawCameraFrameData()
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    if ((mainManager.ARCameraDataQueue != null) && (!mainManager.ARCameraDataQueue.TryDequeue(out curFrameRawTexture)))
                        return null;
#endif
                    return curFrameRawTexture;
                }

                public string GetAppName()
                {
                    var appName = Application.productName;
                    return appName;
                }
                public string GetCompanyName()
                {
                    var compName = Application.companyName;
                    return compName;
                }
                public Vector2 GetCurrentLocation()
                {
                    var curLocation = new Vector2((float)GPS.CurrentLocationInfo.Latitude, (float)GPS.CurrentLocationInfo.Longitude);
                    return curLocation;
                }
                public GPSLocationRes GetAccurateGPSData()
                {
                    return GPS.CurrentLocationInfo;
                }
                public DateTime GetCurrentTime(bool isRaw = false)
                {
                    var curTime = DateTime.Now;
                    if (isRaw)
                        return curTime;
                    else
                        return DateTime.Parse(curTime.ToString("HH:mm"));
                }
                public string GetCurrentFaceId()
                {
                    return CurUserFaceId;
                }
                private (Texture, Bbox[], string[]) Detect(CameraData curCameraData)
                {
                    // Detect objects
                    (var detections, var labels) = predictor.Predict2(curCameraData.imageFeature);
                    return (curCameraData.texture, detections, labels);
                }
                public IDetectionResult GetCurrentCameraFrame()
                {
                    var rawTexture = GetRawCameraFrameData();
                    if (rawTexture == null)
                        return null;

                    // Create image feature
                    var inputFeature = new MLImageFeature(rawTexture);
                    (inputFeature.mean, inputFeature.std) = modelData.normalization;
                    inputFeature.aspectMode = modelData.aspectMode;
                    var camData = new CameraData()
                    {
                        texture = rawTexture,
                        imageFeature = inputFeature
                    };

                    //Run Object detection & Tracker & Conflation
                    (var curTexture, var curDetBoxes, var curDetLabels) = Detect(camData);

                    curFrameDetectionResult = new Data.DetectionResult() { Data = new List<IMetadata>() };
                    for (int index = 0; index < curDetBoxes.Length; index++)
                    {
                        var bbox = curDetBoxes[index];
                        var label = curDetLabels[index];
                        curFrameDetectionResult.Data.Add(new Data.Metadata
                        {
                            Bbox = new int[] { bbox.X, bbox.Y, bbox.W, bbox.H },
                            Label = label
                        });
                    }

                    return curFrameDetectionResult;
                }

                public List<string> GetTrustedAppNames()
                {
                    var data = DataController.Instance.GetAttrData("Application Name").trustedEntities;
                    var trustedAppNames = new List<string>();
                    foreach (var item in data)
                        trustedAppNames.Add(item.data);

                    return trustedAppNames;
                }
                public List<string> GetTrustedCompanyNames()
                {
                    var data = DataController.Instance.GetAttrData("Company Name").trustedEntities;
                    var trustedCompNames = new List<string>();
                    foreach (var item in data)
                        trustedCompNames.Add(item.data);

                    return trustedCompNames;
                }
                public string GetTrustedLocation(string searchTag)
                {
                    searchTag = searchTag.Trim().ToLower();
                    foreach (var trustedEntity in DataController.Instance.GetAttrData("Location").trustedEntities)
                    {
                        if (trustedEntity.tag.Trim().ToLower() == searchTag)
                        {
                            return trustedEntity.data;
                        }
                    }
                    return null;
                }
                public string GetValidHour(string searchTag)
                {
                    searchTag = searchTag.Trim().ToLower();
                    foreach (var trustedEntity in DataController.Instance.GetAttrData("Time").trustedEntities)
                    {
                        if (trustedEntity.tag.Trim().ToLower() == searchTag)
                        {
                            return trustedEntity.data;
                        }
                    }
                    return null;
                }
                public string GetTrustedFaceId(string searchTag)
                {
                    searchTag = searchTag.Trim().ToLower();
                    foreach (var trustedEntity in DataController.Instance.GetAttrData("Face ID").trustedEntities)
                    {
                        if (trustedEntity.tag.Trim().ToLower() == searchTag)
                        {
                            return trustedEntity.tag;
                        }
                    }
                    return null;
                }
            }
            public static class GPS
            {
                public enum Status
                {
                    Ready,
                    Processing,
                    Complete,
                    Error,
                };
                public static Status status = Status.Ready;
                public static GPSLocationRes CurrentLocationInfo { get; set; } = default;
                public const double LocRadiusBound = 1;//in Kilometer
                                                       //public static string geoID;//(lat-lon-radius)
                private const int R = 6371; // km (Earth radius)
                                            //Reference : https://stackoverflow.com/questions/17787235/creating-a-method-using-haversine-formula-android-v2

                private const double latLongSmoothingFactor = 0.000025; //2.5 meters
                private const float trueHeadingSmoothingFactor = 0.5f;
                private static double lastLat = 0;
                private static double lastLong = 0;
                private static float lastTrueHeading = 0;

                public static IEnumerator GpsSensorUpdateLoop(float gpsUpdateInterval)
                {
                    while (true)
                    {
                        (var accuracy, var state) = GetState();
                        var curTrueHeading = GetBearing();
                        (var lat, var lon) = GetLatLong();

                        if (state == GPSState.Good || CurrentLocationInfo.Equals(default))
                        {
                            CurrentLocationInfo = new GPSLocationRes()
                            {
                                TrackingState = state,
                                Latitude = lat,
                                Longitude = lon,
                                Accuracy = accuracy,
                                //Bearing : CW
                                //0 (or 360 Deg.) : North, +90 : East, 180 : South, 270 : West
                                TrueHeading = curTrueHeading
                            };
                            yield return new WaitForSeconds(gpsUpdateInterval);
                        }
                        else
                            yield return new WaitForSeconds(2.5f);
                    }
                }
                private static (double, double) GetLatLong()
                {
                    var latitude = NativeGPSPlugin.GetLatitude();
                    var longitude = NativeGPSPlugin.GetLongitude();

                    if (lastLat < latitude - latLongSmoothingFactor || lastLat > latitude + latLongSmoothingFactor)
                        lastLat = latitude;

                    if (lastLong < longitude - latLongSmoothingFactor || lastLong > longitude + latLongSmoothingFactor)
                        lastLong = longitude;

                    return (lastLat, lastLong);
                }
                private static (float, GPSState) GetState()
                {
                    var accuracy = NativeGPSPlugin.GetAccuracy(); //in meters
                    var state = GPSState.None; // (accuracy == 0)
                    if (accuracy > 50) //Home : 6meters, NCS : 50meters
                        state = GPSState.Bad;
                    else if (accuracy > 0)
                        state = GPSState.Good;

                    return (accuracy, state);
                }
                private static float GetBearing()
                {
                    float curTrueHeading = Input.compass.trueHeading;
                    if (lastTrueHeading < curTrueHeading - trueHeadingSmoothingFactor || lastTrueHeading > curTrueHeading + trueHeadingSmoothingFactor)
                        lastTrueHeading = curTrueHeading;
                    return lastTrueHeading;
                }

                //public static IEnumerator GetLocation()
                //{
                //    //Debug.Log("__________________________________________________GET GPS");
                //    status = Status.Processing;
                //    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                //        Permission.RequestUserPermission(Permission.FineLocation);
                //    if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
                //        Permission.RequestUserPermission(Permission.CoarseLocation);

                //    // First, check if user has location service enabled
                //    if (!Input.location.isEnabledByUser)
                //        yield return new WaitForSeconds(10);

                //    // Start service before querying location
                //    Input.location.Start();

                //    // Wait until service initializes
                //    int maxWait = 10;
                //    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                //    {
                //        yield return new WaitForSeconds(1);
                //        maxWait--;
                //    }

                //    // Service didn't initialize in 20 seconds
                //    if (maxWait < 1)
                //    {
                //        Debug.LogError("Timed out");
                //        status = Status.Error;
                //        yield break;
                //    }

                //    // Connection has failed
                //    if (Input.location.status == LocationServiceStatus.Failed)
                //    {
                //        Debug.LogError("Unable to determine device location");
                //        status = Status.Error;
                //        yield break;
                //    }
                //    else
                //    {
                //        // Access granted and location value could be retrieved
                //        locationInfo = Input.location.lastData;
                //        //geoID = $"{locationInfo.latitude}-{locationInfo.longitude}-{gpsRadius}";
                //        //Debug.LogError($"[[[Location1]]] : {locationInfo.latitude}, {locationInfo.longitude}");
                //        status = Status.Complete;
                //    }
                //    Input.location.Stop();
                //}
                public static bool IsWithinRadius(Vector2 cmpLocation, Vector2 inputLocation)
                {
                    //GoogleMap/Geofence uses Haversine formula to calculate the dist btw two lat/long points
                    double distance = CalcHaversineDistance(cmpLocation, inputLocation);
                    //Only allows within the GPS radius (Default 1Km)
                    //Debug.Log($"[GPS] {distance}");

                    return distance <= LocRadiusBound;
                }
                private static double CalcHaversineDistance(Vector2 latLong1, Vector2 latLong2)
                {
                    double lat1 = latLong1.x;
                    double long1 = latLong1.y;
                    double lat2 = latLong2.x;
                    double long2 = latLong2.y;

                    double dLat = ToRadian(lat2 - lat1);
                    double dLon = ToRadian(long2 - long1);
                    lat1 = ToRadian(lat1);
                    lat2 = ToRadian(lat2);

                    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                            Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    return R * c;
                }
                private static double ToRadian(double deg)
                {
                    return deg * (Math.PI / 180);
                }
            }
            public class DetectionResult : IDetectionResult
            {
                public DetectionResult() { }
                public List<IMetadata> Data { get; set; }
            }
            public class Metadata : IMetadata
            {
                //[[id, label, score, bbox(x, y, w, h), mask]]
                public int Id { get; set; } //Instance ID
                public string Label { get; set; }
                public float Score { get; set; }
                public int[] Bbox { get; set; }
                public bool[] Mask { get; set; }
                //x0 = int (bbox[0])
                //y0 = int(bbox[1])
                //x1 = int (x0 + bbox[2])
                //y1 = int (y0 + bbox[3])
                public Metadata() { }
            }
        }
        namespace UI
        {
            public enum WaitState
            {
                Block = 0,
                Idle = 1,
                Terminate = 2
            }
            public class PopUpController
            {
                private GameObject popUpCanvas;
                private GameObject popUpGobj;
                private Text popUpInfoText;
                private MonoBehaviour monohavior;
                private readonly Dictionary<string, string[]> popUpFixedTexts = new Dictionary<string, string[]>()
                    {
                        { "Application Name", new string[] { "Current product <b>'", "'</b> is <b>not</b> recognized as trusted product. Please reconfigure <b>Trusted Application Names</b> attribute in the Settings" } },
                        { "Company Name", new string[] { "Current company <b>'", "'</b> is <b>not</b> recognized as trusted company. Please reconfigure <b>Trusted Company Names</b> attribute in the Settings" } },
                        { "Time", new string[] { "Current time <b>'", "'</b> is <b>not</b> within valid hours. Please reconfigure <b>Valid Hours</b> in the Settings" } },
                        { "Location", new string[] { "Current location <b>'", "'</b> is <b>not</b> recognized as trusted location. Please reconfigure <b>Trusted Locations</b> attribute in the Settings" } },
                        { "Face ID", new string[] { "Current face is <b>not</b> recognized as trusted face. Please try again or reconfigure <b>Trusted Face IDs</b>  in the Settings" } },
                    };
                public WaitState WaitStatus { get; set; } = WaitState.Idle;

                private Data.DataController dataController;
                private Data.DataEntryPoint appDataController;

                private bool hasCheckedFaceId = false;
                private bool hasClearFaceIdCheck = false;
                private string curUserFaceId = "";

                public bool IsPermissionCleared { get; private set; } = false;

                public PopUpController(GameObject popUpCanvas, MonoBehaviour monohavior)
                {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    var faceController = UnityEngine.Object.FindObjectOfType<FaceController>(true);
                    if (faceController)
                    {
                        hasCheckedFaceId = true;
                        hasClearFaceIdCheck = FaceController.Instance.HasClearFaceIdCheck;
                        curUserFaceId = FaceController.Instance.FinalEntityTag;
                        UnityEngine.Object.Destroy(faceController.gameObject);
                    }
                    else
                    {
                        hasCheckedFaceId = false;
                        hasClearFaceIdCheck = false;
                    }

                    this.monohavior = monohavior;
                    this.popUpCanvas = UnityEngine.Object.Instantiate(popUpCanvas);
                    popUpInfoText = this.popUpCanvas.GetComponentsInChildren<Text>(true).Where(
                        obj => obj.gameObject.name == "Information Text").ToList()[0];
                    popUpInfoText.supportRichText = true;

                    popUpGobj = this.popUpCanvas.GetComponentsInChildren<Image>(true).Where(
                        obj => obj.gameObject.name == "Pop-up menu").ToList()[0].gameObject;

                    HidePopUp();
                    var buttons = this.popUpCanvas.transform.GetComponentsInChildren<Button>(true);
                    foreach (var button in buttons)
                    {
                        var buttonName = button.name;
                        switch (buttonName)
                        {
                            case "Confirm Button":
                                button.onClick.AddListener(() => OnConfirmButtonPress());
                                break;
                                //case "Allow Button":
                                //    button.onClick.AddListener(() => OnAllowButtonPress());
                                //    break;
                                //case "Deny Button":
                                //    button.onClick.AddListener(() => OnDenyButtonPress());
                                //    break;
                        }
                    }
                }
                public void Initialize(Data.DataController dataController, Data.DataEntryPoint appDataController)
                {
                    this.dataController = dataController;
                    this.appDataController = appDataController;
                    var curAppName = this.appDataController.GetAppName();
                    monohavior.StartCoroutine(WaitForGpsSensorAndExecCallback(CheckAttrPermission, curAppName));
                }
                public void ShowPopUp(Dictionary<string, object[]> attrNameVals)
                {
                    monohavior.StartCoroutine(ShowPopUpOneByOne(attrNameVals));
                }
                private IEnumerator ShowPopUpOneByOne(Dictionary<string, object[]> attrNameVals)
                {
                    var terminateAfterPopup = false;
                    foreach (var attrName in attrNameVals.Keys)
                    {
                        var attrVal = (string)attrNameVals[attrName][0];
                        var rawAttrVal = (string)attrNameVals[attrName][1];

                        if (IsInTrustedAttrList(attrName, rawAttrVal))
                            continue;

                        UpdatePopUpInfoText(attrName, attrVal);
                        popUpGobj.SetActive(true);
                        WaitStatus = WaitState.Block;
                        yield return new WaitUntil(() => WaitStatus >= WaitState.Idle);
                        if (WaitStatus == WaitState.Terminate)
                            terminateAfterPopup = true;
                        //else if (waitState == WaitState.Idle) //Add attribute to the Trusted list
                        //    Data.DataController.Instance.AddTrustedEntity(attrName, attrVal, rawAttrVal);
                    }
                    //HidePopUp();
                    if (terminateAfterPopup)
                        monohavior.StartCoroutine(TimedApplicationQuit(1.0f));
                    else
                        IsPermissionCleared = true;
                    //if (waitState == WaitState.Idle)
                    //    IsAppAccessGranted = true;
                    yield return new WaitUntil(() => IsPermissionCleared);
                    appDataController.CurUserFaceId = curUserFaceId;
                    HidePopUpCanvas();
                    Screen.orientation = ScreenOrientation.AutoRotation;
                }
                private IEnumerator TimedApplicationQuit(float seconds)
                {
                    yield return new WaitForSeconds(seconds);
                    Application.Quit();
                }
                private bool IsInTrustedAttrList(string attrName, string rawAttrVal)
                {
                    //attrVal : Current application's information
                    //(Application name of this app, current time, etc)
                    var attrData = Data.DataController.Instance.GetAttrData(attrName);

                    foreach (var trustedEntity in attrData.trustedEntities)
                    {
                        if (attrName == "Time")
                        {
                            var curTime = DateTime.Parse(rawAttrVal);
                            var cmpTimes = trustedEntity.data.Split(',');
                            var startTime = DateTime.Parse(cmpTimes[0]);
                            var endTime = DateTime.Parse(cmpTimes[1]);

                            if ((curTime >= startTime) && (curTime <= endTime))
                            {
                                WaitStatus = WaitState.Idle;
                                return true;
                            }
                        }
                        else if (attrName == "Location")
                        {
                            var rawCurLoc = rawAttrVal.Split(',');
                            var curLoc = new Vector2(float.Parse(rawCurLoc[0]), float.Parse(rawCurLoc[1]));
                            var rawCmpLoc = trustedEntity.data.Split(',');
                            var cmpLoc = new Vector2(float.Parse(rawCmpLoc[0]), float.Parse(rawCmpLoc[1]));

                            if (Data.GPS.IsWithinRadius(cmpLoc, curLoc))
                            {
                                WaitStatus = WaitState.Idle;
                                return true;
                            }
                        }
                        else //Application Name, Company Name
                        {
                            if (trustedEntity.data == rawAttrVal)
                            {
                                WaitStatus = WaitState.Idle;
                                return true;
                            }
                        }
                    }
                    return false;
                }
                public void HidePopUpCanvas()
                {
                    popUpCanvas.SetActive(false);
                }
                public void HidePopUp()
                {
                    popUpGobj.SetActive(false);
                }
                public void OnConfirmButtonPress()
                {
                    WaitStatus = WaitState.Terminate;
                    Debug.Log("Confirm");
                }
                //public void OnAllowButtonPress()
                //{
                //    waitState = WaitState.Idle;
                //    Debug.Log("Allow");
                //}
                //public void OnDenyButtonPress()
                //{
                //    waitState = WaitState.Terminate;
                //    Debug.Log("Deny");
                //}
                public void UpdatePopUpInfoText(string attrName, string attrValue)
                {
                    var fixedTexts = popUpFixedTexts[attrName];
                    var finalInfoText = "";

                    int fixedTextsIndex = 0;
                    for (; fixedTextsIndex < fixedTexts.Length - 1; fixedTextsIndex++)
                    {
                        var fixedText = fixedTexts[fixedTextsIndex];
                        finalInfoText += fixedText;
                        finalInfoText += attrValue;
                    }
                    finalInfoText += fixedTexts[fixedTextsIndex];

                    popUpInfoText.text = finalInfoText;
                }

                public IEnumerator WaitForGpsSensorAndExecCallback(Action<string> callback, string arg)
                {
                    var appData = dataController.GetAppData(arg);
                    //If there is Face ID Check, don't wait for GPS
                    if (appData != null)
                    {
                        if (appData.requestedAttributes.Contains("Face ID") && !hasCheckedFaceId)
                        {
                            appDataController.Dispose();
                            SceneManager.LoadScene("FaceIDCheck");
                        }
                    }
                    yield return new WaitUntil(() => appDataController.IsGPSReady);
                    callback(arg);
                }
                private void CheckAttrPermission(string curAppName)
                {
                    var appData = dataController.GetAppData(curAppName);
                    if (appData != null)
                    {
                        if (appData.requestedAttributes.Contains("Face ID") && !hasCheckedFaceId)
                            SceneManager.LoadScene("FaceIDCheck");
                        else
                            InitPopUpDisplay(appData);
                    }
                }
                private void InitPopUpDisplay(Data.App appData)
                {
                    var rqstAttrDict = new Dictionary<string, object[]>();
                    foreach (var rqstAttr in appData.requestedAttributes)
                        GetValueToAttr(rqstAttr, ref rqstAttrDict);

                    ShowPopUp(rqstAttrDict);
                }
                //Two elements in the 'rqstAttrDict' : First is processed data, second is the raw data
                private void GetValueToAttr(string attrName, ref Dictionary<string, object[]> rqstAttrDict)
                {
                    switch (attrName)
                    {
                        case "Time":
                            var time = appDataController.GetCurrentTime(isRaw: true);
                            rqstAttrDict[attrName] = new object[] { time.ToString("hh:mm tt"), time.ToString() };
                            break;
                        case "Location":
                            var loc = appDataController.GetCurrentLocation();
                            rqstAttrDict[attrName] = new object[] { loc.ToString(), $"{loc.x},{loc.y}" };
                            break;
                        case "Face ID": //Skip FaceID attr
                            break;
                        case "Application Name":
                            var appName = appDataController.GetAppName();
                            rqstAttrDict[attrName] = new object[] { appName, appName };
                            break;
                        case "Company Name":
                            var compName = appDataController.GetCompanyName();
                            rqstAttrDict[attrName] = new object[] { compName, compName };
                            break;
                    }
                }
            }
        }
        namespace Networking
        {
            public class NetworkCommunicator
            {
                private string ipAddr = "*";
                private string portNum = "12345";
                private TimeSpan timeout = default;
                public NetworkCommunicator(TimeSpan timeout, string ipAddr = "*", string portNum = "12345")
                {
                    this.ipAddr = ipAddr;
                    this.portNum = portNum;
                    this.timeout = timeout;
                }
            }
        }
    }
    [Serializable]
    public struct GPSLocationRes
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Accuracy { get; set; }
        public float TrueHeading { get; set; }
        public GPSState TrackingState { get; set; }
    }
    [Serializable]
    public enum GPSState
    {
        None = 0,
        Good = 1,
        Bad = 2
    }
}