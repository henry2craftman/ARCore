using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;
using System;

public class GPSManager : MonoBehaviour
{
    [SerializeField] TMP_Text latitudeTxt;
    [SerializeField] TMP_Text longtitudeTxt;
    [SerializeField] TMP_Text logText;
    [SerializeField] float gpsRenewalTime = 1f;
    [SerializeField] List<GPS> gps;
    public List<GPS> Gps { get { return gps; } }
    [SerializeField] float minimumDistance = 20f;
    double latitude = 0;
    double longtitude = 0;
    CompassManager compass;
    [SerializeField] List<GameObject> locationObjects = new List<GameObject>();

    private void Awake()
    {
        compass = FindAnyObjectByType<CompassManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnOnGPS());
    }

    // 미터 단위로 계산된 거리를 지자계 센서의 각도 기준으로 회전변환
    private void CreateLocationObj()
    {
        string log = "";
        foreach(GPS location in gps)
        {
            float scaledPosX = (float)(location.latitude - latitude) * 100000;
            float scaledPosY = (float)(location.longtitude - longtitude) * 100000;

            log += location.name + "\n";

            // 3751365, 12703062 -> 100m, 20m
            log += "scaledPosX: " + scaledPosX + "/ scaledPosY " + scaledPosY + "\n";

            float newX = Mathf.Cos(compass.Angle) * scaledPosX + (-Mathf.Sin(compass.Angle) * scaledPosY);
            float newY = Mathf.Sin(compass.Angle) * scaledPosX + Mathf.Cos(compass.Angle) * scaledPosY;
            Vector3 objPos = new Vector3(newX, 0, newY);

            log += "newX: " + newX + "/ newY " + newY + "\n";

            GameObject prefab = Resources.Load<GameObject>(location.name);

            if(prefab != null)
            {
                GameObject locationObject = Instantiate(prefab);
                locationObject.name = location.name;
                locationObject.transform.position = objPos;

                locationObject.SetActive(false);
                locationObjects.Add(locationObject);

                logText.text += log;
            }
            else
            {
                logText.text += "No prefab exsist";
            }
        }

        
    }

    void CalculateDistance()
    {
        string totalDistance = "";

        foreach(var restaurant in gps)
        {
            float distance = CalculateDistance(latitude, longtitude, restaurant.latitude, restaurant.longtitude);

            if(locationObjects != null)
            {
                GameObject restaurnatObj = locationObjects.Find(obj => obj.name == restaurant.name);

                totalDistance += restaurnatObj.name + ": " + distance + "m\n";

                // Location Object 활성화
                if (distance < minimumDistance)
                {
                    restaurnatObj.SetActive(true);
                }
                else
                {
                    restaurnatObj.SetActive(false);
                }
            }
        }

        logText.text = totalDistance;
    }

    float CalculateDistance(double fromX, double fromY, double toX, double toY)
    {
        // meter 단위로 표시
        // 예시 나 37.514060, 127.029500, 맥도날드 37.513647, 127.030560
        // 0.000413^2 + (-0.00106)^2 -> 0.000000170569 + 0.0000011236 = 0.000001294169
        // sqrt(0.000001294169) = 0.001137
        // 0.001137 * 100,000 -> 113m
        float distance = Mathf.Sqrt(Mathf.Pow((float)(toX - fromX), 2) + Mathf.Pow((float)(toY - fromY), 2)) * 100000;

        return distance;
    }

    IEnumerator TurnOnGPS()
    {
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            if(Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                break;
            }
        }

        if(!Input.location.isEnabledByUser)
        {
            latitudeTxt.text = "GPS access failed";

            yield return new WaitForSeconds(3);

            Application.Quit();

            yield break;
        }

        Input.location.Start();

        while(Input.location.status == LocationServiceStatus.Initializing)
        {
            latitudeTxt.text = "Location Service Initializing..."; 

            yield return new WaitForSeconds(1);
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            latitudeTxt.text = "Initialization failed";

            yield return new WaitForSeconds(3);

            Application.Quit();

            yield break;
        }

        int tempNumber = 0;
        while (Input.location.status == LocationServiceStatus.Running)
        {
            yield return new WaitForSeconds(gpsRenewalTime);

            latitude = Input.location.lastData.latitude;
            longtitude = Input.location.lastData.longitude;

            latitudeTxt.text = string.Format("{0:F6}", latitude);
            longtitudeTxt.text = string.Format("{0:F6}", longtitude);

            if(tempNumber == 0)
            {
                CreateLocationObj();
                tempNumber = 1;
            }

            CalculateDistance();
        }
    }
}
