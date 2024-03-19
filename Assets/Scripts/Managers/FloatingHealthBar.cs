using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        cam = Camera.main;
    
        if (cam == null)
        {
            Debug.LogError("No main camera found");
        }
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    private void Update()
    {
        if(cam != null)
        {
            transform.rotation = cam.transform.rotation;
            transform.position = target.position + offset;
        }
    }
}
