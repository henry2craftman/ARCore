using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject showcaseObj;
    ARRaycastManager raycastManager;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        showcaseObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ARRaycastHit hitInfo = CastARRay();

        TouchScreen(hitInfo);
    }

    /// <summary>
    /// 스크린을 터치하면 Object를 바닥에 위치시킨다.
    /// </summary>
    private void TouchScreen(ARRaycastHit hitInfo)
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                if(hitInfo.trackable)
                {
                    showcaseObj.SetActive(true);
                    showcaseObj.transform.position = hitInfo.pose.position;
                }
                else
                {
                    showcaseObj.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// AR Ray를 발사하여 indicator를 위치시킨다.
    /// </summary>
    private ARRaycastHit CastARRay()
    {
        Vector2 screenPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        List<ARRaycastHit> hitInfo = new List<ARRaycastHit>();

        if (raycastManager.Raycast(screenPoint, hitInfo, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            indicator.SetActive(true);
            indicator.transform.position = hitInfo[0].pose.position;
            indicator.transform.rotation = hitInfo[0].pose.rotation;
            indicator.transform.forward = Vector3.up;
        }
        else
        {
            indicator.SetActive(false);
        }

        return hitInfo[0];
    }
}
