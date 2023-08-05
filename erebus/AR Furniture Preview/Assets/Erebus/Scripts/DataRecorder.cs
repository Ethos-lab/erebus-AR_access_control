using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using Newtonsoft.Json;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Erebus
{
    namespace Benchmark
    {
        public class DataRecorder : MonoBehaviour
        {
            public static DataRecorder Instance => instance;
            private static DataRecorder instance = null;

            private List<BenchmarkData> dataList = null;
            private string benchmarkSavePath = null;
            private Stopwatch stopwatch = new Stopwatch();
            [SerializeField] private bool isDataLoadMode = false;
            private static int dataIndexCnt = 0;

            private void Awake()
            {
                if (instance != null && instance != this)
                {
                    Destroy(this.gameObject);
                    return;
                }
#if UNITY_ANDROID && !UNITY_EDITOR
                isDataLoadMode = false;
#endif
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                BetterStreamingAssets.Initialize();

                var dataSaveBasePath = GetBaseDir();
                benchmarkSavePath = Path.Combine(dataSaveBasePath, "BenchmarkData.json");
                dataList = new List<BenchmarkData>();
                if (isDataLoadMode)
                    LoadData();

                Debug.Log($"Data recording (Mode :{isDataLoadMode})");
            }
#if UNITY_ANDROID && !UNITY_EDITOR
            private void OnApplicationPause(bool pause)
            {
                if (!isDataLoadMode && pause)
                    SaveData();
            }
#elif UNITY_EDITOR
            private void OnDestroy()
            {
                if (!isDataLoadMode)
                    SaveData();
            }
#endif
            private void SaveData()
            {
                string json = JsonConvert.SerializeObject(dataList, Formatting.Indented);
                if (File.Exists(benchmarkSavePath))
                {
                    File.Delete(benchmarkSavePath);
                    Debug.Log($"Removing file : {benchmarkSavePath}");
                }
                File.WriteAllText(benchmarkSavePath, json);
                Debug.Log($"File saved : {benchmarkSavePath}");
            }
            private void LoadData()
            {
                if (File.Exists(benchmarkSavePath))
                {
                    var appDataJsonString = File.ReadAllText(benchmarkSavePath);
                    dataList = JsonConvert.DeserializeObject<List<BenchmarkData>>(appDataJsonString);

                    //Print data
                    foreach (var dataItem in dataList)
                    {
                        var index = dataItem.Index;
                        var type = dataItem.DataType;
                        var time = dataItem.Time;
                        var timeSinceLast = dataItem.MillisecSinceLast;
                        var memo = dataItem.Memo;
                        Debug.Log($"[{index}] ({type}) | {time} | {memo}");
                    }
                    Debug.Log($"File loaded : {benchmarkSavePath}");
                }
                else
                    Debug.Log($"File does NOT exist");
            }
            public void AddData(InputType type, string memo = "")
            {
                stopwatch.Stop();
                BenchmarkData data = new BenchmarkData()
                {
                    Index = dataIndexCnt++,
                    DataType = type,
                    Time = DateTime.Now,
                    MillisecSinceLast = stopwatch.Elapsed.TotalMilliseconds,
                    Memo = memo
                };
                dataList.Add(data);
                stopwatch.Restart();
            }
            private string GetBaseDir()
            {
                return UnityEngine.Application.persistentDataPath;
            }
        }
        public class BenchmarkData
        {
            public int Index { get; set; } = -1;
            public InputType DataType { get; set; } = InputType.Undefined;
            public DateTime Time { get; set; } = default;
            public double MillisecSinceLast { get; set; } = 0;
            public string Memo { get; set; } = "";
        }
        public enum InputType
        {
            Undefined = 0,
            AppStart = 1,
            FrameStart = 2,
            FrameEnd = 3,
            ApiStart = 4,
            ApiEnd = 5,
            SerializationStart = 6,
            SerializationEnd = 7,
            DeserializationStart = 8,
            DeserializationEnd = 9,
            CompressionStart = 10,
            CompressionEnd = 11,
            DecompressionStart = 12,
            DecompressionEnd = 13,
            InferenceStart = 14,
            InferenceEnd = 15,
            WhitelistingStart = 16,
            WhitelistingEnd = 17,
            RawCameraDataCaptured = 18,
            RawCameraDataSent = 19,
            AppEnd = 20
        }
    }
}
