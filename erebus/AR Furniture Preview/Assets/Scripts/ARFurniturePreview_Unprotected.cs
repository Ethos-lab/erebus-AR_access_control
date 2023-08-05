using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFurniturePreview_Unprotected : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] furniturePrefabs; //0 : Sofa, 1 : Leg rest, 2: Standing light
    [SerializeField] private TMP_Text nameTextUI;
    [SerializeField] private TMP_Text priceTextUI;
    private GameObject[] furnitures;
    private int furnitureIndex = 0;
    private GameObject[] placedFurnitures;
    private Vector2 initialTouchPos = default;
    private Quaternion initialRot = default;
    private const int binSize = 5;
    private bool blockUI = false;
    private void Start()
    {
        furnitures = new GameObject[furniturePrefabs.Length];
        placedFurnitures = new GameObject[furniturePrefabs.Length];
        for (int i = 0; i < furniturePrefabs.Length; i++)
        {
            furnitures[i] = Instantiate(furniturePrefabs[i], null);
            furnitures[i].SetActive(false);
        }
        nameTextUI.text = "CONTEMPORARY STYLE SOFA";
        priceTextUI.text = "299";
    }
    public void OnConfirmButtonPress()
    {
        var newFurniture = Instantiate(furnitures[furnitureIndex], null);
        placedFurnitures[furnitureIndex] = newFurniture;
        newFurniture.transform.position = furnitures[furnitureIndex].transform.position;
        newFurniture.AddComponent<ARAnchor>();
    }
    public void OnHideOrShowAllButtonPress()
    {
        blockUI = !blockUI;
        foreach (var furniture in furnitures)
            furniture.SetActive(!blockUI);
        furnitures[furnitureIndex].transform.rotation = furniturePrefabs[furnitureIndex].transform.rotation;
    }
    public void OnRemoveButtonPress()
    {
        //var anchor = furnitures[furnitureIndex].GetComponent<ARAnchor>();
        if (placedFurnitures[furnitureIndex] != null)
            Destroy(placedFurnitures[furnitureIndex]);
    }
    public void OnNextButtonPress()
    {
        furnitures[furnitureIndex].SetActive(false);
        furnitureIndex = (++furnitureIndex) % furniturePrefabs.Length;
        furnitures[furnitureIndex].transform.rotation = furniturePrefabs[furnitureIndex].transform.rotation;

        switch (furnitureIndex)
        {
            case 0:
                nameTextUI.text = "CONTEMPORARY STYLE SOFA";
                priceTextUI.text = "299";
                break;
            case 1:
                nameTextUI.text = "MODERN SOFA LEG REST";
                priceTextUI.text = "52";
                break;
            case 2:
                nameTextUI.text = "MODERN STANDING LIGHT";
                priceTextUI.text = "199";
                break;
            default:
                Debug.LogError("Unexpected index number");
                break;
        }
    }
    private void Update()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        if (!blockUI)
        {
            var curFurniture = furnitures[furnitureIndex];
            var screenViewportCenter = new Vector2(0.5f, 0.5f);
            var screenCenter = cam.ViewportToScreenPoint(screenViewportCenter);
            var hitRes = new List<ARRaycastHit>();
            //var angle = -999f;
            if (raycastManager.Raycast(screenCenter, hitRes, TrackableType.PlaneWithinPolygon))
            {
                var hitObj = hitRes[0];
                var hitPos = hitObj.pose.position;
                curFurniture.transform.position = hitPos;
                //curFurniture.transform.LookAt(cam.transform);
                //var headingDir = (curFurniture.transform.position - cam.transform.position).normalized;
                //angle = Vector3.SignedAngle(initialForward, headingDir, new Vector3(0, 0, 1));

                //headingArrowMarker.transform.rotation = Quaternion.Euler(initialRotation.x, initialRotation.y, -angle);
                //Debug.DrawRay(deviceNode.transform.position, headingDir * 10, Color.blue, 0.1f);
                curFurniture.SetActive(true);
            }
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
#elif UNITY_ANDROID
            if (Input.touchCount > 0)
#endif
            {
                Vector2 curTouchPos = Input.mousePosition;
                if (initialTouchPos == default)
                {
                    initialTouchPos = curTouchPos;
                    initialRot = curFurniture.transform.rotation;
                }
                else
                {
                    var touchDiff = curTouchPos - initialTouchPos;
                    var screenWidth = Screen.width;
                    var screenHeight = Screen.height;
                    var normalizedHorizontalDiff = touchDiff.x / screenWidth;
                    var normalizedVerticalDiff = touchDiff.y / screenHeight;
                    var newRot = initialRot * Quaternion.Euler(360 * normalizedVerticalDiff, 0, 360 * normalizedHorizontalDiff);
                    var newRotAngles = newRot.eulerAngles / binSize;
                    var newRotAnglesBinned = new Vector3(
                        Mathf.RoundToInt(newRotAngles.x) * binSize,
                        Mathf.RoundToInt(newRotAngles.y) * binSize,
                        Mathf.RoundToInt(newRotAngles.z) * binSize);
                    curFurniture.transform.rotation = Quaternion.Euler(newRotAnglesBinned);
                }
            }
            else
                initialTouchPos = default;
        }
    }
}
