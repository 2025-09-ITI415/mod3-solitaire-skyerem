using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChoose : MonoBehaviour
{
    public GameObject targetObject;
    private Canvas parentCanvas;
    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    void OnButtonClick()
    {
        if (parentCanvas != null)
            parentCanvas.gameObject.SetActive(false);
        if (targetObject != null)
            targetObject.SetActive(true);
        {

        }
    }
}