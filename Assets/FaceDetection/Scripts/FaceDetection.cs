using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System;

// 1. Region Pose
public class FaceDetection : MonoBehaviour
{
    [Header("Feature Indication")]
    List<GameObject> regionFeatures = new List<GameObject>(); // region features
    List<GameObject> totalFeatures = new List<GameObject>(); // total 468 features
    List<TMP_Text> featureTexts = new List<TMP_Text>();
    [SerializeField] GameObject pointPrefab;
    ARCoreFaceSubsystem faceSubsystem;
    ARFaceManager faceManager;
    NativeArray<ARCoreFaceRegionData> faceRegions;
    [SerializeField] bool isPointEnabled = false;

    [Header("Christmas Edition")]
    [SerializeField] GameObject santaHatObj;
    [SerializeField] GameObject santaBeirdObj;
    [SerializeField] GameObject particle;

    private void Awake()
    {
        faceManager = GetComponent<ARFaceManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(isPointEnabled)
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
        }

        faceSubsystem = (ARCoreFaceSubsystem)faceManager.subsystem;

        faceManager.facesChanged += OnDetectedRegionPos;
        faceManager.facesChanged += OnDetectedTotalFeaturePos;

        santaHatObj.SetActive(false);
        santaBeirdObj.SetActive(false);
    }



    void OnDetectedRegionPos(ARFacesChangedEventArgs args)
    {
        if (args.updated.Count > 0)
        {
            faceSubsystem.GetRegionPoses(args.updated[0].trackableId, Unity.Collections.Allocator.Persistent, ref faceRegions);

            if (regionFeatures.Count > 0)
            {
                // 코(0), 왼쪽이마(1), 오른쪽이마(2)
                for (int i = 0; i < faceRegions.Length; i++)
                {
                    regionFeatures[i].transform.position = faceRegions[i].pose.position;
                    regionFeatures[i].transform.rotation = faceRegions[i].pose.rotation;
                    regionFeatures[i].SetActive(true);
                }
            }
        }
        else if (args.removed.Count > 0)
        {
            if (regionFeatures.Count > 0)
            {
                for (int i = 0; i < faceRegions.Length; i++)
                {
                    regionFeatures[i].SetActive(false);
                }
            }
        }
        
    }

    Vector3 vertPos;
    Vector3 worldVertPos;
    void OnDetectedTotalFeaturePos(ARFacesChangedEventArgs args)
    {
        if (args.updated.Count > 0)
        {
            if (totalFeatures.Count > 0)
            {
                for (int i = 0; i < args.updated[0].vertices.Length; i++)
                {
                    vertPos = args.updated[0].vertices[i];
                    worldVertPos = args.updated[0].transform.TransformPoint(vertPos);

                    totalFeatures[i].transform.position = worldVertPos;
                    totalFeatures[i].SetActive(true);

                    featureTexts[i].text = i.ToString();
                }
            }

            LocateSantaObjs();

            void LocateSantaObjs()
            {
                vertPos = args.updated[0].vertices[151];
                worldVertPos = args.updated[0].transform.TransformPoint(vertPos);

                santaHatObj.transform.position = worldVertPos;
                santaHatObj.transform.localRotation = faceRegions[0].pose.rotation;
                santaHatObj.SetActive(true);

                vertPos = args.updated[0].vertices[306];
                worldVertPos = args.updated[0].transform.TransformPoint(vertPos);

                santaBeirdObj.transform.position = worldVertPos;
                santaBeirdObj.transform.localRotation = faceRegions[0].pose.rotation;
                santaBeirdObj.SetActive(true);
            }
        }
        else if (args.removed.Count > 0)
        {
            if (totalFeatures.Count > 0)
            {
                for (int i = 0; i < totalFeatures.Count; i++)
                {
                    totalFeatures[i].SetActive(false);
                }
            }

            santaHatObj.SetActive(false);
            santaBeirdObj.SetActive(false);
        }
    }
}
