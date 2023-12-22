using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase;
using System;

// Firebase Database에 접근하여 데이터를 쓰고 / 받아온다.
public class DBManager : MonoBehaviour
{
    [SerializeField] string dbURL;
    GPSManager gpsManager;

    class gpsList
    {
        public List<GPS> gps;
    }
    gpsList gpsInfo = new gpsList();
    //DatabaseReference dbReference;


    private void Awake()
    {
        gpsManager = FindAnyObjectByType<GPSManager>();
        gpsInfo.gps = gpsManager.Gps;
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

        StartCoroutine(SendData());
    }

    IEnumerator SendData()
    {
        DatabaseReference dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        //foreach(var gps in gpsInfo.gps)
        //{
        //    string json = JsonUtility.ToJson(gps);

        //    dbReference.Child("gps").Child(gps.name).SetRawJsonValueAsync(json);
        //}

        string json = JsonUtility.ToJson(gpsInfo);

        var task = dbReference.SetRawJsonValueAsync(json);

        yield return new WaitUntil(() => task.IsCompleted); // 다 보내졌다면

        // 작업 시작!
        yield return RequestData();
    }

    IEnumerator RequestData()
    {
        DatabaseReference dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        var task = dbReference.GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted); // 다 받아졌으면

        DataSnapshot snapshot = task.Result;

        foreach(var data in snapshot.Children)
        {
            string json = data.GetRawJsonValue();

            print(json);
        }
    }
}
