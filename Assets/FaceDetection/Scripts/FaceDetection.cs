using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using TMPro;

// 1. Region Pose
public class FaceDetection : MonoBehaviour
{
    List<GameObject> regionFeatures = new List<GameObject>(); // region features
    List<GameObject> totalFeatures = new List<GameObject>(); // total 468 features
    List<TMP_Text> featureTexts = new List<TMP_Text>();
    [SerializeField] GameObject pointPrefab;
    ARCoreFaceSubsystem faceSubsystem;
    ARFaceManager faceManager;
    NativeArray<ARCoreFaceRegionData> faceRegions;

    private void Awake()
    {
        faceManager = GetComponent<ARFaceManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 3; i++) // region features
        {
            GameObject featureObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            featureObj.transform.SetParent(transform);
            featureObj.name = "region point " + i;
            featureObj.transform.localScale = Vector3.one * 0.02f;

            regionFeatures.Add(featureObj);
            featureObj.SetActive(false);
        }

        for (int i = 0; i < 468; i++) // total 468 features
        {
            GameObject featureObj = Instantiate(pointPrefab, transform);
            featureObj.name = "point " + i;

            TMP_Text txt = featureObj.GetComponentInChildren<TMP_Text>(); // obj의 텍스트 가져오기
            txt.text = "0";
            featureTexts.Add(txt);

            totalFeatures.Add(featureObj);
            featureObj.SetActive(false);
        }

        faceSubsystem = (ARCoreFaceSubsystem)faceManager.subsystem;

        faceManager.facesChanged += OnDetectedRegionPos;
        faceManager.facesChanged += OnDetectedTotalFeaturePos;
    }

    void OnDetectedRegionPos(ARFacesChangedEventArgs args)
    {
        if(args.updated.Count > 0)
        {
            faceSubsystem.GetRegionPoses(args.updated[0].trackableId, Unity.Collections.Allocator.Persistent, ref faceRegions);

            // 코(0), 왼쪽이마(1), 오른쪽이마(2)
            for(int i = 0; i < faceRegions.Length; i++)
            {
                regionFeatures[i].transform.position = faceRegions[i].pose.position;
                regionFeatures[i].transform.rotation = faceRegions[i].pose.rotation;
                regionFeatures[i].SetActive(true);
            }
        }
        else if(args.removed.Count > 0)
        {
            for (int i = 0; i < faceRegions.Length; i++)
            {
                regionFeatures[i].SetActive(false);
            }
        }
    }

    void OnDetectedTotalFeaturePos(ARFacesChangedEventArgs args)
    {
        if (args.updated.Count > 0)
        {
            for (int i = 0; i < args.updated[0].vertices.Length; i++)
            {
                Vector3 vertPos = args.updated[0].vertices[i];
                Vector3 worldVertPos = args.updated[0].transform.TransformPoint(vertPos);

                totalFeatures[i].transform.position = worldVertPos;
                totalFeatures[i].SetActive(true);

                featureTexts[i].text = i.ToString();
            }
        }
        else if (args.removed.Count > 0)
        {
            for (int i = 0; i < totalFeatures.Count; i++)
            {
                totalFeatures[i].SetActive(false);
            }
        }
    }
}
