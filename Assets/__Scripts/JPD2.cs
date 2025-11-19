using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class stores information about each decorator or pip from JSON_Deck
[System.Serializable]                                                        // a
public class Jpip2
{                                                       // b
    public string type = "pip";  // "pip", "letter", or "suit"
    public Vector3 loc;           // Location of the Sprite on the Card
    public bool flip = false;  // True to flip the Sprite vertically
    public float scale = 1;     // The scale of the Sprite
}

// This class stores information for each rank of card
[System.Serializable]
public class JCard
{                                                       // c
    public int rank;          // The rank (1-13) of this card
    public string face;          // Sprite to use for each face card
    public List<Jpip2> pips = new List<Jpip2>(); // The pips on this card
}

// This class contains information about the entire deck
[System.Serializable]
public class JDeck
{                                                       // d
    public List<Jpip2> decorators = new List<Jpip2>();
    public List<JCard> cards = new List<JCard>();
}

public class JPD2 : MonoBehaviour
{
    private static JPD2 Z { get; set; } // Another automatic property

    [Header("Inscribed")]
    public TextAsset jsonDeckFile;  // Reference to the JSON_Deck text file 

    [Header("Dynamic")]
    public JDeck deck;



    void Awake()
    {
        if (Z != null)
        {
            Debug.LogError("JsonParseDeck.S can’t be set a 2nd time!");
            return;
        }
        Z  = this;

        deck = JsonUtility.FromJson<JDeck>(jsonDeckFile.text);
    }

    /// <summary>
    /// Returns the decorator layout information for all cards.
    /// </summary>
    static public List<Jpip2> DECORATORS
    {
        get { return Z.deck.decorators; }
    }

    /// <summary>
    /// Returns the JCard matching the rank passed in.
    /// Note: The rank should be 1 (Ace) - 13 (King).
    /// </summary>
    /// <param name="rank">Must be an int in range 1-13</param>
    /// <returns>JCard information</returns>
    static public JCard GET_CARD_DEFI(int rank)
    {
        if ((rank < 1) || (rank > Z.deck.cards.Count))
        {
            Debug.LogWarning("Illegal rank argument: " + rank);
            return null;
        }
        return Z.deck.cards[rank - 1];
    }
}


