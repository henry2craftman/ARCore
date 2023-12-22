using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ���ڰ� ������ �̿��Ͽ� �������� ���� �󸶳� ȸ���ߴ��� Ȯ��
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

        angleTxt.text = angle.ToString() + "��";

        compassObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
