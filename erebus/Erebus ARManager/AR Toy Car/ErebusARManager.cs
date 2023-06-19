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
using MessagePack;
using System.IO.Compression;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

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
            [SerializeField] private string serverIpAddress = "192.168.0.6"; //130.245.4.113 (Lab processing server)
            [SerializeField] private string serverPort = "12345";

            private UI.PopUpController UIControl = null;
            private Data.DataController dataController = null;
            private Data.DataEntryPoint appDataController = null;
            public ARFunctionProvider.FunctionProvider FunctionProvider { get; private set; } = null;

            //Erebus AR Manager (FunctionProvider) private member 
            public ConcurrentQueue<byte[]> ARCameraDataQueue = null;
            private const int cameraDataQueueLimit = 2;
            private bool arCamConfigInitialized = false;

            private void Awake()
            {
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
                FunctionProvider.Initialize(sessionOriginGobj, this);
                appDataController.Initialize(serverIpAddress, serverPort, this, this);
                UIControl.Initialize(dataController, appDataController);
                AttachARCameraConfig();
                Debug.Log("Erebus ARManager Start");
            }
            public void AttachARCameraConfig()
            {
                Debug.Log($"[AttachARCameraConfig]");
                ARCameraDataQueue = new ConcurrentQueue<byte[]>();
                arCameraManager.frameReceived += OnCameraFrameUpdate;

                //Debug.Log($"[AWAKE] {arCamConfigInitialized} {arCameraManager != dataController.arCameraManager}");
                //if (!arCamConfigInitialized)//dataController != null && ((!dataController.IsConfigAttached) || (arCameraManager != dataController.arCameraManager)))
                //{
                //    Debug.Log($"[ACTIVATED] {arCameraManager != dataController.arCameraManager}");

                //    ARCameraDataQueue = new ConcurrentQueue<byte[]>();
                //    arCameraManager.frameReceived += OnCameraFrameUpdate;
                //    dataController.IsConfigAttached = true;
                //    dataController.arCameraManager = arCameraManager;
                //}
            }
            private unsafe void OnCameraFrameUpdate(ARCameraFrameEventArgs eventArgs)
            {
                if (ARCameraDataQueue.Count > cameraDataQueueLimit)
                    return;

                if (!arCamConfigInitialized)
                    InitializeARCamConfigOnFirstFrame(eventArgs);

                if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                    return;

                //Debug.LogError("[IMAGE]" + image.width + "," + image.height);
                var conversionParams = new XRCpuImage.ConversionParams
                {
                    // Get the entire image.
                    inputRect = new RectInt(0, 0, image.width, image.height),
                    outputDimensions = new Vector2Int(image.width, image.height),

                    //X (Left), Y (Top), W, H 
                    //inputRect = new RectInt(0, 0, clientScreenResolution.x, clientScreenResolution.y),

                    //outputDimensions = new Vector2Int(clientScreenResolution.x, clientScreenResolution.y),

                    // Choose RGB format.
                    outputFormat = TextureFormat.RGB24,

                    // Flip across the vertical axis (mirror image).
                    transformation = XRCpuImage.Transformation.None//MirrorY
                };

                // See how many bytes you need to store the final image.
                int size = image.GetConvertedDataSize(conversionParams);

                // Allocate a buffer to store the image.
                var buffer = new NativeArray<byte>(size, Allocator.Temp);

                // Extract the image data
                image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

                // The image was converted to RGBA32 format and written into the provided buffer
                // so you can dispose of the XRCpuImage. You must do this or it will leak resources.
                image.Dispose();

                // At this point, you can process the image, pass it to a computer vision algorithm, etc.
                // In this example, you apply it to a texture to visualize it.

                // You've got the data; let's put it into a texture so you can visualize it.
                //Texture2D m_Texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
                //Debug.Log("TIME1:" + (start_time - Time.time));

                NativeArray<byte> jpgBytes =
                   ImageConversion.EncodeNativeArrayToJPG(buffer, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm,
                   (uint)conversionParams.outputDimensions.x, (uint)conversionParams.outputDimensions.y);

                //NativeArray<byte> pngBytes =
                //    ImageConversion.EncodeNativeArrayToPNG(buffer, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8_UNorm,
                //    (uint)conversionParams.outputDimensions.x, (uint)conversionParams.outputDimensions.y);
                //Debug.Log("TIME2:" + (start_time - Time.time));
                //m_Texture.LoadRawTextureData(pngBytes);
                //m_Texture.Apply();

                // Done with your temporary data, so you can dispose it.
                buffer.Dispose();

                ARCameraDataQueue.Enqueue(jpgBytes.ToArray());
            }
            private void InitializeARCamConfigOnFirstFrame(ARCameraFrameEventArgs eventArgs)
            {
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
                }
                private GameObject sessionOriginGobj;
                private ErebusARPlaneManager erebusARPlaneManager;
                private ErebusARRaycastManager erebusARRaycastManager;
                private Data.DataEntryPoint appDataController;
                private AccessControl.AccessControlWrapper accessControlWrapper = null;
                private MonoBehaviour monoBehaviourObj = null;
                public bool IsFunctionProviderReady { get; private set; } = false;

                public void Initialize(GameObject sessionOriginGobj, MonoBehaviour monoBehaviourObj)
                {
                    this.monoBehaviourObj = monoBehaviourObj;
                    this.sessionOriginGobj = sessionOriginGobj;
                    erebusARPlaneManager = new ErebusARPlaneManager(this.sessionOriginGobj);
                    erebusARRaycastManager = new ErebusARRaycastManager(this.sessionOriginGobj);
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
                        Assert.IsNotNull(PlaneManager);
                        UserEventOnPlanesChange = new List<Action<ARPlanesChangedEventArgs>>();
                        PlaneManager.planesChanged += OnPlanesChanged;
                    }
                    ~ErebusARPlaneManager()
                    {
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
                        Assert.IsNotNull(RaycastManager);
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
                    //Debug.Log($"[{passedTest}] GetPlane");
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

                    var passedTest = accessControlWrapper.GetFunction("CheckGetARRayCast", null);
                    //Debug.Log($"[{passedTest}] Raycast1");
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

                    var passedTest = accessControlWrapper.GetFunction("CheckGetARRayCast", null);
                    //Debug.Log($"[{passedTest}] Raycast2");
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

                        var passedTest = accessControlWrapper.GetFunction("CheckGetPlaneTrackables", null);
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

                #region Object Detection
                public byte[] GetObjectRawPixels()
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
                    var allowedPixels = GetWhitelistedRawPixels();
                    return allowedPixels;
                }
                private byte[] GetWhitelistedRawPixels()
                {
                    //Remove everything except the target, from 'curFrameDetectionResult' data
                    //The remaining data only contains data of target labels
                    Mat summedMask = Mat.zeros(480, 640, CvType.CV_8UC1);

                    appDataController = Data.DataEntryPoint.Instance;
                    foreach (var item in appDataController.curFrameDetectionResult.Data)
                    {
                        Mat mask = new Mat(480, 640, CvType.CV_8UC1);
                        MatUtils.copyToMat(item.Mask, mask);
                        Core.bitwise_or(summedMask, mask, summedMask);
                    }
#if UNITY_ANDROID && !UNITY_EDITOR
                    Mat camRawMat = new Mat(1, appDataController.curFrameRawCamBytes.Length, CvType.CV_8UC1);
                    MatUtils.copyToMat(appDataController.curFrameRawCamBytes, camRawMat);
                    Mat imgMat = Imgcodecs.imdecode(camRawMat, Imgcodecs.IMREAD_COLOR);
                    Core.flip(imgMat, imgMat, 0); //Flip vertically
#elif UNITY_EDITOR
                    var path = @"C:\Users\YoonsangKim\Desktop\TestProject\Assets\Resources\test_640x480.jpg";
                    Mat imgMat = Imgcodecs.imread(path, Imgcodecs.IMREAD_COLOR);
#endif

                    Mat resMat = new Mat();
                    Core.bitwise_and(imgMat, imgMat, resMat, mask: summedMask);

                    var resJpgBytes = new byte[resMat.total() * resMat.channels()];
                    MatUtils.copyFromMat(resMat, resJpgBytes);

                    return resJpgBytes;
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
                public byte[] curFrameRawCamBytes = null;
                private Networking.NetworkCommunicator comm = null;
                private string serverIpAddress = null;
                private string serverPort = null;

                public void Initialize(string serverIpAddress, string serverPort, ErebusARManager mainManager, MonoBehaviour monoBehaviourObj)
                {
                    this.serverIpAddress = serverIpAddress;
                    this.serverPort = serverPort;
                    this.mainManager = mainManager;
                    this.monoBehaviourObj = monoBehaviourObj;
                    InitializeLocationSensor();
                }
                private void InitializeLocationSensor()
                {
                    monoBehaviourObj.StartCoroutine(InitializeGPS());
                    monoBehaviourObj.StartCoroutine(GpsLocationUpdate());
                }
                private IEnumerator InitializeGPS()
                {
                    int invokeCnt = 1;
                    GPS.status = GPS.Status.Ready;
                    monoBehaviourObj.StartCoroutine(GPS.GetLocation());
                    while (invokeCnt < 3)
                    {
                        if (GPS.status != GPS.Status.Processing)
                        {
                            GPS.status = GPS.Status.Processing;
                            monoBehaviourObj.StartCoroutine(GPS.GetLocation());
                            invokeCnt++;
                        }
                        yield return null;
                    }
                    IsGPSReady = true;
                    Debug.Log($"GPS ready {IsGPSReady}");
                }
                private IEnumerator GpsLocationUpdate()
                {
                    while (true)
                    {
                        //Constantly updating the GPS location
                        if (IsGPSReady)
                        {
                            if (GPS.status != GPS.Status.Processing)
                            {
                                GPS.status = GPS.Status.Processing;
                                monoBehaviourObj.StartCoroutine(GPS.GetLocation());
                            }
                            yield return new WaitForSeconds(10f);
                        }
                        else
                            yield return new WaitForSeconds(2.5f);
                    }
                }
                private byte[] GetRawCameraFrameData()
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    if ((mainManager.ARCameraDataQueue != null) && (!mainManager.ARCameraDataQueue.TryDequeue(out curFrameRawCamBytes)))
                        return null;
#elif UNITY_EDITOR
                    var path = @"C:\Users\YoonsangKim\Desktop\TestProject\Assets\Resources\test_640x480.jpg";
                    Mat imgMat = Imgcodecs.imread(path, Imgcodecs.IMREAD_COLOR);
                    curFrameRawCamBytes = new byte[imgMat.total() * imgMat.channels()];
                    MatUtils.copyFromMat(imgMat, curFrameRawCamBytes);
#endif
                    return curFrameRawCamBytes;
                }
                private IDetectionResult ExecRemoteObjectDetector(byte[] imgBytes)
                {
                    if (comm == null)
                        comm = new Networking.NetworkCommunicator(TimeSpan.FromSeconds(10), serverIpAddress, serverPort);
                    if (imgBytes == null)
                        return null;
                    curFrameDetectionResult = comm.SendImgBytes(imgBytes);
                    return curFrameDetectionResult;
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
                    var curLocation = new Vector2(GPS.locationInfo.latitude, GPS.locationInfo.longitude);
                    return curLocation;
                }
                public DateTime GetCurrentTime(bool isRaw = false)
                {
                    var curTime = DateTime.Now;
                    if (isRaw)
                        return curTime;
                    else
                        return DateTime.Parse(curTime.ToString("HH:mm"));
                }
                public object GetCurrentFaceId()
                {
                    return null;
                }
                public IDetectionResult GetCurrentCameraFrame()
                {
                    var imgBytes = GetRawCameraFrameData();
                    return ExecRemoteObjectDetector(imgBytes);
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
                public string GetTrustedFaceIds()
                {
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
                public static LocationInfo locationInfo;
                public const double LocRadiusBound = 1;//in Kilometer
                                                       //public static string geoID;//(lat-lon-radius)
                private const int R = 6371; // km (Earth radius)
                                            //Reference : https://stackoverflow.com/questions/17787235/creating-a-method-using-haversine-formula-android-v2
                public static IEnumerator GetLocation()
                {
                    //Debug.Log("__________________________________________________GET GPS");
                    status = Status.Processing;
                    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                        Permission.RequestUserPermission(Permission.FineLocation);
                    if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
                        Permission.RequestUserPermission(Permission.CoarseLocation);

                    // First, check if user has location service enabled
                    if (!Input.location.isEnabledByUser)
                        yield return new WaitForSeconds(10);

                    // Start service before querying location
                    Input.location.Start();

                    // Wait until service initializes
                    int maxWait = 10;
                    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                    {
                        yield return new WaitForSeconds(1);
                        maxWait--;
                    }

                    // Service didn't initialize in 20 seconds
                    if (maxWait < 1)
                    {
                        Debug.LogError("Timed out");
                        status = Status.Error;
                        yield break;
                    }

                    // Connection has failed
                    if (Input.location.status == LocationServiceStatus.Failed)
                    {
                        Debug.LogError("Unable to determine device location");
                        status = Status.Error;
                        yield break;
                    }
                    else
                    {
                        // Access granted and location value could be retrieved
                        locationInfo = Input.location.lastData;
                        //geoID = $"{locationInfo.latitude}-{locationInfo.longitude}-{gpsRadius}";
                        //Debug.LogError($"[[[Location1]]] : {locationInfo.latitude}, {locationInfo.longitude}");
                        status = Status.Complete;
                    }
                    Input.location.Stop();
                }
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

                public bool IsPermissionCleared { get; private set; } = false;

                public PopUpController(GameObject popUpGobj, MonoBehaviour monohavior)
                {
                    var faceController = UnityEngine.Object.FindObjectOfType<FaceController>(true);
                    if (faceController)
                    {
                        hasCheckedFaceId = true;
                        hasClearFaceIdCheck = FaceController.Instance.HasClearFaceIdCheck;
                        UnityEngine.Object.Destroy(faceController.gameObject);
                    }
                    else
                    {
                        hasCheckedFaceId = false;
                        hasClearFaceIdCheck = false;
                    }

                    this.monohavior = monohavior;
                    this.popUpGobj = UnityEngine.Object.Instantiate(popUpGobj);
                    HidePopUp();
                    popUpInfoText = this.popUpGobj.GetComponentsInChildren<Text>(true).Where(
                        obj => obj.gameObject.name == "Information Text").ToList()[0];
                    popUpInfoText.supportRichText = true;

                    var buttons = this.popUpGobj.transform.GetComponentsInChildren<Button>(true);
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
                    HidePopUp();
                    if (terminateAfterPopup)
                        monohavior.StartCoroutine(TimedApplicationQuit(1.0f));
                    else
                        IsPermissionCleared = true;
                    //if (waitState == WaitState.Idle)
                    //    IsAppAccessGranted = true;
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
                private byte[] Decompress(byte[] inputBytes)
                {
                    using (MemoryStream ouputStream = new MemoryStream())
                    {
                        using (MemoryStream inputStream = new MemoryStream(inputBytes))
                        {
                            using (var decompressor = new GZipStream(inputStream, CompressionMode.Decompress))
                            {
                                decompressor.CopyTo(ouputStream);
                                return ouputStream.ToArray();
                            }
                        }
                    }
                }
                public IDetectionResult SendImgBytes(byte[] imgBytes)
                {
                    AsyncIO.ForceDotNet.Force();

                    using (var socket = new RequestSocket($"tcp://{ipAddr}:{portNum}"))
                    {
                        try
                        {
                            socket.SendFrame(imgBytes);
                            if (socket.TryReceiveFrameBytes(timeout, out byte[] compressedResultBytes))
                            {
                                var decompressedResultBytes = Decompress(compressedResultBytes);
                                var msgPackDetectionResult = MessagePackSerializer.Deserialize<MsgPackDetectionResult>(decompressedResultBytes);
                                var detectionResult = new Data.DetectionResult() { Data = new List<IMetadata>() };
                                foreach (var msgPackItemMetaData in msgPackDetectionResult.Data)
                                {
                                    detectionResult.Data.Add(new Data.Metadata
                                    {
                                        Id = msgPackItemMetaData.Id,
                                        Label = msgPackItemMetaData.Label,
                                        Score = msgPackItemMetaData.Score,
                                        Bbox = msgPackItemMetaData.Bbox,
                                        Mask = msgPackItemMetaData.Mask
                                    });
                                    //Debug.Log($"{msgPackItemMetaData.Id},{msgPackItemMetaData.Label},{msgPackItemMetaData.Score}," +
                                    //    $"{msgPackItemMetaData.Bbox.Length},{msgPackItemMetaData.Mask.Length}," +
                                    //    $" => {detectionResult.Data.Last().Id},{detectionResult.Data.Last().Label}," +
                                    //    $"{detectionResult.Data.Last().Score},{detectionResult.Data.Last().Bbox.Length}," +
                                    //    $"{detectionResult.Data.Last().Mask.Length}");
                                }
                                ////var resultStr = Encoding.UTF8.GetString(resultBytes);
                                ////var detectionResult = JsonConvert.DeserializeObject<DetectionResult>(resultBytes);
                                return detectionResult;
                            }
                            else
                                Debug.Log("Client timeout");
                        }
                        finally
                        {
                            socket.Close();
                            NetMQConfig.Cleanup();
                        }
                    }

                    return null;
                }
            }
            [MessagePackObject(keyAsPropertyName: true)]
            public class MsgPackDetectionResult
            {
                public List<MsgPackDummyMetadata> Data { get; set; }
            }
            [MessagePackObject(keyAsPropertyName: true)]
            public class MsgPackDummyMetadata
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
            }
        }
    }
}