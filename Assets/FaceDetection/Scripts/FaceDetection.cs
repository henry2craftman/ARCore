using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

// 1. Region Pose
public class FaceDetection : MonoBehaviour
{
    List<GameObject> features = new List<GameObject>();
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
        for(int i = 0; i < 3; i++)
        {
            GameObject featureObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            featureObj.transform.localScale = Vector3.one * 0.02f;
            features.Add(featureObj);
            featureObj.SetActive(false);
        }

        faceSubsystem = (ARCoreFaceSubsystem)faceManager.subsystem;

        faceManager.facesChanged += OnDetectedRegionPos;
    }

    void OnDetectedRegionPos(ARFacesChangedEventArgs args)
    {
        if(args.updated.Count > 0)
        {
            faceSubsystem.GetRegionPoses(args.updated[0].trackableId, Unity.Collections.Allocator.Persistent, ref faceRegions);

            // 코(0), 왼쪽이마(1), 오른쪽이마(2)
            for(int i = 0; i < faceRegions.Length; i++)
            {
                features[i].transform.position = faceRegions[i].pose.position;
                features[i].transform.rotation = faceRegions[i].pose.rotation;
                features[i].SetActive(true);
            }
        }
    }
}
