using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChoose1 : MonoBehaviour
{
    public GameObject targetObject;
    private Canvas parentalCanvas;
    void Start()
    {
        parentalCanvas = GetComponentInParent<Canvas>();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    void OnButtonClick()
    {
        if (parentalCanvas != null)
            parentalCanvas.gameObject.SetActive(false);
        if (targetObject != null)
            targetObject.SetActive(true);
        {

        }
    }
}