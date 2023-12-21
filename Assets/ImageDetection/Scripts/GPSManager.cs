using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;

public class GPSManager : MonoBehaviour
{
    [SerializeField] TMP_Text latitudeTxt;
    [SerializeField] TMP_Text longtitudeTxt;
    [SerializeField] float gpsRenewalTime = 1;
    [SerializeField] List<GPS> gps; 
    double latitude = 0;
    double longtitude = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnOnGPS());
    }

    void CalculateDistance()
    {
        foreach(var restaurant in gps)
        {
            float distance = CalculateDistance(latitude, longtitude, restaurant.latitude, restaurant.longtitude);
        }
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

        while(Input.location.status == LocationServiceStatus.Running)
        {
            yield return new WaitForSeconds(gpsRenewalTime);

            latitude = Input.location.lastData.latitude;
            longtitude = Input.location.lastData.longitude;

            latitudeTxt.text = string.Format("{0:F6}", latitude);
            longtitudeTxt.text = string.Format("{0:F6}", longtitude);

            CalculateDistance();
        }
    }
}
