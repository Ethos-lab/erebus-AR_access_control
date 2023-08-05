using OpenCVFaceTracker;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UtilsModule;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Rect = OpenCVForUnity.CoreModule.Rect;
using Erebus.Benchmark;
using UnityEngine.XR.ARFoundation;
using NatML;
using NatML.Vision;
using Unity.Collections;
using NatML.Features;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Rendering;

public class ARFaceFilter : MonoBehaviour
{
    [SerializeField] private RawImage glassesCanvas;
    [SerializeField] private RawImage mustacheCanvas;
    [SerializeField] private Texture2D rawGlassesTexture;
    [SerializeField] private Texture2D rawMustacheTexture;
    private Texture2D mustacheTexture = null;
    private Texture2D glassesTexture = null;

    private string tracker_model_json_filepath;
    private string haarcascade_frontalface_alt_xml_filepath;
    private FaceTracker faceTracker;
    private FaceTrackerParams faceTrackerParams;
    private CascadeClassifier cascade;

    private Erebus.AR.ARFunctionProvider.FunctionProvider erebusFunctionProvider;

    private void Start()
    {
        erebusFunctionProvider = Erebus.AR.ARFunctionProvider.FunctionProvider.Instance;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        InitializeFaceTracker();
    }
    private void InitializeFaceTracker()
    {
        tracker_model_json_filepath = Utils.getFilePath("tracker_model.json");
        haarcascade_frontalface_alt_xml_filepath = Utils.getFilePath("haarcascade_frontalface_alt.xml");

        //initialize FaceTracker
        faceTracker = new FaceTracker(tracker_model_json_filepath);
        //initialize FaceTrackerParams
        faceTrackerParams = new FaceTrackerParams();

        cascade = new CascadeClassifier();
        cascade.load(haarcascade_frontalface_alt_xml_filepath);
        if (cascade.empty())
        {
            Debug.LogError("cascade file is not loaded.Please copy from FaceTrackerExample/StreamingAssets/�� to ��Assets/StreamingAssets/�� folder. ");
        }
    }
    private void DetectFace(Mat rgbaMat)
    {
        Mat grayMat = new Mat(rgbaMat.rows(), rgbaMat.cols(), CvType.CV_8UC1);
        //convert image to greyscale
        Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_BGRA2GRAY);
        //Imgcodecs.imwrite(Path.Combine(Application.dataPath, "grayMat.jpg"), grayMat);

        //convert image to greyscale
        using (Mat equalizeHistMat = new Mat())
        using (MatOfRect faces = new MatOfRect())
        {
            Imgproc.equalizeHist(grayMat, equalizeHistMat);
            cascade.detectMultiScale(equalizeHistMat, faces, 1.1f, 2, 0
            | Objdetect.CASCADE_FIND_BIGGEST_OBJECT
            | Objdetect.CASCADE_SCALE_IMAGE, new Size(equalizeHistMat.cols() * 0.25, equalizeHistMat.cols() * 0.25), new Size());

            if (faces.rows() > 0)
            {
                List<OpenCVForUnity.CoreModule.Rect> rectsList = faces.toList();
                List<Point[]> pointsList = faceTracker.getPoints();

                //add initial face points from MatOfRect
                if (pointsList.Count <= 0)
                    faceTracker.addPoints(faces);
            }
            else
            {
                glassesCanvas.gameObject.SetActive(false);
                mustacheCanvas.gameObject.SetActive(false);
            }
        }

