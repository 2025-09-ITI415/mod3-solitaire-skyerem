using UnityEngine;
using UnityEngine.UI;

public class MatchMode : MonoBehaviour
{
    [Header("Assign your buttons here")]
    public Button buttonChoose;   // prospector
    public Button buttonChoose1;  // pyramid

    [Header("Game Controllers")]
    public GameObject prospectorObj;
    public GameObject pyramidObj;

    [Header("Debug Mode Display")]
    public bool useAdjacentTo = true; // true = Prospector, false = Pyramid

    void Start()
    {
        if (buttonChoose != null)
            buttonChoose.onClick.AddListener(() => SetMode(true));

        if (buttonChoose1 != null)
            buttonChoose1.onClick.AddListener(() => SetMode(false));
    }

    void SetMode(bool useAdjacent)
    {
        useAdjacentTo = useAdjacent;

        // activates only the mode we choose
        if (prospectorObj != null) prospectorObj.SetActive(useAdjacent);
        if (pyramidObj != null) pyramidObj.SetActive(!useAdjacent);
        if (!useAdjacent && pyramidObj != null)
        {
            Pyramid pyramid = pyramidObj.GetComponent<Pyramid>();
            if (pyramid != null && Pyramid.P == null)
            {
                pyramid.enabled = true;
            }
        }
        Debug.Log("Match mode set to: " + (useAdjacentTo ? "Prospector (AdjacentTo)" : "Pyramid (Add to 13)"));
    }
}


