using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class stores information about each decorator or pip from JSON_Deck
[System.Serializable]                                                        // a
public class JsonPipbj
{                                                       // b
    public string type = "pip";  // "pip", "letter", or "suit"
    public Vector3 loc;           // Location of the Sprite on the Card
    public bool flip = false;  // True to flip the Sprite vertically
    public float scale = 1;     // The scale of the Sprite
}

// This class stores information for each rank of card
[System.Serializable]
public class JsonCardbj
{                                                       // c
    public int rank;          // The rank (1-13) of this card
    public string face;          // Sprite to use for each face card
    public List<JsonPipbj> pips = new List<JsonPipbj>(); // The pips on this card
}

// This class contains information about the entire deck
[System.Serializable]
public class JsonDeckbj
{                                                       // d
    public List<JsonPipbj> decorators = new List<JsonPipbj>();
    public List<JsonCardbj> cards = new List<JsonCardbj>();
}

public class JsonParseDeckbj : MonoBehaviour
{
    private static JsonParseDeckbj Z { get; set; } // Another automatic property

    [Header("Inscribed")]
    public TextAsset jsonDeckFile;  // Reference to the JSON_Deck text file 

    [Header("Dynamic")]
    public JsonDeckbj deck;



    void Awake()
    {
        if (Z != null)
        {
            Debug.LogError("JsonParseDeckbj.S can’t be set a 2nd time!");
            return;
        }
        Z  = this;

        deck = JsonUtility.FromJson<JsonDeckbj>(jsonDeckFile.text);
    }

    /// <summary>
    /// Returns the decorator layout information for all cards.
    /// </summary>
    static public List<JsonPipbj> DECORATORS
    {
        get { return Z.deck.decorators; }
    }

    /// <summary>
    /// Returns the JsonCardbj matching the rank passed in.
    /// Note: The rank should be 1 (Ace) - 13 (King).
    /// </summary>
    /// <param name="rank">Must be an int in range 1-13</param>
    /// <returns>JsonCardbj information</returns>
    static public JsonCardbj GET_CARD_DEF(int rank)
    {
        if ((rank < 1) || (rank > Z.deck.cards.Count))
        {
            Debug.LogWarning("Illegal rank argument: " + rank);
            return null;
        }
        return Z.deck.cards[rank - 1];
    }
}


