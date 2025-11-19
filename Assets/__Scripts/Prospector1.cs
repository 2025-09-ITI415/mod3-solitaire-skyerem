using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;   // We’ll need this line later in the chapter

[RequireComponent(typeof(dEck2))]                                              // a
[RequireComponent(typeof(JPL2))]
public class Prospector1 : MonoBehaviour
{
    private static Prospector1 Z; // A private Singleton for Prospector

    [Header("Dynamic")]
    public List<cardprospros> drawPile;

    public List<cardprospros> discardPile;
    public List<cardprospros> mine;
    public cardprospros target;

    private Transform layoutAnchor;

    private dEck2 deck;
    private JsonL2 jsonLayout;

    // A Dictionary to pair mine layout IDs and actual Cards
    private Dictionary<int, cardprospros> mineIdToCardDict;                 // a


    void Start()
    {
        // Set the private Singleton. We’ll use this later.
        if (Z != null) Debug.LogError("Attempted to set S more than once!");  // b
        Z = this;

        jsonLayout = GetComponent<JPL2>().layout;

        deck = GetComponent<dEck2>();
        // These two lines replace the Start() call we commented out in Deck
        deck.InitDeck();
        dEck2.Shuffle(ref deck.cards);

        drawPile = ConvertCardsTocardprospross(deck.cards);

        LayoutMine();

        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    /// <summary>
    /// Converts each Card in a List(Card) into a List(cardprospros) so that it
    ///  can be used in the Prospector game.
    /// </summary>
    /// <param name="listCard">A List(Card) to be converted</param>
    /// <returns>A List(cardprospros) of the converted cards</returns>
    List<cardprospros> ConvertCardsTocardprospross(List<cardprpr> listCard)
    {
        List<cardprospros> listCP = new List<cardprospros>();
        cardprospros cp;
        foreach (cardprpr card in listCard)
        {
            cp = card as cardprospros;                                      // c
            listCP.Add(cp);
        }
        return (listCP);
    }

    /// <summary>
    /// Pulls a single card from the beginning of the drawPile and returns it
    /// Note: There is no protection against trying to draw from an empty pile!
    /// </summary>
    /// <returns>The top card of drawPile</returns>
    cardprospros Draw()
    {
        cardprospros cp = drawPile[0]; // Pull the 0th cardprospros
        drawPile.RemoveAt(0);            // Then remove it from drawPile
        return (cp);                      // And return it
    }

    /// <summary>
    /// Positions the initial tableau of cards, a.k.a. the "mine"
    /// </summary>
    void LayoutMine()
    {
        // Create an empty GameObject to serve as an anchor for the tableau   // a
        if (layoutAnchor == null)
        {
            // Create an empty GameObject named _LayoutAnchor in the Hierarchy
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;             // Grab its Transform
        }

        cardprospros cp;

        // Generate the Dictionary to match mine layout ID to cardprospros
        mineIdToCardDict = new Dictionary<int, cardprospros>();             // b


        // Iterate through the JsonLayoutSlots pulled from the JSON_Layout
        foreach (JLS2 slot in jsonLayout.slots)
        {
            cp = Draw(); // Pull a card from the top (beginning) of the draw Pile
            cp.faceUp = slot.faceUp;    // Set its faceUp to the value in SlotDef
                                        // Make the cardprospros a child of layoutAnchor
            cp.transform.SetParent(layoutAnchor);

            // Convert the last char of the layer string to an int (e.g. "Row 0")
            int z = int.Parse(slot.layer[slot.layer.Length - 1].ToString());  // c

            // Set the localPosition of the card based on the slot information
            cp.SetLocalPos(new Vector3(
            jsonLayout.multiplier.x * slot.x,
            jsonLayout.multiplier.y * slot.y,
            -z));                                                       // d

            cp.layoutID = slot.id;
            cp.layoutSlot = slot;
            // cardprospross in the mine have the state CardState.mine
            cp.state = eprosCardState.mine;

            // Set the sorting layer of all SpriteRenderers on the Card
            cp.SetSpriteSortingLayer(slot.layer);

            mine.Add(cp); // Add this cardprospros to the List<mine>

            // Add this cardprospros to the mineIDtoCardDict Dictionary
            mineIdToCardDict.Add(slot.id, cp);                                // c

        }
    }

    /// <summary>
    /// Moves the current target card to the discardPile
    /// </summary>
    /// <param name="cp">The cardprospros to be moved</param>
    void MoveToDiscard(cardprospros cp)
    {
        // Set the state of the card to discard
        cp.state = eprosCardState.discard;
        discardPile.Add(cp);  // Add it to the discardPile List<>
        cp.transform.SetParent(layoutAnchor); // Update its transform parent

        // Position it on the discardPile
        cp.SetLocalPos(new Vector3(
        jsonLayout.multiplier.x * jsonLayout.discardPile.x,
        jsonLayout.multiplier.y * jsonLayout.discardPile.y,
        0));

        cp.faceUp = true;

        // Place it on top of the pile for depth sorting
        cp.SetSpriteSortingLayer(jsonLayout.discardPile.layer);               // a
        cp.SetSortingOrder(-200 + (discardPile.Count * 3));                  // b
    }

    /// <summary>
    /// Make cp the new target card
    /// </summary>
    /// <param name="cp">The cardprospros to be moved</param>
    void MoveToTarget(cardprospros cp)
    {
        // If there is currently a target card, move it to discardPile
        if (target != null) MoveToDiscard(target);

        // Use MoveToDiscard to move the target card to the correct location
        MoveToDiscard(cp);                                                    // c

        // Then set a few additional things to make cp the new target
        target = cp; // cp is the new target
        cp.state = eprosCardState.target;

        // Set the depth sorting so that cp is on top of the discardPile
        cp.SetSpriteSortingLayer("Target");                                 // c
        cp.SetSortingOrder(0);
    }

    /// <summary>
    /// Arranges all the cards of the drawPile to show how many are left
    /// </summary>
    void UpdateDrawPile()
    {
        cardprospros cp;
        // Go through all the cards of the drawPile
        for (int i = 0; i < drawPile.Count; i++)
        {
            cp = drawPile[i];
            cp.transform.SetParent(layoutAnchor);

            // Position it correctly with the layout.drawPile.stagger
            Vector3 cpPos = new Vector3();
            cpPos.x = jsonLayout.multiplier.x * jsonLayout.drawPile.x;
            // Add the staggering for the drawPile
            cpPos.x += jsonLayout.drawPile.xStagger * i;
            cpPos.y = jsonLayout.multiplier.y * jsonLayout.drawPile.y;
            cpPos.z = 0.1f * i;
            cp.SetLocalPos(cpPos);

            cp.faceUp = false; // DrawPile Cards are all face-down
            cp.state = eprosCardState.drawpile;
            // Set depth sorting
            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10 * i);
        }
    }

