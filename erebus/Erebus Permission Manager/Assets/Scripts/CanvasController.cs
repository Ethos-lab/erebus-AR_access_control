using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Networking;

namespace Erebus
{
    namespace LanguageManager
    {
        public class CanvasController : MonoBehaviour
        {
            public static CanvasController Instance => instance;
            private static CanvasController instance = null;

            [SerializeField] private string IpAddr = "192.168.0.6";
            [SerializeField] private string PortNum = "12349";

            [SerializeField] private GraphicRaycaster canvasRaycaster = null;
            [SerializeField] private EventSystem eventSystem = null;
            private PointerEventData eventPointerSystem = null;

            private DataController dataController = null;
            private SceneController sceneController = null;
            private CodeController codeController = null;
            private NetworkCommunicator comm = null;

            #region API Policy
            [SerializeField] private Text apiModTimeText = null;

            [SerializeField] private Text arNavigationApiModTimeText = null;
            [SerializeField] private Text arFurnitureApiModTimeText = null;
            [SerializeField] private Text arToyMonstersApiModTimeText = null;
            [SerializeField] private Text arFaceFilterApiModTimeText = null;
            [SerializeField] private Text arRemoteMaintenanceApiModTimeText = null;

            [SerializeField] private InputField apiWriteInputField = null;
            [SerializeField] private Text apiVoiceText = null;
            [SerializeField] private string apiCodeString = null;
            private GUIStyle apiCodeAreaGUIStyle = null;
            private Rect apiCodeAreaRect = default;

            [SerializeField] private GameObject voiceCmdStartIcon = null;
            [SerializeField] private GameObject voiceCmdStopIcon = null;

            [SerializeField] private GameObject saveButtonLoadingUI = null;
            private bool showLoadingUI = false;
            #endregion

            #region Policy Attribute
            [SerializeField] private Text attrModTimeText = null;

            [SerializeField] private Text appNameModTimeText = null;
            [SerializeField] private Text compNameModTimeText = null;
            [SerializeField] private Text faceIdModTimeText = null;
            [SerializeField] private Text locationModTimeText = null;
            [SerializeField] private Text timeModTimeText = null;

            [SerializeField] private Text attrNameText = null;
            [SerializeField] private Text attrTypeText = null;
            [SerializeField] private Text attrTrustedText = null;
            [SerializeField] private Text attrUntrustedText = null;
            [SerializeField] private Text attrAddTrustedHeaderText = null;
            [SerializeField] private Text attrAddTrustedButtonText = null;
            [SerializeField] private InputField attrAddInputFieldValue = null;
            [SerializeField] private Text attrAddInputFieldValueText = null;
            [SerializeField] private Text attrAddInputFieldTagText = null;
            private string attrAddInputFieldRawData = null;

            [SerializeField] private GameObject attrTrustedEntityParent = null;
            [SerializeField] private GameObject attrTrustedEntity = null;

            [SerializeField] private GameObject attrAddFaceIdButton = null;
            [SerializeField] private Image addFaceCamTexture = null;
            [SerializeField] private GameObject addFaceIcon = null;
            [SerializeField] private CameraModule cameraModule = null;

            [SerializeField] private GameObject defaultAddValueAreaGobj = null;

            [SerializeField] private GameObject timeFromAttrAddValueAreaGobj = null;
            [SerializeField] private GameObject timeToAttrAddValueAreaGobj = null;

            [SerializeField] private Dropdown attrAddDropDownValueTimeFrom = null;
            [SerializeField] private Dropdown attrAddDropDownValueTimeFromAMPM = null;
            [SerializeField] private Dropdown attrAddDropDownValueTimeTo = null;
            [SerializeField] private Dropdown attrAddDropDownValueTimeToAMPM = null;
            #endregion

