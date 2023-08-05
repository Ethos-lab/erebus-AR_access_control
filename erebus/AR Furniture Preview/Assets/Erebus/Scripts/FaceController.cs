using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Erebus
{
    namespace AR
    {
        namespace UI
        {
            public class FaceController : MonoBehaviour
            {
                public static FaceController Instance => instance;
                private static FaceController instance = null;
                private Data.DataController dataController;
                private FaceRecognize faceCheckModel;

                [SerializeField] private GameObject faceIdCheckUIGobj;
                [SerializeField] private Text faceIdCheckUIText;
                [SerializeField] private CameraModule cameraModule;
                [SerializeField] private GameObject saveButtonLoadingUI;

                private WaitState waitStatus = WaitState.Idle;
                private bool showLoadingUI;

                public bool HasClearFaceIdCheck { get; set; } = false;
                public string FinalEntityTag { get; set; } = "";
                private const int maxCheckAttempt = 5;

                private void Awake()
                {
                    if (instance != null && instance != this)
                    {
                        Destroy(this.gameObject);
                        return;
                    }
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    instance = this;
                    DontDestroyOnLoad(this.gameObject);
                }
                private void Start()
                {
                    saveButtonLoadingUI.SetActive(false);
                    dataController = Data.DataController.Instance;
                    faceCheckModel = new FaceRecognize();
                    faceIdCheckUIText.text = "Please Tap the Box First and Click Confirm";
                    StartCoroutine(WaitForDataLoadAndCheckFaceID());
                }
                private IEnumerator WaitForDataLoadAndCheckFaceID()
                {
                    yield return new WaitUntil(() => dataController.IsAttrDataLoaded);
                    waitStatus = WaitState.Block;
                    faceIdCheckUIGobj.SetActive(true);
                    var checkAttemptCnt = 1;
                    while (waitStatus != WaitState.Idle)
                    {
                        yield return new WaitUntil(() => waitStatus >= WaitState.Idle);
                        if (waitStatus == WaitState.Terminate)
                        {
                            waitStatus = WaitState.Block;
                            Debug.Log($"Failed try again (Attempt {checkAttemptCnt++} out of {maxCheckAttempt})");
                            StartCoroutine(ShowTryAgainOrTerminateApp(2.0f, checkAttemptCnt > maxCheckAttempt));
                            cameraModule.OnPlayButtonClick();
                        }
                    }
                    Debug.Log("Passed");
                    cameraModule.Dispose();
                    this.gameObject.SetActive(false);
                    SceneManager.LoadScene(dataController.MainApplicationSceneName);
                }
                private IEnumerator ShowTryAgainOrTerminateApp(float sec, bool terminateAfter)
                {
                    if (terminateAfter)
                    {
                        faceIdCheckUIText.color = Color.red;
                        faceIdCheckUIText.text = "Too Many Attempts. Please Check Your Settings";
                        yield return new WaitForSeconds(sec);
                        Application.Quit();
                    }
                    else
                    {
                        faceIdCheckUIText.color = Color.red;
                        faceIdCheckUIText.text = "Face ID Not Found Please Try Again";
                        yield return new WaitForSeconds(sec);
                        faceIdCheckUIText.color = Color.white;
                        faceIdCheckUIText.text = "Please Tap the Box First and Click Confirm";
                    }
                }
                public void OnFaceBoxButtonPress()
                {
                    if (cameraModule.IsPaused)
                        cameraModule.OnPlayButtonClick();
                    else
                        cameraModule.OnPauseButtonClick(isFaceId: true);
                }
                public void OnConfirmButtonPress()
                {
                    StartCoroutine(ShowLoadingIcon());
                    new Task(() =>
                    {
                        (var matchFound, var confidence, var entityIndex, var entityTag) = CheckFaceId();
                        Debug.Log($"[{matchFound}] {entityTag} ({confidence})");
                        if (matchFound)
                        {
                            waitStatus = WaitState.Idle;
                            FinalEntityTag = entityTag;
                        }
                        else
                            waitStatus = WaitState.Terminate;
                        showLoadingUI = false;
                    }).Start();
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
                private (bool, double, int, string) CheckFaceId()
                {
                    var curFaceIdMat = cameraModule.RoiSaveMat;
                    //Imgcodecs.imwrite(Path.Combine(Application.persistentDataPath, "input.jpg"), curFaceIdMat);
                    //var curFaceIdMat = Imgcodecs.imread(@"C:\Users\YoonsangKim\AppData\LocalLow\Yoonsang\Erebus Permission Manager\Erebus\output.jpg", Imgcodecs.IMREAD_COLOR);
                    var attrData = dataController.GetAttrData("Face ID");

                    string maxScoreFaceTag = "";
                    double maxScore = 0;
                    int maxScoreIndex = -1;
                    for (int i = 0; i < attrData.trustedEntities.Count; i++)
                    {
                        var trustedEntity = attrData.trustedEntities[i];
                        var trustedFaceIdTag = trustedEntity.tag;
                        var trustedFaceIdStr = trustedEntity.data;
                        var trustedFaceIdMat = cameraModule.ConvertImageStringToMat(trustedFaceIdStr);
                        //Imgcodecs.imwrite(Path.Combine(Application.persistentDataPath, $"cmp{i + 1}.jpg"), trustedFaceIdMat);
                        (var matchFound, var cosScore, var L2score) = faceCheckModel.FaceDetectAndRecognize(curFaceIdMat, trustedFaceIdMat);
                        if (matchFound)
                        {
                            var curScore = CalcFinalScore(cosScore, L2score);
                            if (maxScore < curScore)
                            {
                                maxScore = curScore;
                                maxScoreFaceTag = trustedFaceIdTag;
                                maxScoreIndex = i;
                            }
                        }
                    }

                    return (maxScoreIndex != -1, maxScore, maxScoreIndex, maxScoreFaceTag);
                }
                private double CalcFinalScore(double cosScore, double L2Score)
                {
                    if (cosScore < -0.5) //-0.5 ~ 1(Max)
                        cosScore = -0.5;
                    if (L2Score > 1.5) //0(Max) ~ 1.5
                        L2Score = 1.5;

                    var summedScore = (cosScore + 0.5) + (1.5 - L2Score);
                    return summedScore / 3;
                }
            }
        }
    }
}