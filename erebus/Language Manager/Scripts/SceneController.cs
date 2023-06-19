using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnitySceneManagement = UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Erebus
{
    namespace LanguageManager
    {
        public class SceneController : MonoBehaviour
        {
            public static SceneController Instance => instance;
            private static SceneController instance = null;

            [SerializeField] private string[] sceneNames;
            private string[] accessPolicySceneList;
            private string[] policyAttrSceneList;

            private string[] sceneList = null;
            private int sceneListIndex = 0;
            public string CurSelectedAppOrAttr { get; set; } = null;
            public string CurSelectedType { get; set; } = null;

            private DataController dataController = null;
            private CanvasController canvasController = null;

            private void Awake()
            {
                if (instance != null && instance != this)
                {
                    Destroy(this.gameObject);
                    return;
                }
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                sceneList = sceneNames;
                accessPolicySceneList = new string[]
                { sceneNames[0], sceneNames[1], sceneNames[2], sceneNames[3] };
                policyAttrSceneList = new string[]
                { sceneNames[0], sceneNames[4], sceneNames[5], sceneNames[6] };
            }
            private void Start()
            {
                dataController = DataController.Instance;
                canvasController = CanvasController.Instance;
                Assert.IsNotNull(dataController, "Data Manager not found");
                Assert.IsNotNull(canvasController, "Canvas Manager not found");
                UnitySceneManager.sceneLoaded += OnSceneLoaded;
                //Start( ) is invoked after the scene was loaded,
                //so I need to manually invoke for the first occasion
                canvasController.UpdateCanvasGobjReferences();
            }
            public string GetCurrentScene()
            {
                return sceneList[sceneListIndex];
            }
            public SceneType GetCurrentSceneType()
            {
                var curSceneName = sceneList[sceneListIndex];
                var curSceneIndex = -1;
                for (int index = 0; index < sceneNames.Length; index++)
                {
                    var sceneName = sceneNames[index];
                    if (sceneName == curSceneName)
                    {
                        curSceneIndex = index;
                        break;
                    }
                }
                return (SceneType)curSceneIndex;
            }
            private void UpdateSelectedType()
            {
                if (sceneListIndex == 0)
                    CurSelectedType = null;
                else
                {
                    if (sceneList == accessPolicySceneList)
                        CurSelectedType = "Application";
                    else
                        CurSelectedType = "Attribute";
                }
            }
            private void UpdateSelectedAppOrAttr(string selectedAppOrAttrName)
            {
                var appOrAttrName = selectedAppOrAttrName.Split('(')[1].TrimEnd(')').Trim();
                CurSelectedAppOrAttr = appOrAttrName;
            }
            public (InputType, string) GetResponse(string interactedGobjName)
            {
                var inputType = InputType.SceneChange;
                var prevIndex = sceneListIndex;

                //Base scene (Main Page)
                if (sceneListIndex == 0)
                {
                    if (interactedGobjName != "Back Menu Button")
                    {
                        if (interactedGobjName == "API Access Policy Management")
                            sceneList = accessPolicySceneList;
                        else if (interactedGobjName == "Policy Attribute Management")
                            sceneList = policyAttrSceneList;
                        sceneListIndex++;
                    }
                }
                else
                {
                    if (interactedGobjName == "Back Menu Button")
                    {
                        if (sceneList[sceneListIndex] == sceneNames[3])
                            sceneListIndex--;
                        sceneListIndex--;
                        if (sceneList[sceneListIndex] == sceneNames[1] || sceneList[sceneListIndex] == sceneNames[4])
                            CurSelectedAppOrAttr = null;
                    }
                    else if (interactedGobjName == "Mode Change Button 1") //Voice-cmd-mode to Code-mode
                        sceneListIndex++;
                    else if (interactedGobjName == "Mode Change Button 2") //Code-mode to Voice-cmd-mode
                        sceneListIndex--;
                    else if (interactedGobjName.StartsWith("Application") || interactedGobjName.StartsWith("Attribute"))
                    {
                        UpdateSelectedAppOrAttr(interactedGobjName);
                        sceneListIndex++;
                    }
                    else if (interactedGobjName == "Add Button")
                        sceneListIndex++;
                    else if (interactedGobjName == "Apply Button") //Diskette icon (API Policy's)
                        inputType = InputType.Save;
                    else if (interactedGobjName == "Voice Command Button") //Add button (API Policy's)
                    {
                        inputType = InputType.Special;
                    }
                    else if (interactedGobjName == "CameraIcon" || interactedGobjName == "FaceBox") //Face Add button (Attribute's)
                    {
                        inputType = InputType.Special;
                    }
                    else if (interactedGobjName == "Remove Button") //Trash bin icon (Attribute's)
                        inputType = InputType.Remove;
                    else if (interactedGobjName == "Add Attribute Button") //Add button (Attribute's)
                    {
                        inputType = InputType.SaveAndSceneChange;
                        sceneListIndex--;
                    }
                    else
                    {
                        inputType = InputType.None;
                    }
                }
                print($"HitGobjName : {interactedGobjName}, Index : ({prevIndex},{sceneListIndex}), Input type : {inputType}, Scene name : {sceneList[sceneListIndex]}");

                UpdateSelectedType();

                return (inputType, sceneList[sceneListIndex]);
            }
            public void ChangeScene(string sceneName)
            {
                UnitySceneManager.LoadScene(sceneName);
            }
            private void OnSceneLoaded(UnitySceneManagement.Scene scene, UnitySceneManagement.LoadSceneMode mode)
            {
                canvasController.UpdateCanvasGobjReferences();
            }
        }
        public enum SceneType
        {
            None = -1,
            Base = 0,
            PolicyMain = 1,
            PolicyVoice = 2,
            PolicyCode = 3,
            AttrMain = 4,
            AttrEdit = 5,
            AttrAdd = 6,
            AttrFaceAdd = 7
        }
    }
}