            private void Awake()
            {
                if (instance != null && instance != this)
                {
                    Destroy(this.gameObject);
                    return;
                }
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                eventPointerSystem = new PointerEventData(eventSystem);
            }
            private void Start()
            {
                dataController = DataController.Instance;
                sceneController = SceneController.Instance;
                codeController = CodeController.Instance;
                Assert.IsNotNull(dataController, "Data Manager not found");
                Assert.IsNotNull(sceneController, "Scene Manager not found");
                Assert.IsNotNull(codeController, "Code Manager not found");
            }
            public void UpdateUI()
            {
                var curSceneType = sceneController.GetCurrentSceneType();
                var curSelectionName = sceneController.CurSelectedAppOrAttr;
                var curSelectionType = sceneController.CurSelectedType;

                print($"SceneType : {curSceneType}");
                switch (curSceneType)
                {
                    case SceneType.Base:
                        var apiLatestModTime = dataController.ApiPolicyLastModified;
                        var attrLatestModTime = dataController.AttrLastModified;
                        apiModTimeText.text = FormatLastUpdateDateTime(apiLatestModTime);
                        attrModTimeText.text = FormatLastUpdateDateTime(attrLatestModTime);
                        break;
                    case SceneType.PolicyMain:
                        var arNavigationApiModTime = dataController.GetAppData("AR Navigation").lastModified;
                        var arFurnitureApiModTime = dataController.GetAppData("AR Furniture Preview").lastModified;
                        var arToyMonstersApiModTime = dataController.GetAppData("AR Toy Monsters").lastModified;
                        var arFaceFilterApiModTime = dataController.GetAppData("AR Face Filter").lastModified;
                        var arRemoteMaintenanceApiModTime = dataController.GetAppData("AR Remote Maintenance").lastModified;

                        arNavigationApiModTimeText.text = FormatLastUpdateDateTime(arNavigationApiModTime);
                        arFurnitureApiModTimeText.text = FormatLastUpdateDateTime(arFurnitureApiModTime);
                        arToyMonstersApiModTimeText.text = FormatLastUpdateDateTime(arToyMonstersApiModTime);
                        arFaceFilterApiModTimeText.text = FormatLastUpdateDateTime(arFaceFilterApiModTime);
                        arRemoteMaintenanceApiModTimeText.text = FormatLastUpdateDateTime(arRemoteMaintenanceApiModTime);
                        break;
                    case SceneType.PolicyVoice:
                        voiceCmdStartIcon.SetActive(true);
                        voiceCmdStopIcon.SetActive(false);
                        var voiceInputText = dataController.GetAppData(curSelectionName).langVoiceCommandInput;
                        apiWriteInputField.text = voiceInputText;
                        //apiVoiceText.text = voiceInputText;
                        break;
                    case SceneType.PolicyCode:
                        var langFilePath = dataController.GetAppData(curSelectionName).langFilePath;
                        var langText = File.ReadAllText(langFilePath);
                        apiCodeString = langText;
                        break;
                    case SceneType.AttrMain:
                        var appNameModTime = dataController.GetAttrData("Application Name").lastModified;
                        var compNameModTime = dataController.GetAttrData("Company Name").lastModified;
                        var faceIdModTime = dataController.GetAttrData("Face ID").lastModified;
                        var locationModTime = dataController.GetAttrData("Location").lastModified;
                        var timeModTime = dataController.GetAttrData("Time").lastModified;
                        appNameModTimeText.text = FormatLastUpdateDateTime(appNameModTime);
                        compNameModTimeText.text = FormatLastUpdateDateTime(compNameModTime);
                        faceIdModTimeText.text = FormatLastUpdateDateTime(faceIdModTime);
                        locationModTimeText.text = FormatLastUpdateDateTime(locationModTime);
                        timeModTimeText.text = FormatLastUpdateDateTime(timeModTime);
                        break;
                    case SceneType.AttrEdit:
                        var attrName = curSelectionName;
                        var attrType = dataController.GetAttrData(curSelectionName).type;
                        var trustedAttrEntities = dataController.GetAttrData(curSelectionName).trustedEntities;
                        var trustedAttrListParent = attrTrustedEntityParent;
                        var trustedAttrEntity = attrTrustedEntity;

                        attrNameText.text = attrName;
                        attrTypeText.text = $"<{attrType}>";
                        attrTrustedText.text = $"Trusted {curSelectionName}s";
                        attrUntrustedText.text = $"Untrusted {curSelectionName}s";
                        attrAddTrustedButtonText.text = $"Add Another Trusted {attrName}";
                        var entityId = 1;
                        foreach (var trustedEntity in trustedAttrEntities)
                        {
                            var entity = Instantiate(trustedAttrEntity, trustedAttrListParent.transform);
                            entity.SetActive(true);
                            entity.name = "AttrTrustedEntity" + (entityId++);
                            entity.transform.SetSiblingIndex(entityId);
                            var textComponent = entity.GetComponent<Text>();
                            textComponent.text = trustedEntity.formattedData;
                            if (trustedEntity.tag != null && trustedEntity.tag != "")
                                textComponent.text += $" ({trustedEntity.tag})";
                        }
                        break;
                    case SceneType.AttrAdd:
                        attrAddTrustedHeaderText.text = $"Add {curSelectionName}";
                        if (curSelectionName == "Time")
                        {
                            defaultAddValueAreaGobj.SetActive(false);
                            timeFromAttrAddValueAreaGobj.SetActive(true);
                            timeToAttrAddValueAreaGobj.SetActive(true);
                            //var curTime = dataController.GetCurTime;
                            //attrAddInputFieldRawData = curTime.ToString();
                            //attrAddInputFieldValue.text = curTime.ToString("hh:mm tt");
                        }
                        else if (curSelectionName == "Location")
                        {
                            var curLoc = dataController.GetCurLocation;
                            attrAddInputFieldRawData = $"{curLoc.x},{curLoc.y}";
                            attrAddInputFieldValue.text = curLoc.ToString();
                            attrAddInputFieldValue.readOnly = true;
                        }
                        else if (curSelectionName == "Face ID")
                        {
                            attrAddFaceIdButton.SetActive(true);
                            addFaceIcon.SetActive(true);
                            addFaceCamTexture.gameObject.SetActive(false);
                            cameraModule.transform.gameObject.SetActive(true);
                            attrAddInputFieldValue.readOnly = true;
                        }
                        else
                            attrAddInputFieldValue.readOnly = false;
                        break;
                }
            }
            private void OnGUI()
            {
                if (apiCodeString != null)
                {
                    GUILayout.BeginArea(apiCodeAreaRect);
                    apiCodeString = GUILayout.TextArea(apiCodeString, apiCodeAreaGUIStyle);
                    GUILayout.EndArea();
                }
            }
            private void InitApiCodeRectGUIStyle(GameObject canvasWindow, GameObject targetWindow)
            {
                apiCodeAreaGUIStyle = new GUIStyle();
                apiCodeAreaGUIStyle.fontSize = 50;
                apiCodeAreaGUIStyle.fontStyle = FontStyle.Normal;
                apiCodeAreaGUIStyle.normal.textColor = Color.black;
                apiCodeAreaGUIStyle.wordWrap = true;

                var canvasRect = canvasWindow.GetComponent<RectTransform>().rect;
                var windowRect = targetWindow.GetComponent<RectTransform>().rect;

                var pos = windowRect.position - canvasRect.position;
                apiCodeAreaRect = new Rect(pos.x, pos.y, windowRect.width, windowRect.height);
            }
            public void UpdateCanvasGobjReferences()
            {
                UpdateCanvasEventSystem();
                UpdateCanvasRaycaster();

                var canvasGobj = canvasRaycaster.gameObject;
                var texts = canvasGobj.GetComponentsInChildren<Text>(true);
                var textPros = canvasGobj.GetComponentsInChildren<TMP_InputField>(true);
                var vertGroups = canvasGobj.GetComponentsInChildren<VerticalLayoutGroup>(true);
                var horiGroups = canvasGobj.GetComponentsInChildren<HorizontalLayoutGroup>(true);
                var imgs = canvasGobj.GetComponentsInChildren<Image>(true);
                var dropdowns = canvasGobj.GetComponentsInChildren<Dropdown>(true);
                cameraModule = canvasGobj.GetComponentInChildren<CameraModule>(true);

                #region API Policy
                apiModTimeText = FindTargetInChildren(texts, "ApiModTimeText");
                attrModTimeText = FindTargetInChildren(texts, "AttrModTimeText");

                //[SerializeField] private Text arNavigationApiModTimeText = null;
                //[SerializeField] private Text arFurnitureApiModTimeText = null;
                //[SerializeField] private Text arToyMonstersApiModTimeText = null;
                //[SerializeField] private Text arFaceFilterApiModTimeText = null;
                //[SerializeField] private Text arRemoteMaintenanceApiModTimeText = null;

                arNavigationApiModTimeText = FindTargetInChildren(texts, "NavigationApiModTimeText");
                arFurnitureApiModTimeText = FindTargetInChildren(texts, "FurnitureApiModTimeText");
                arToyMonstersApiModTimeText = FindTargetInChildren(texts, "ToyMonstersApiModTimeText");
                arFaceFilterApiModTimeText = FindTargetInChildren(texts, "FaceFilterApiModTimeText");
                arRemoteMaintenanceApiModTimeText = FindTargetInChildren(texts, "RemoteMaintenanceApiModTimeText");

                apiWriteInputField = FindTargetInChildren(imgs, "ApiWriteInputField")?.gameObject.GetComponent<InputField>();

                apiVoiceText = FindTargetInChildren(texts, "ApiVoiceText");
                apiCodeString = null;
                if (apiCodeAreaGUIStyle == null)
                {
                    var apiCodeArea = FindTargetInChildren(imgs, "Code Area")?.gameObject;
                    if (apiCodeArea != null)
                    {
                        InitApiCodeRectGUIStyle(canvasGobj, apiCodeArea);
                    }
                }

                voiceCmdStartIcon = FindTargetInChildren(imgs, "Start Icon")?.gameObject;
                voiceCmdStopIcon = FindTargetInChildren(imgs, "Stop Icon")?.gameObject;
                saveButtonLoadingUI = FindTargetInChildren(imgs, "SaveLoadingUI")?.gameObject;

                #endregion
                #region Attribute
                appNameModTimeText = FindTargetInChildren(texts, "AppNameUsageText");
                compNameModTimeText = FindTargetInChildren(texts, "CompNameUsageText");
                faceIdModTimeText = FindTargetInChildren(texts, "FaceIdUsageText");
                locationModTimeText = FindTargetInChildren(texts, "LocationUsageText");
                timeModTimeText = FindTargetInChildren(texts, "TimeUsageText");

                attrNameText = FindTargetInChildren(texts, "AttrNameText");
                attrTypeText = FindTargetInChildren(texts, "AttrTypeText");
                attrTrustedText = FindTargetInChildren(texts, "AttrTrustedText");
                attrUntrustedText = FindTargetInChildren(texts, "AttrUntrustedText");
                attrAddTrustedHeaderText = FindTargetInChildren(texts, "AttrTrustedHeaderText");
                attrAddTrustedButtonText = FindTargetInChildren(texts, "AttrAddTrustedText");
                attrAddInputFieldValueText = FindTargetInChildren(texts, "AttrAddInputFieldValueText");
                attrAddInputFieldValue = attrAddInputFieldValueText?.GetComponentInParent<InputField>();
                attrAddInputFieldTagText = FindTargetInChildren(texts, "AttrAddInputFieldTagText");

                attrTrustedEntityParent = FindTargetInChildren(vertGroups, "AttrTrustedEntityParent")?.gameObject;
                attrTrustedEntity = FindTargetInChildren(texts, "AttrTrustedEntity")?.gameObject;
                attrAddFaceIdButton = FindTargetInChildren(imgs, "FaceIDInput Button")?.gameObject;
                addFaceCamTexture = FindTargetInChildren(imgs, "CameraTextureArea");
                addFaceIcon = FindTargetInChildren(imgs, "CameraIcon")?.gameObject;

                defaultAddValueAreaGobj = FindTargetInChildren(horiGroups, "Add Value Area")?.gameObject;
                timeFromAttrAddValueAreaGobj = FindTargetInChildren(horiGroups, "Add Value Area (Time-From)")?.gameObject;
                timeToAttrAddValueAreaGobj = FindTargetInChildren(horiGroups, "Add Value Area (Time-To)")?.gameObject;
                attrAddDropDownValueTimeTo = FindTargetInChildren(dropdowns, "AttrAddDropDownValueTimeTo");
                attrAddDropDownValueTimeToAMPM = FindTargetInChildren(dropdowns, "AttrAddDropDownValueTimeToAMPM");
                attrAddDropDownValueTimeFrom = FindTargetInChildren(dropdowns, "AttrAddDropDownValueTimeFrom");
                attrAddDropDownValueTimeFromAMPM = FindTargetInChildren(dropdowns, "AttrAddDropDownValueTimeFromAMPM");
                #endregion

                UpdateUI();
            }
            private string FormatLastUpdateDateTime(DateTime time)
            {
                if (time == default)
                    return "-";
                return time.ToString("hh:mm tt");
            }
            private IEnumerator ShowLoadingIcon()
            {
                showLoadingUI = true;
                saveButtonLoadingUI.SetActive(true);
                var rotateMagnitude = new Vector3(0, 0, -1);
                var startTime = Time.time;
                var curTime = Time.time;
                var minSecs = 2;
                while ((curTime - startTime < minSecs) || showLoadingUI)
                {
                    saveButtonLoadingUI.transform.Rotate(rotateMagnitude);
                    yield return null;
                    curTime = Time.time;
                }
                saveButtonLoadingUI.SetActive(false);
                showLoadingUI = false;
            }
            private T FindTargetInChildren<T>(T[] items, string targetName)
            {
                foreach (dynamic item in items)
                {
                    if (item.gameObject.name == targetName)
                        return item;
                }
                return default(T);
            }
            private void UpdateCanvasRaycaster()
            {
                canvasRaycaster = FindObjectOfType<GraphicRaycaster>();
            }
            private void UpdateCanvasEventSystem()
            {
                eventSystem = FindObjectOfType<EventSystem>();
                eventPointerSystem = new PointerEventData(eventSystem);
            }
            private void Update()
            {
                CheckInteraction();
            }
            private void CheckInteraction()
            {
                //Android's backbutton is the ESC key
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    (var inputType, var response) = sceneController.GetResponse("Back Menu Button");
                    if (inputType == InputType.SceneChange)
                        sceneController.ChangeScene(response);
                }
#if UNITY_ANDROID && !UNITY_EDITOR
                else if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        InteractionAction();
                    }
                }
#elif UNITY_EDITOR
                else if (Input.GetMouseButtonDown(0))
                {
                    InteractionAction();
                }
#endif
            }
            private void InteractionAction()
            {
                if (!showLoadingUI && GetHitGameObject(out GameObject hitGobj))
                {
                    (var inputType, var response) = sceneController.GetResponse(hitGobj.name);
                    if (inputType == InputType.SceneChange) //On any scene changes
                        sceneController.ChangeScene(response);
                    else if (inputType == InputType.SaveAndSceneChange) //Attribute add button 
                    {
                        var curSelectionName = sceneController.CurSelectedAppOrAttr;
                        var formattedData = attrAddInputFieldValueText.text;
                        var tag = attrAddInputFieldTagText.text;
                        string rawData = null;
                        if (curSelectionName == "Application Name" || curSelectionName == "Company Name")
                            rawData = formattedData;
                        else if (curSelectionName == "Time")
                        {
                            var leftTimeIndex = attrAddDropDownValueTimeFrom.value;
                            var leftAMPMIndex = attrAddDropDownValueTimeFromAMPM.value;
                            var lefthandSideTime = attrAddDropDownValueTimeFrom.options[leftTimeIndex].text;
                            var lefthandSideTimeAMPM = attrAddDropDownValueTimeFromAMPM.options[leftAMPMIndex].text;

                            var rightTimeIndex = attrAddDropDownValueTimeTo.value;
                            var rightAMPMIndex = attrAddDropDownValueTimeToAMPM.value;
                            var righthandSideTime = attrAddDropDownValueTimeTo.options[rightTimeIndex].text;
                            var righthandSideTimeAMPM = attrAddDropDownValueTimeToAMPM.options[rightAMPMIndex].text;

                            var leftData = $"{lefthandSideTime} {lefthandSideTimeAMPM}";
                            var rightData = $"{righthandSideTime} {righthandSideTimeAMPM}";

                            formattedData = $"{leftData} ~ {rightData}";
                            rawData = $"{leftData},{rightData}";
                        }
                        else
                            rawData = attrAddInputFieldRawData;
                        dataController.AddAttrTrustedEntity(curSelectionName, new TrustedEntity(formattedData, tag, rawData));
                        attrAddInputFieldRawData = null;
                        //Save data
                        dataController.SaveData();
                        sceneController.ChangeScene(response);
                    }
                    else if (inputType == InputType.Remove) //Attribute remove button (Trash bin icon)
                    {
                        var curSelectionName = sceneController.CurSelectedAppOrAttr;
                        var curSelectionEntity = hitGobj.transform.parent;
                        var entityName = curSelectionEntity.name;
                        var entityIndex = int.Parse(entityName.Replace("AttrTrustedEntity", "").Trim()) - 1;
                        dataController.RemoveAttrTrustedEntity(curSelectionName, entityIndex);
                        //Save data
                        dataController.SaveData();
                        //Refereshing the scene (UI) with the new list of entities
                        sceneController.ChangeScene(response);
                    }
                    else if (inputType == InputType.Save) //Api policy save button (Diskette icon)
                    {
                        StartCoroutine(ShowLoadingIcon());
                        var curSceneType = sceneController.GetCurrentSceneType();
                        if (curSceneType == SceneType.PolicyCode)
                        {
                            var apiCodeStringErebus = apiCodeString;
                            var curSelectionName = sceneController.CurSelectedAppOrAttr;
                            var appData = dataController.GetAppData(curSelectionName);
                            var apiCodeFilePath = appData.langFilePath;

                            //Convert to assembly bytes
                            var apiCodeStringCSharp = codeController.ConvertToCSharpCode(apiCodeStringErebus);
                            print(apiCodeStringCSharp);

                            ////TEMPORALLY FORCING INPUT TO CUSTOM SCRIPT ('OUTPUT.TXT')
                            //var apiCodeStringCSharp = (Resources.Load("output") as TextAsset).text;
                            if (apiCodeStringCSharp != null)
                            {
                                var apiCodeAssemblyBytes = codeController.ConvertToAssemblyBytes(apiCodeStringCSharp);
                                var baseAssembly = codeController.GetBaseAssembly();

                                //Update app data
                                appData.codeBaseAssemblyBytes = baseAssembly;
                                appData.codeAssemblyBytes = apiCodeAssemblyBytes;

                                //Update code (To a file path)
                                File.WriteAllText(apiCodeFilePath, apiCodeStringErebus);

                                //Update used attributes
                                var usedAttr = codeController.GetIncludedAttrNames(apiCodeStringCSharp);
                                appData.requestedAttributes = usedAttr;

                                //Update modified time
                                appData.lastModified = DateTime.Now;

                                //Save data
                                dataController.SaveData();
                            }
                        }
                        else if (curSceneType == SceneType.PolicyVoice)
                        {
                            //var apiVoiceInputString = apiVoiceText.text;
                            var apiVoiceInputString = apiWriteInputField.text;
                            var curSelectionName = sceneController.CurSelectedAppOrAttr;
                            var appData = dataController.GetAppData(curSelectionName);
                            var apiCodeFilePath = appData.langFilePath;

                            //Update app data
                            appData.langVoiceCommandInput = apiVoiceInputString;

                            //Send NL Query to the server & Get Erebus code string from the server
                            if (comm == null)
                                comm = new NetworkCommunicator(TimeSpan.FromSeconds(10), IpAddr, PortNum);
                            var apiCodeStringErebus = comm.SendCode(apiVoiceInputString);
                            //Convert to assembly bytes
                            var apiCodeStringCSharp = codeController.ConvertToCSharpCode(apiCodeStringErebus);
                            if (apiCodeStringCSharp != null)
                            {
                                var apiCodeAssemblyBytes = codeController.ConvertToAssemblyBytes(apiCodeStringCSharp);
                                var baseAssembly = codeController.GetBaseAssembly();

                                //Update app data
                                appData.codeBaseAssemblyBytes = baseAssembly;
                                appData.codeAssemblyBytes = apiCodeAssemblyBytes;

                                //Save erebus language string returned from the server (To a file path)
                                File.WriteAllText(apiCodeFilePath, apiCodeStringErebus);
                                Debug.Log($"CODE SAVED: {apiCodeFilePath}");

                                //Update used attributes
                                var usedAttr = codeController.GetIncludedAttrNames(apiCodeStringCSharp);
                                appData.requestedAttributes = usedAttr;

                                //Update modified time
                                appData.lastModified = DateTime.Now;

                                //Save data
                                dataController.SaveData();
                            }
                        }
                        showLoadingUI = false;
                    }
                    else if (inputType == InputType.Special)
                    {
                        var curSelectionType = sceneController.CurSelectedType;
                        if (curSelectionType == "Application") //Policy (Voice command record button tapped)
                        {
                            ////Please Tap to Start Voice Command
                            //var curMode = voiceCmdStartIcon.activeSelf ? "Idle" : "Recording";
                            //if (curMode == "Idle")
                            //{
                            //    voiceCmdStartIcon.SetActive(false);
                            //    voiceCmdStopIcon.SetActive(true);
                            //}
                            //else
                            //{
                            //    voiceCmdStartIcon.SetActive(true);
                            //    voiceCmdStopIcon.SetActive(false);
                            //}
                        }
                        else //Attribute (Face add/pause button tapped)
                        {
                            var hitGobjName = hitGobj.name;
                            if (hitGobjName == "CameraIcon")
                            {
                                addFaceIcon.SetActive(false);
                                addFaceCamTexture.gameObject.SetActive(true);
                            }
                            else //"FaceBox" hit : During camera texture being displayed
                            {
                                if (cameraModule.IsPaused)
                                    cameraModule.OnPlayButtonClick();
                                else
                                {
                                    cameraModule.OnPauseButtonClick();
                                    var attr = dataController.GetAttrData("Face ID");
                                    var faceId = attr.trustedEntities.Count + 1;
                                    attrAddInputFieldValue.text = $"Face ID #{faceId}";
                                    var faceDataBytes = cameraModule.GetCurrentFaceBytes();
                                    string faceDataString = Convert.ToBase64String(faceDataBytes);
                                    attrAddInputFieldRawData = faceDataString;
                                }
                            }
                        }
                    }
                }
            }
            private bool GetHitGameObject(out GameObject hitGobj)
            {
                var touchPosition = Input.mousePosition;
                //Debug.Log($"Clicked : {touchPosition}");
                var rayHitList = UIHitTest(touchPosition);
                if (rayHitList.Count != 1)
                {
                    hitGobj = null;
                    return false;
                }
                hitGobj = rayHitList[0].gameObject;
                return true;
            }
            private List<RaycastResult> UIHitTest(Vector3 touchPosition)
            {
                eventPointerSystem.position = touchPosition;
                List<RaycastResult> results = new List<RaycastResult>();
                canvasRaycaster.Raycast(eventPointerSystem, results);
                return results;
            }
        }
        public enum InputType
        {
            None = -1,
            SceneChange = 0,
            Save = 1,
            SaveAndSceneChange = 2,
            Remove = 3,
            Special = 4 //Voice record tap, Face Mesh add tap
        }
    }
}
