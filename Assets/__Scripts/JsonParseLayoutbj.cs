using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores information about the Layout of the Prospector mine.
/// </summary>
[System.Serializable]
public class JsonLayoutbj
{
    public Vector2 multiplier;
    public List<JsonLayoutSlotbj> slots;
    public JsonLayoutPilebj drawPile, discardPile, hitPile, finishPile, DealerhitPile;
}

/// <summary>
/// Stores information for each slot in the layout.
/// Implements Unity’s ISerializationCallbackReceiver Interface.
/// </summary>
[System.Serializable]
public class JsonLayoutSlotbj : ISerializationCallbackReceiver
{                  // a
    public int id;
    public int x;
    public int y;
    public bool faceUp;
    public string layer;
    public string hiddenByString;                                         // b

    [System.NonSerialized]                                                      // b
    public List<int> hiddenBy;                                               // b

    /// <summary>
    /// Pulls data from hiddenByString and places it into the hiddenBy List
    /// </summary>
    public void OnAfterDeserialize()
    {                                          // c
        hiddenBy = new List<int>();
        if (hiddenByString.Length == 0) return;

        string[] bits = hiddenByString.Split(',');
        for (int i = 0; i < bits.Length; i++)
        {
            hiddenBy.Add(int.Parse(bits[i]));
        }
    }

    /// <summary>
    /// Required by ISerializationCallbackReceiver, but empty in this class
    /// </summary>
    public void OnBeforeSerialize() { }     // Note the empty braces here        // d
}

/// <summary>
/// Stores information for the draw and discard piles
/// </summary>
[System.Serializable]
public class JsonLayoutPilebj
{
    public int x, y;
    public string layer;
    public float xStagger; // xStagger fans cards to the side for the draw pile
}

public class JsonParseLayoutbj : MonoBehaviour
{
    public static JsonParseLayoutbj S { get; private set; }


    [Header("Inscribed")]
    public TextAsset jsonLayoutFile;

    [Header("Dynamic")]
    public JsonLayoutbj layout;

    void Awake()
    {
        layout = JsonUtility.FromJson<JsonLayoutbj>(jsonLayoutFile.text);
        S = this;                                                               // e
    }

}
