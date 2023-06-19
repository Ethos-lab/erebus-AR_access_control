//Reference code : https://answers.unity.com/questions/1427629/how-to-save-ando-load-a-json-file-on-android.html
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.Android;

namespace Erebus
{
    namespace LanguageManager
    {
        public class DataController : MonoBehaviour
        {
            public static DataController Instance => instance;
            private static DataController instance = null;
            private bool saveOnPause = true;

            private Dictionary<string, App> appDataDict;
            private Dictionary<string, Attribute> attrDataDict;
            public DateTime ApiPolicyLastModified { get; set; } = default;
            public DateTime AttrLastModified { get; set; } = default;

            private string myDataSaveBasePath;
            private string myAppDataSavePath;
            private string myAttrDataSavePath;

            [SerializeField] private string[] policyCodeFileNames = new string[4] 
            { "input1.erebus", "input2.erebus", "input3.erebus", "input4.erebus" };
            private string[] policyCodeFilePaths = new string[4]; //Local storage path

            private SceneController sceneController = null;
            private CanvasController canvasController = null;

            private void Awake()
            {
                if (instance != null && instance != this)
                {
                    Destroy(this.gameObject);
                    saveOnPause = false;
                    return;
                }
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                BetterStreamingAssets.Initialize();
#if UNITY_ANDROID && !UNITY_EDITOR
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                    Permission.RequestUserPermission(Permission.Camera);
#endif
                //Sharing resources among apps in Internal storage doesn't work
                //Manually accessing the path of External sd card : "/storage/0000-0000/Android/data/com.Yoonsang.ErebusPermissionManager/files/"
                myDataSaveBasePath = Path.Combine(GetBaseDir(), "Erebus");

                myAppDataSavePath = Path.Combine(myDataSaveBasePath, "AppData.json");
                myAttrDataSavePath = Path.Combine(myDataSaveBasePath, "AttrData.json");
                for (int i = 0; i < policyCodeFileNames.Length; i++)
                {
                    var policyCodeFileName = policyCodeFileNames[i];
                    policyCodeFilePaths[i] = Path.Combine(myDataSaveBasePath, policyCodeFileName);
                }
                if (!Directory.Exists(myDataSaveBasePath))
                    Directory.CreateDirectory(myDataSaveBasePath);

                LoadSavedData();
                StartCoroutine(InitializeGPS());
                StartCoroutine(GpsLocationUpdate());
            }
            private void Start()
            {
                sceneController = SceneController.Instance;
                canvasController = CanvasController.Instance;
                Assert.IsNotNull(canvasController, "Canvas Manager not found");
                Assert.IsNotNull(sceneController, "Scene Manager not found");
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            private void OnApplicationPause(bool pause)
            {
                if (pause)
                {
                    if (saveOnPause)
                        SaveData();
                }
            }
#elif UNITY_EDITOR
            private void OnDestroy()
            {
                if (saveOnPause)
                    SaveData();
            }
#endif
            public void SetAppData(string appName, App appData = null)
            {
                if (appData == null)
                    appData = new App();
                appDataDict[appName] = appData;
            }
            public void SetAttrData(string attrName, Attribute attrData = null)
            {
                if (attrData == null)
                    attrData = new Attribute();
                attrDataDict[attrName] = attrData;
            }
            public App GetAppData(string appName)
            {
                if (!appDataDict.ContainsKey(appName))
                    return null;
                return appDataDict[appName];
            }
            public Attribute GetAttrData(string attrName)
            {
                if (!attrDataDict.ContainsKey(attrName))
                    return null;
                return attrDataDict[attrName];
            }
            public void AddAttrTrustedEntity(string attrName, TrustedEntity entityData)
            {
                var curAttr = GetAttrData(attrName);
                AttrLastModified = curAttr.lastModified = DateTime.Now;
                curAttr.trustedEntities.Add(entityData);
            }
            public void RemoveAttrTrustedEntity(string attrName, int entityIndex)
            {
                var curAttr = GetAttrData(attrName);
                AttrLastModified = curAttr.lastModified = DateTime.Now;
                curAttr.trustedEntities.RemoveAt(entityIndex);
            }
            private bool isGPSReady = false;
            public Vector2 GetCurLocation => new Vector2(GPS.locationInfo.latitude, GPS.locationInfo.longitude);
            public DateTime GetCurTime => DateTime.Now;
            private IEnumerator InitializeGPS()
            {
                int invokeCnt = 1;
                GPS.status = GPS.Status.Ready;
                StartCoroutine(GPS.GetLocation());
                while (invokeCnt < 3)
                {
                    if (GPS.status != GPS.Status.Processing)
                    {
                        GPS.status = GPS.Status.Processing;
                        StartCoroutine(GPS.GetLocation());
                        invokeCnt++;
                    }
                    yield return null;
                }
                isGPSReady = true;
                Debug.Log($"GPS ready {isGPSReady}");
            }
            private IEnumerator GpsLocationUpdate()
            {
                while (true)
                {
                    //Constantly updating the GPS location
                    if (isGPSReady)
                    {
                        if (GPS.status != GPS.Status.Processing)
                        {
                            GPS.status = GPS.Status.Processing;
                            StartCoroutine(GPS.GetLocation());
                        }
                        yield return new WaitForSeconds(10f);
                    }
                    else
                        yield return new WaitForSeconds(2.5f);
                }
            }
            public void SaveData()
            {
                if (!Directory.Exists(myDataSaveBasePath))
                    Directory.CreateDirectory(myDataSaveBasePath);

                //App data
                List<KeyValue<string, App>> appDataList = new List<KeyValue<string, App>>();
                DateTime appModifiedLatest = default;
                foreach (var appDataKey in appDataDict.Keys)
                {
                    var appData = appDataDict[appDataKey];
                    DateTime appLastModified = appData.lastModified;
                    appModifiedLatest = appModifiedLatest < appLastModified ? appLastModified : appModifiedLatest;
                    appDataList.Add(new KeyValue<string, App>(appDataKey, appData));
                }
                var appDataJsonString = JsonUtilityWrapper.ToJson(appDataList, true);
                File.WriteAllText(myAppDataSavePath, appDataJsonString);
                Debug.Log($"Saved file to : {myAppDataSavePath}");
                ApiPolicyLastModified = appModifiedLatest;

                //Attribute data
                List<KeyValue<string, Attribute>> attrDataList = new List<KeyValue<string, Attribute>>();
                DateTime attrModifiedLatest = default;
                foreach (var attrDataKey in attrDataDict.Keys)
                {
                    var attrData = attrDataDict[attrDataKey];
                    DateTime attrModified = attrData.lastModified;
                    attrModifiedLatest = attrModifiedLatest < attrModified ? attrModified : attrModifiedLatest;
                    attrDataList.Add(new KeyValue<string, Attribute>(attrDataKey, attrData));
                }
                var attrDataJsonString = JsonUtilityWrapper.ToJson(attrDataList, true);
                File.WriteAllText(myAttrDataSavePath, attrDataJsonString);
                Debug.Log($"Saved file to : {myAttrDataSavePath}");
                AttrLastModified = attrModifiedLatest;
            }
            private DateTime LoadMetadata(string dataPath)
            {
                if (File.Exists(dataPath))
                {
                    var metaDataJsonString = File.ReadAllText(dataPath);
                    var parsedMetaData = JsonUtilityWrapper.FromJson<AppMetaData>(metaDataJsonString)[0];
                    return parsedMetaData.lastModifed;
                }
                return default;
            }
            public void LoadSavedData()
            {
                appDataDict = new Dictionary<string, App>();
                attrDataDict = new Dictionary<string, Attribute>();

                //Generating file on local storage (Android local storage) from StreamingAssets dir
                for (int i = 0; i < policyCodeFilePaths.Length; i++)
                {
                    var policyCodeFilePath = policyCodeFilePaths[i];
                    if (!File.Exists(policyCodeFilePath))
                    {
                        SaveFileToAndroidLocalStorage(policyCodeFileNames[i], policyCodeFilePath);
                    }
                }

                if (File.Exists(myAppDataSavePath))
                {
                    var appDataJsonString = File.ReadAllText(myAppDataSavePath);
                    var parsedAppData = JsonUtilityWrapper.FromJson<KeyValue<string, App>>(appDataJsonString);

                    //Convert to Dictionary format
                    DateTime appModifiedLatest = default;
                    foreach (var parsedAppDataItem in parsedAppData)
                    {
                        var appName = parsedAppDataItem.name;
                        var appData = parsedAppDataItem.data;
                        appDataDict[appName] = appData;
                        DateTime lastModified = appData.lastModified;
                        appModifiedLatest = appModifiedLatest < lastModified ? lastModified : appModifiedLatest;
                    }
                    ApiPolicyLastModified = appModifiedLatest;

                    ////Print data
                    //foreach (var appDataDictKey in appDataDict.Keys)
                    //{
                    //    var data = appDataDict[appDataDictKey];
                    //    Debug.Log($"{data.langFilePath}, {data.langVoiceCommandInput}, {data.lastModified}");
                    //}
                    Debug.Log($"File loaded : {myAppDataSavePath}");
                }
                else
                {
                    //Fill up with empty data
                    var curTime = DateTime.Now;
                    SetAppData("IKEA AR Furniture", new App
                    {
                        codeAssemblyBytes = null,
                        langFilePath = Path.Combine(myDataSaveBasePath, "input1.erebus"),
                        langVoiceCommandInput = "",
                        lastModified = curTime
                    }); ;
                    SetAppData("AR Remote Maintenance", new App
                    {
                        codeAssemblyBytes = null,
                        langFilePath = Path.Combine(myDataSaveBasePath, "input2.erebus"),
                        langVoiceCommandInput = "",
                        lastModified = curTime
                    });
                    SetAppData("AR Face Filter", new App
                    {
                        codeAssemblyBytes = null,
                        langFilePath = Path.Combine(myDataSaveBasePath, "input3.erebus"),
                        langVoiceCommandInput = "",
                        lastModified = curTime
                    });
                    SetAppData("AR Toy Car", new App
                    {
                        codeAssemblyBytes = null,
                        langFilePath = Path.Combine(myDataSaveBasePath, "input4.erebus"),
                        langVoiceCommandInput = "",
                        lastModified = curTime
                    });
                    ApiPolicyLastModified = curTime;
                }

                if (File.Exists(myAttrDataSavePath))
                {
                    var attrDataJsonString = File.ReadAllText(myAttrDataSavePath);
                    var parsedAttrData = JsonUtilityWrapper.FromJson<KeyValue<string, Attribute>>(attrDataJsonString);

                    //Convert to Dictionary format
                    DateTime attrModifiedLatest = default;
                    foreach (var parsedAttrDataItem in parsedAttrData)
                    {
                        var attrName = parsedAttrDataItem.name;
                        var attrData = parsedAttrDataItem.data;
                        attrDataDict[attrName] = attrData;
                        DateTime attrModified = attrData.lastModified;
                        attrModifiedLatest = attrModifiedLatest < attrModified ? attrModified : attrModifiedLatest;
                    }
                    AttrLastModified = attrModifiedLatest;

                    ////Print data
                    //foreach (var attrDataDictKey in attrDataDict.Keys)
                    //{
                    //    var data = attrDataDict[attrDataDictKey];
                    //    Debug.Log($"{data.type}, {data.usageCount}");
                    //    foreach (var entity in data.trustedEntities)
                    //    {
                    //        Debug.Log($"{entity.name}, {entity.tag}, {entity.data}");
                    //    }
                    //}
                    Debug.Log($"File loaded : {myAttrDataSavePath}");
                }
                else
                {
                    //Fill up with empty data
                    var curTime = DateTime.Now;
                    SetAttrData("Application Name", new Attribute
                    {
                        type = "String",
                        lastModified = curTime,
                        trustedEntities = new List<TrustedEntity>()
                    });
                    SetAttrData("Company Name", new Attribute
                    {
                        type = "String",
                        lastModified = curTime,
                        trustedEntities = new List<TrustedEntity>()
                    }); ;
                    SetAttrData("Face ID", new Attribute
                    {
                        type = "Face",
                        lastModified = curTime,
                        trustedEntities = new List<TrustedEntity>()
                    });
                    SetAttrData("Location", new Attribute
                    {
                        type = "(Lat,Long)",
                        lastModified = curTime,
                        trustedEntities = new List<TrustedEntity>()
                    });
                    SetAttrData("Time", new Attribute
                    {
                        type = "Time (HH:mm)",
                        lastModified = curTime,
                        trustedEntities = new List<TrustedEntity>()
                    });
                    AttrLastModified = curTime;
                }
            }
            private T ConvertObjectType<T>(object input)
            {
                return (T)Convert.ChangeType(input, typeof(T));
            }
            private void SaveFileToAndroidLocalStorage(string streamingAssetsPath, string androidPath)
            {
                var text = BetterStreamingAssets.ReadAllText(streamingAssetsPath);

                File.WriteAllText(androidPath, text);
                Debug.Log($"File saved to local storage : {androidPath}");
            }
            private static string GetBaseDir()
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                using (AndroidJavaClass unityPlayer =
                       new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject context =
                           unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        // Get all available external file directories (emulated and sdCards)
                        AndroidJavaObject[] externalFilesDirectories =
                                            context.Call<AndroidJavaObject[]>
                                            ("getExternalFilesDirs", (object)null);

                        AndroidJavaObject emulated = null;
                        AndroidJavaObject sdCard = null;

                        for (int i = 0; i < externalFilesDirectories.Length; i++)
                        {
                            AndroidJavaObject directory = externalFilesDirectories[i];
                            using (AndroidJavaClass environment =
                                   new AndroidJavaClass("android.os.Environment"))
                            {
                                // Check which one is the emulated and which the sdCard.
                                bool isRemovable = environment.CallStatic<bool>
                                                  ("isExternalStorageRemovable", directory);
                                bool isEmulated = environment.CallStatic<bool>
                                                  ("isExternalStorageEmulated", directory);
                                if (isEmulated)
                                    emulated = directory;
                                else if (isRemovable && isEmulated == false)
                                    sdCard = directory;
                            }
                        }
                        // Return the sdCard if available
                        if (sdCard != null)
                            return sdCard.Call<string>("getAbsolutePath");
                        else
                            return emulated.Call<string>("getAbsolutePath");
                    }
                }
#elif UNITY_EDITOR
                return UnityEngine.Application.persistentDataPath;
#endif
            }
        }

