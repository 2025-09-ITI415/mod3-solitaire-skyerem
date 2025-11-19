using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardprpr : MonoBehaviour
{
    [Header("Dynamic")]
    public char suit; // Suit of thecardprpr (C,D,H, or S)
    public int rank; // Rank of thecardprpr (1-13)
    public Color color = Color.black; // Color to tint pips
    public string colS = "Black"; // or "Red". Name of the Color
    public GameObject back; // The GameObject of the back of thecardprpr
    public JCard def; // Thecardprpr layout as defined in JSON_dEck2.json

    // This List holds all of the Decorator GameObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    // This List holds all of the Pip GameObjects
    public List<GameObject> pipGOs = new List<GameObject>();

    /// <summary>
    /// Creates thiscardprpr’s visuals based on suit and rank.
    /// Note that this method assumes it will be passed a valid suit and rank.
    /// </summary>
    /// <param name="eSuit">The suit of thecardprpr (e.g., ’C’)</param>
    /// <param name="eRank">The rank from 1 to 13</param>
    /// <returns></returns>
    public void Init(char eSuit, int eRank, bool startFaceUp = true)
    {
        // Assign basic values to thecardprpr
        gameObject.name = name = eSuit.ToString() + eRank;
        suit = eSuit;
        rank = eRank;
        // If this is a Diamond or Heart, change the default Black color to Red
        if (suit == 'D' || suit == 'H')
        {
            colS = "Red";
            color = Color.red;
        }

        def = JPD2.GET_CARD_DEFI(rank);

        // Build thecardprpr from Sprites
        AddDecorators();
        AddPips();
        AddFace();
        AddBack();
        faceUp = startFaceUp;

    }

    /// <summary>
    /// Shortcut for setting transform.localPosition.
    /// </summary>
    /// <param name="v"></param>
    public virtual void SetLocalPos(Vector3 v)
    {                              // b
        transform.localPosition = v;
    }

    // These private variables that will be reused several times                    // c
    private Sprite _tSprite = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSRend = null;
    // An Euler rotation of 180° around the Z-axis will flip sprites upside down
    private Quaternion _flipRot = Quaternion.Euler(0, 0, 180);                  // d

    /// <summary>
    /// Adds the decorators to the top-left and bottom-right of eachcardprpr.
    ///  Decorators are the suit and rank in the corners of eachcardprpr.
    /// </summary>
    private void AddDecorators()
    {
        // Add Decorators
        foreach (Jpip2 pip in JPD2.DECORATORS)
        {                         // e
            if (pip.type == "suit")
            {
                // Instantiate a Sprite GameObject
                _tGO = Instantiate<GameObject>(dEck2.SPRITE_PREFAB, transform);       // f
                                                                                     // Get the SpriteRenderer Component
                _tSRend = _tGO.GetComponent<SpriteRenderer>();
                // Get the suit Sprite from theCardspritessopros.SUIT static field
                _tSRend.sprite =Cardspritessopros.SUITS[suit];
            }
            else
            {
                _tGO = Instantiate<GameObject>(dEck2.SPRITE_PREFAB, transform);       // f
                _tSRend = _tGO.GetComponent<SpriteRenderer>();
                // Get the rank Sprite from theCardspritessopros.RANK static field
                _tSRend.sprite =Cardspritessopros.RANKS[rank];
                // Set the color of the rank to match the suit
               // _tSRend.color = color;
            }

            // Make the Decorator Sprites render above thecardprpr
            _tSRend.sortingOrder = 1;                                               // g
                                                                                    // Set the localPosition based on the location from dEck2XML
            _tGO.transform.localPosition = pip.loc;
            // Flip the decorator if needed
            if (pip.flip) _tGO.transform.rotation = _flipRot;                       // h
                                                                                    // Set the scale to keep decorators from being too big
            if (pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            // Name this GameObject so it’s easy to find in the Hierarchy
            _tGO.name = pip.type;
            // Add this decorator GameObject to the Listcardprpr.decoGOs
            decoGOs.Add(_tGO);
        }
    }

    /// <summary>
    /// Adds pips to the front of allcardprprs from A to 10
    /// </summary>
    private void AddPips()
    {
        int pipNum = 0;
        // For each of the pips in the definition...
        foreach (Jpip2 pip in def.pips)
        {                                   // b
                                            // Instantiate a GameObject from the dEck2.SPRITE_PREFAB static field
            _tGO = Instantiate<GameObject>(dEck2.SPRITE_PREFAB, transform);
            // Set the position to that specified in the JSON
            _tGO.transform.localPosition = pip.loc;
            // Flip it if necessary
            if (pip.flip) _tGO.transform.rotation = _flipRot;
            // Scale it if necessary (only for the Ace)
            if (pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            // Give this GameObject a name
            _tGO.name = "pip_" + pipNum++;                                      // c
                                                                                // Get the SpriteRenderer Component
            _tSRend = _tGO.GetComponent<SpriteRenderer>();
            // Set the Sprite to the proper suit
            _tSRend.sprite =Cardspritessopros.SUITS[suit];
            // sortingOrder=1 renders this pip above thecardprpr_Front
            _tSRend.sortingOrder = 1;
            // Add this to thecardprpr’s list of pips
            pipGOs.Add(_tGO);
        }
    }

    /// <summary>
    /// Adds the face sprite forcardprpr ranks 11 to 13
    /// </summary>
    private void AddFace()
    {
        if (def.face == "")
            return;// No need to run if this isn’t a facecardprpr

        // Find a face sprite inCardspritessopros with the right name
        string faceName = def.face + suit;                                   // b
        _tSprite =Cardspritessopros.GET_FACE(faceName);                       // c
        if (_tSprite == null)
        {
            Debug.LogError("Face sprite " + faceName + " not found.");
            return;
        }
        _tGO = Instantiate<GameObject>(dEck2.SPRITE_PREFAB, transform);     // d
        _tSRend = _tGO.GetComponent<SpriteRenderer>();
        _tSRend.sprite = _tSprite;// Assign the face Sprite to _tSRend
        _tSRend.sortingOrder = 1;// Set the sortingOrder
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = faceName;
    }

    /// <summary>
    /// Property to show and hide the back of thecardprpr.
    /// </summary>
    public bool faceUp
    {
        get { return (!back.activeSelf); }                                   // a
        set { back.SetActive(!value); }
    }

    /// <summary>
    /// Adds a back to thecardprpr so that renders on top of everything else
    /// </summary>
    private void AddBack()
    {
        _tGO = Instantiate<GameObject>(dEck2.SPRITE_PREFAB, transform);
        _tSRend = _tGO.GetComponent<SpriteRenderer>();
        _tSRend.sprite =Cardspritessopros.BACK;
        _tGO.transform.localPosition = Vector3.zero;
        // 2 is a higher sortingOrder than anything else
        _tSRend.sortingOrder = 2;                                            // b
        _tGO.name = "back";
        back = _tGO;
    }

    private SpriteRenderer[] spriteRenderers;

    /// <summary>
    /// Gather all SpriteRenderers on this and its children into an array.
    /// </summary>
    void PopulateSpriteRenderers()
    {
        // If we’ve already populated spriteRenderers, just return.            // a
        if (spriteRenderers != null) return;
        // GetComponentsInChildren is slow, but we’re only doing it once percardprpr
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Moves the Sprites of thiscardprpr into a specified sorting layer
    /// </summary>
    /// <param name="layerName">The name of the layer to move to</param>
    public void SetSpriteSortingLayer(string layerName)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer srend in spriteRenderers)
        {
            srend.sortingLayerName = layerName;
        }
    }

    /// <summary>
    /// Sets the sortingOrder of the Sprites on thiscardprpr. This allows multiple
    ///cardprprs to be in the same sorting layer and still overlap properly, and
    /// it is used by both the draw and discard piles.
    /// </summary>
    /// <param name="sOrd">The sortingOrder for the face of thecardprpr</param>
    public void SetSortingOrder(int sOrd)
    {                                    // b
        PopulateSpriteRenderers();

        foreach (SpriteRenderer srend in spriteRenderers)
        {
            if (srend.gameObject == this.gameObject)
            {
                // If the gameObject is this.gameObject, it’s thecardprpr face
                srend.sortingOrder = sOrd;  // Set its order to sOrd
            }
            else if (srend.gameObject.name == "back")
            {
                // If it’s the back, set it to the highest layer
                srend.sortingOrder = sOrd + 2;
            }
            else
            {
                // If it’s anything else, put it in between.
                srend.sortingOrder = sOrd + 1;
            }
        }
    }

    // Virtual methods can be overridden by subclass methods with the same name
    virtual public void OnMouseUpAsButton()
    {
        print(name);  // When clicked, this outputs thecardprpr name
    }

    /// <summary>
    /// Return true if the twocardprprs are adjacent in rank.
    /// If wrap is true, Ace and King are adjacent.
    /// </summary>
    /// <param name="otherCard">Thecardprpr to compare to</param>
    /// <param name="wrap">If true (default) Ace and King wrap</param>
    /// <returns>true, if thecardprprs are adjacent</returns>
    public bool AdjacentTo(cardprpr otherCard, bool wrap = true)
    {
        // If eithercardprpr is face-down, it’s not a valid match.
        if (!faceUp || !otherCard.faceUp) return (false);

        // If the ranks are 1 apart, they are adjacent
        if (Mathf.Abs(rank - otherCard.rank) == 1) return (true);

        if (wrap)
        {  // If wrap == true, Ace and King are treated as adjacent
            // If onecardprpr is Ace and the other King, they are adjacent
            if (rank == 1 && otherCard.rank == 13) return (true);
            if (rank == 13 && otherCard.rank == 1) return (true);
        }

        return (false);  // Otherwise, return false
    }

}
