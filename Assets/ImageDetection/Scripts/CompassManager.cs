using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 지자계 센서를 이용하여 북쪽으로 부터 얼마나 회전했는지 확인
public class CompassManager : MonoBehaviour
{
    [SerializeField] GameObject compassObj;
    [SerializeField] TextMeshProUGUI angleTxt;
    int angle;
    public int Angle { get { return angle; } }

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;

        angle = Mathf.RoundToInt(Input.compass.trueHeading);
    }

    // Update is called once per frame
    void Update()
    {
        angle = 360 - Mathf.RoundToInt(Input.compass.trueHeading);

        angleTxt.text = angle.ToString() + "˚";

        compassObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
