//Reference code : https://answers.unity.com/questions/1427629/how-to-save-ando-load-a-json-file-on-android.html
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;

namespace Erebus
{
    namespace AR
    {
        namespace Data
        {
            public class DataController : MonoBehaviour
            {
                public static DataController Instance => instance;
                private static DataController instance = null;

                public bool IsAttrDataLoaded { get { return attrDataDict != null; } }
                private Dictionary<string, App> appDataDict = null;
                private Dictionary<string, Attribute> attrDataDict = null;
                public byte[] AppBaseAssemblyBytes { get; set; } = null;
                public byte[] AppAssemblyBytes { get; set; } = null; //This app's assembly bytes
                //private Dictionary<string, byte[]> appAssemblyDataDict = null;

                private string dataSaveBasePath = null;
                private string appDataSaveFullPath;
                private string attrDataSaveFullPath;

                private string curAppName = null;
                public string MainApplicationSceneName { get; private set; } = null;
                public bool IsConfigAttached { get; set; } = false;
                public ARCameraManager arCameraManager { get; set; } = null;

#if UNITY_ANDROID && !UNITY_EDITOR
                private readonly string appDataSavePath = @"Erebus/AppData.json";
                private readonly string attrDataSavePath = @"Erebus/AttrData.json";
                //private readonly string appDataSavePath = @"com.Yoonsang.ErebusPermissionManager/files/Erebus/AppData.json";
                //private readonly string attrDataSavePath = @"com.Yoonsang.ErebusPermissionManager/files/Erebus/AttrData.json";
#elif UNITY_EDITOR
                private readonly string appDataSavePath = @"Erebus Permission Manager/Erebus/AppData.json";
                private readonly string attrDataSavePath = @"Erebus Permission Manager/Erebus/AttrData.json";
#endif
                private void Awake()
                {
                    if (instance != null && instance != this)
                    {
                        Destroy(this.gameObject);
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
                    //var targetString = "/Android/data/";
#elif UNITY_EDITOR
                    var targetString = "Yoonsang/";
#endif
                    //Sharing resources among apps in private internal storage doesn't work
                    //Manually accessing the path of shared external storage:
                    //For logicl external storage (shared internal): Downloads folder in Android
                    //For SD card : storage/0000-0000/Android/data/com.Yoonsang.PermissionTest/files/
                    var rawBasePath = GetBaseDir();//UnityEngine.Application.persistentDataPath;
                    //int searchResIndex = rawBasePath.IndexOf(targetString);
                    //dataSaveBasePath = rawBasePath.Substring(0, searchResIndex + targetString.Length);
                    //appDataSaveFullPath = Path.Combine(dataSaveBasePath, appDataSavePath);
                    //attrDataSaveFullPath = Path.Combine(dataSaveBasePath, attrDataSavePath);

                    appDataSaveFullPath = Path.Combine(rawBasePath, appDataSavePath);
                    attrDataSaveFullPath = Path.Combine(rawBasePath, attrDataSavePath);
                    Debug.Log(appDataSaveFullPath);
                    Debug.Log(attrDataSaveFullPath);

                    curAppName = DataEntryPoint.Instance.GetAppName();
                    MainApplicationSceneName = SceneManager.GetActiveScene().name;
                }
                private void OnApplicationFocus(bool focus)
                {
                    if (focus)
                        LoadData();
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
                public void AddTrustedEntity(string attrName, string attrVal, string rawAttrVal)
                {
                    attrDataDict[attrName].lastModified = DateTime.Now;
                    attrDataDict[attrName].trustedEntities.Add(new TrustedEntity(attrVal, "", rawAttrVal));
                }
                public void LoadData()
                {
                    if (appDataDict != null)
                        appDataDict.Clear();
                    else
                        appDataDict = new Dictionary<string, App>();

                    if (attrDataDict != null)
                        attrDataDict.Clear();
                    else
                        attrDataDict = new Dictionary<string, Attribute>();

                    //if (appAssemblyDataDict != null)
                    //    appAssemblyDataDict.Clear();
                    //else
                    //    appAssemblyDataDict = new Dictionary<string, byte[]>();
                    var loadNewBaseAssembly = false;
                    var loadNewDynamicAssembly = false;
                    if (File.Exists(appDataSaveFullPath))
                    {
                        var appDataJsonString = File.ReadAllText(appDataSaveFullPath);
                        var parsedAppData = JsonUtilityWrapper.FromJson<KeyValue<string, App>>(appDataJsonString);

                        //Convert to Dictionary format
                        DateTime appModifiedLatest = default;
                        foreach (var parsedAppDataItem in parsedAppData)
                        {
                            var appName = parsedAppDataItem.name;
                            //Get only the data of this current app
                            if (appName == curAppName)
                            {
                                var appData = parsedAppDataItem.data;
                                appDataDict[appName] = appData;
                                DateTime lastModified = appData.lastModified;
                                appModifiedLatest = appModifiedLatest < lastModified ? lastModified : appModifiedLatest;
                                var baseAssemblyBytes = appDataDict[appName].codeBaseAssemblyBytes;
                                var dynamicAssemblyBytes = appDataDict[appName].codeAssemblyBytes;

                                if (baseAssemblyBytes != null)
                                {
                                    if (AppBaseAssemblyBytes != baseAssemblyBytes)
                                    {
                                        AppBaseAssemblyBytes = baseAssemblyBytes;
                                        loadNewBaseAssembly = true;
                                    }
                                }
                                if (dynamicAssemblyBytes != null)
                                {
                                    if (AppAssemblyBytes != dynamicAssemblyBytes)
                                    {
                                        AppAssemblyBytes = dynamicAssemblyBytes;
                                        loadNewDynamicAssembly = true;
                                    }
                                    //appAssemblyDataDict[appName] = dynamicAssemblyBytes;
                                }

                                //print($"Base::{baseAssemblyBytes.Length}");
                                //print($"Dynamic::{dynamicAssemblyBytes.Length}");

                                //if (baseAssemblyBytes != null)
                                //{
                                //    Assembly.Load(baseAssemblyBytes);
                                //    appAssemblyDataDict[appName] = Assembly.Load(dynamicAssemblyBytes);
                                //}
                            }
                        }

                        ////Print data
                        //foreach (var appDataDictKey in appDataDict.Keys)
                        //{
                        //    var data = appDataDict[appDataDictKey];
                        //    Debug.Log($"{data.langFilePath}, {data.langVoiceCommandInput}, {data.lastModified}");
                        //}
                        Debug.Log($"File loaded : {appDataSaveFullPath}");
                    }

                    if (File.Exists(attrDataSaveFullPath))
                    {
                        var attrDataJsonString = File.ReadAllText(attrDataSaveFullPath);
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
                        Debug.Log($"File loaded : {attrDataSaveFullPath}");
                    }
                    GC.Collect();

                    //if (loadNewBaseAssembly || loadNewDynamicAssembly)
                    if (loadNewDynamicAssembly)
                    {
                        Debug.Log($"Reloaded Assembly ({loadNewBaseAssembly},{loadNewDynamicAssembly})");
                        AR.ARFunctionProvider.FunctionProvider.Instance.LoadWrapper(AppBaseAssemblyBytes, AppAssemblyBytes);
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
                private static string GetBaseDir(bool actualSDCard = false)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (actualSDCard)
                {
                    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
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
                }
                else
                {
                    AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
                    string path = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", jc.GetStatic<string>("DIRECTORY_DOWNLOADS")).Call<string>("getAbsolutePath");
                    return path;
                }
#elif UNITY_EDITOR
                    return UnityEngine.Application.persistentDataPath;
#endif
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
}
