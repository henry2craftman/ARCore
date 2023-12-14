using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorTimer : MonoBehaviour
{
    ObjectManager objectManager;
    Image image;

    private void Awake()
    {
        objectManager = FindAnyObjectByType<ObjectManager>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = objectManager.ElapsedTime / objectManager.SpawnTime;
    }
}