    /// <summary>
    /// This turns cards in the Mine face-up and face-down
    /// </summary>
    public void SetMineFaceUps()
    {                                            // d
        cardprospros coverCP;
        foreach (cardprospros cp in mine)
        {
            bool faceUp = true; // Assume the card will be face-up

            // Iterate through the covering cards by mine layout ID
            foreach (int coverID in cp.layoutSlot.hiddenBy)
            {
                coverCP = mineIdToCardDict[coverID];
                // If the covering card is null or still in the mine...
                if (coverCP == null || coverCP.state == eprosCardState.mine)
                {
                    faceUp = false; // then this card is face-down
                }
            }
            cp.faceUp = faceUp; // Set the value on the card
        }
    }



    /// <summary>
    /// Handler for any time a card in the game is clicked
    /// </summary>
    /// <param name="cp">The cardprospros that was clicked</param>
    static public void CARD_CLICKED(cardprospros cp)
    {
        // The reaction is determined by the state of the clicked card
        switch (cp.state)
        {
            case eprosCardState.target:
                // Clicking the target card does nothing
                break;
            case eprosCardState.drawpile:
                // Clicking *any* card in the drawPile will draw the next card
                // Call two methods on the Prospector Singleton S
               Z.MoveToTarget(Z.Draw());  // Draw a new target card
               Z.UpdateDrawPile();          // Restack the drawPile
                break;
            case eprosCardState.mine:
                // Clicking a card in the mine will check if it’s a valid play
                bool validMatch = true;  // Initially assume that it’s valid 

                // If the card is face-down, it’s not valid
                if (!cp.faceUp) validMatch = false;

                // If it’s not an adjacent rank, it’s not valid
                if (!cp.AdjacentTo(Z.target)) validMatch = false;            // b

                if (validMatch)
                {        // If it’s a valid card
                   Z.mine.Remove(cp);   // Remove it from the tableau List
                   Z.MoveToTarget(cp);  // Make it the target card

                   Z.SetMineFaceUps();  // Be sure to add this line!!
                }
                break;
        }
    }

}