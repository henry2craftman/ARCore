using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracker : MonoBehaviour
{
    ARTrackedImageManager imageManager;

    // Start is called before the first frame update
    void Start()
    {
        imageManager = GetComponent<ARTrackedImageManager>();

        imageManager.trackedImagesChanged += OnImageTrackedEvent;
    }

    void OnImageTrackedEvent(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage trackedImage in args.added)
        {
            string imageName = trackedImage.referenceImage.name;

            // Resources 폴더에서 objPrefab 정보를 가져옴
            GameObject objPrefab = Resources.Load<GameObject>(imageName);

            if(objPrefab != null)
            {
                // 정보를 가져왔다면, 게임오브젝트를 생성
                GameObject obj = Instantiate(
                    objPrefab,
                    trackedImage.transform.position,
                    trackedImage.transform.rotation);

                // TrackedImage의 자식으로 설정
                obj.transform.SetParent(trackedImage.transform);
            }
        }

        foreach (ARTrackedImage trackedImage in args.updated)
        {
            if(trackedImage.transform.childCount > 0)
            {
                trackedImage.transform.GetChild(0).gameObject.SetActive(true);
                trackedImage.transform.GetChild(0).position = trackedImage.transform.position;
                trackedImage.transform.GetChild(0).rotation = trackedImage.transform.rotation;
            }
        }

        

        //foreach (ARTrackedImage trackedImage in args.removed)
        //{

        //    if(trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
        //    {
        //        GameObject obj = objs.Find(value => value.name == trackedImage.name);
        //        obj.SetActive(false);
        //        //trackedImage.transform.GetChild(0).gameObject.SetActive(false);
        //    }

        //    //if (trackedImage.transform.childCount > 0)
        //    //{
        //    //    trackedImage.transform.GetChild(0).gameObject.SetActive(false);
        //    //}
        //}
    }
}