        try
        {
            //track face points.if face points <= 0, always return false.
            if (faceTracker.track(grayMat, faceTrackerParams))
            {
                //Utils.setDebugMode(true);
                //faceTracker.draw(rgbaMat, new Scalar(255, 0, 0, 255), new Scalar(0, 255, 0, 255));

                Point[] points = faceTracker.getPoints()[0];
                //Imgproc.rectangle(rgbaMat, new Rect(0, 0, 500, 500), new Scalar(255, 255, 255, 255), -1);
                //Imgproc.rectangle(rgbaMat, new Rect(500, 0, 500, 500), new Scalar(255, 0, 0, 255), -1);
                //Imgproc.rectangle(rgbaMat, new Rect(0, 500, 500, 500), new Scalar(0, 255, 0, 255), -1);

                var eyeCenter = (points[31] + points[36]) / 2f;
                var nose = points[67];
                var mouthCenter = (points[48] + points[54]) / 2f;
                var mustache = (nose + mouthCenter) / 2f;

                //Imgproc.circle(rgbaMat, nose, 20, new Scalar(255, 255, 0, 255), 2, Imgproc.LINE_AA, 0);//nose
                //Imgproc.circle(rgbaMat, mouthCenter, 20, new Scalar(0, 255, 255, 255), 2, Imgproc.LINE_AA, 0);//nose
                //Imgproc.circle(rgbaMat, mustache, 20, new Scalar(255, 0, 0, 255), 2, Imgproc.LINE_AA, 0);
                //Imgproc.circle(rgbaMat, eyeCenter, 20, new Scalar(0, 0, 255, 255), 2, Imgproc.LINE_AA, 0);

                //Imgcodecs.imwrite(Path.Combine(Application.persistentDataPath, "here_test.jpg"), rgbaMat);

                Mat mustacheMat = new Mat(rawMustacheTexture.height, rawMustacheTexture.width, CvType.CV_8UC4);
                Mat glassesMat = new Mat(rawGlassesTexture.height, rawGlassesTexture.width, CvType.CV_8UC4);
                Utils.fastTexture2DToMat(rawGlassesTexture, glassesMat);
                Utils.fastTexture2DToMat(rawMustacheTexture, mustacheMat);

                Rect mustacheRoi = new Rect(
                    (int)(mustache.x - rawMustacheTexture.width / 2f),
                    (int)(mustache.y - rawMustacheTexture.height / 2f),
                    rawMustacheTexture.width, rawMustacheTexture.height);
                Rect glassesRoi = new Rect(
                    (int)(eyeCenter.x - rawGlassesTexture.width / 2f),
                    (int)(eyeCenter.y - rawGlassesTexture.height / 2f),
                    rawGlassesTexture.width, rawGlassesTexture.height);

                if (mustacheRoi.size().width <= 0 || mustacheRoi.size().height <= 0 ||
                    glassesRoi.size().width <= 0 || glassesRoi.size().height <= 0 ||
                    mustacheRoi.x <= 0 || mustacheRoi.y <= 0 || glassesRoi.x <= 0 || glassesRoi.y <= 0)
                {
                    faceTracker.reset();
                    glassesCanvas.gameObject.SetActive(false);
                    mustacheCanvas.gameObject.SetActive(false);
                    return;
                }

                var finalMustacheMat = Mat.zeros(rgbaMat.rows(), rgbaMat.cols(), CvType.CV_8UC4);
                mustacheMat.copyTo(finalMustacheMat.submat(mustacheRoi));
                var finalGlassesMat = Mat.zeros(rgbaMat.rows(), rgbaMat.cols(), CvType.CV_8UC4);
                glassesMat.copyTo(finalGlassesMat.submat(glassesRoi));

                if (mustacheTexture == null)
                    mustacheTexture = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);
                if (glassesTexture == null)
                    glassesTexture = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);

                //#if UNITY_EDITOR
                //                if (faceTexture == null)
                //                    faceTexture = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);
                //                Imgproc.cvtColor(rgbaMat, rgbaMat, Imgproc.COLOR_BGRA2RGBA);
                //                Utils.fastMatToTexture2D(rgbaMat, faceTexture);
                //#endif
                //                //faceTracker.draw(finalMustacheMat, new Scalar(255, 0, 0, 255), new Scalar(0, 255, 0, 255));
                Utils.fastMatToTexture2D(finalMustacheMat, mustacheTexture);
                Utils.fastMatToTexture2D(finalGlassesMat, glassesTexture);

                //#if UNITY_EDITOR
                //                faceCanvas.texture = faceTexture;
                //#endif

                mustacheCanvas.GetComponent<RawImage>().texture = mustacheTexture;
                glassesCanvas.GetComponent<RawImage>().texture = glassesTexture;
                //var scale = glassesCanvas.GetComponent<RectTransform>().localScale;
                //glassesCanvas.GetComponent<RectTransform>().localScale = new Vector3(scale.x * 1.5f, scale.y * 1.5f, scale.z);
                //glassesCanvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(-16, -65);
                glassesCanvas.gameObject.SetActive(true);
                mustacheCanvas.gameObject.SetActive(true);
                faceTracker.reset();
                //Utils.setDebugMode(false);
                //#if UNITY_EDITOR
                //                faceTracker.reset();
                //#endif
            }
            else
            {
                glassesCanvas.gameObject.SetActive(false);
                mustacheCanvas.gameObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            glassesCanvas.gameObject.SetActive(false);
            mustacheCanvas.gameObject.SetActive(false);
            if (faceTracker != null)
                faceTracker.reset();
        }
    }
    private void Update()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.orientation = ScreenOrientation.Portrait;

        //Run Object detection & Tracker & Conflation
        var texture = erebusFunctionProvider.GetObjectRawPixels();

        if (texture != null)
        {
            //Application logic (Putting AR Sunglasses + Mustache)
            Mat rawImgMat = new Mat(texture.height, texture.width, CvType.CV_8UC4);
            Utils.fastTexture2DToMat(texture, rawImgMat, flip: true);
            DetectFace(rawImgMat);
        }
    }
    private void OnDisable()
    {
        cascade?.Dispose();
    }
}