        [Serializable]
        public class AppMetaData
        {
            public DateTimeWrapper lastModifed; //Only Attribute can be updated in the AR Manager
            public AppMetaData(DateTime lastModifed)
            {
                this.lastModifed = lastModifed;
            }
        }

        [Serializable]
        public class KeyValue<K, V>
        {
            public K name; //Key
            public V data; //Value
            public KeyValue(K name, V data)
            {
                this.name = name;
                this.data = data;
            }
        }

        [Serializable]
        public class App
        {
            public string langFilePath;
            public byte[] codeBaseAssemblyBytes = null;
            public byte[] codeAssemblyBytes = null;
            public List<string> requestedAttributes = null;
            public string langVoiceCommandInput;
            public DateTimeWrapper lastModified;
            public App()
            { }
            public App(string langFilePath, string langVoiceCommandInput, DateTimeWrapper lastModified)
            {
                this.langFilePath = langFilePath;
                this.langVoiceCommandInput = langVoiceCommandInput;
                this.lastModified = lastModified;
            }
        }

        [Serializable]
        public class Attribute
        {
            public string type;
            public DateTimeWrapper lastModified;
            public List<TrustedEntity> trustedEntities;
            public Attribute()
            {
                trustedEntities = new List<TrustedEntity>();
            }
            public Attribute(string type, DateTimeWrapper lastModified = default, List<TrustedEntity> trustedEntities = null)
            {
                this.type = type;
                this.lastModified = lastModified;
                if (trustedEntities == null)
                    trustedEntities = new List<TrustedEntity>();
                this.trustedEntities = trustedEntities;
            }
        }

        [Serializable]
        public class TrustedEntity
        {
            public string formattedData; //Stringified data (Presentable format)
            public string tag;
            public string data; //Raw data
            public TrustedEntity(string formattedData, string tag, string data = null)
            {
                this.formattedData = formattedData;
                this.tag = tag;
                this.data = data;
            }
        }

        [Serializable]
        public struct DateTimeWrapper
        {
            public long value;
            public static implicit operator DateTime(DateTimeWrapper convTime)
            {
                return DateTime.FromFileTime(convTime.value);
            }
            public static implicit operator DateTimeWrapper(DateTime time)
            {
                DateTimeWrapper convTime = new DateTimeWrapper();
                convTime.value = time.ToFileTime();
                return convTime;
            }
        }
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
    public const double gpsRadius = 1;//in Kilometer
                                      //public static string geoID;//(lat-lon-radius)
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
}