using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject showcaseObj;
    [SerializeField] float spawnTime = 1f;
    public float SpawnTime { get { return spawnTime; } }
    ARRaycastManager raycastManager;
    private float rotMultiplier = 0.1f;
    private float elapsedTime;
    public float ElapsedTime { get { return elapsedTime; } }


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
    /// 나중 터치 위치 - 처음 터치 위치 = 변화량 
    /// </summary>
    private void TouchScreen(ARRaycastHit hitInfo)
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                elapsedTime = 0;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 deltaPos = touch.deltaPosition;
                showcaseObj.transform.Rotate(transform.up, -deltaPos.x * rotMultiplier);
            }
            else if(touch.phase == TouchPhase.Stationary)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime > spawnTime)
                {
                    if (hitInfo.trackable)
                    {
                        showcaseObj.SetActive(true);
                        showcaseObj.transform.position = hitInfo.pose.position;
                    }
                    else
                    {
                        showcaseObj.SetActive(false);
                    }

                    elapsedTime = 0;
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                elapsedTime = 0;
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

    //private void CastRay()
    //{
    //    Touch touch = Input.GetTouch(0);
    //    Ray ray = Camera.main.ScreenPointToRay(touch.position);

    //    RaycastHit hitinfo;

    //    Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
    //    Debug.DrawRay(touchWorldPos, transform.forward * 10, Color.red, 5);

    //    if(Physics.Raycast(ray, out hitinfo))
    //    {
            
    //    }
    //}
}
