using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This enum defines the variable type eCardState with four named values.      // a
public enum eCardState { drawpile, mine, target, discard }

public class CardProspector : Card
{ // Make CardProspector extend Card        // b
    [Header("Dynamic: CardProspector")]
    public eCardState state = eCardState.drawpile;                   // c
                                                                     // The hiddenBy list stores which other cards will keep this one face down
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    // The layoutID matches this card to the tableau JSON if it’s a tableau card
    public int layoutID;
    // The JsonLayoutSlot class stores information pulled in from JSON_Layout
    public JsonLayoutSlot layoutSlot;

    /// <summary>
    /// Informs the Prospector class that this card has been clicked.
    /// </summary>
    override public void OnMouseUpAsButton()
    {
        MatchMode selector = FindObjectOfType<MatchMode>();

        if (selector != null)
        {
            // Prospector mode
            if (selector.useAdjacentTo && Prospector.S != null && Prospector.S.gameObject.activeInHierarchy)
            {
                Prospector.CARD_CLICKED(this);
            }
            // Pyramid mode
            else if (!selector.useAdjacentTo && Pyramid.P != null && Pyramid.P.gameObject.activeInHierarchy)
            {
                Pyramid.CARD_CLICKED(this);
            }
            else
            {
                Debug.LogWarning("No active mode detected or Singleton missing.");
            }
        }
        else
        {
            // this lets uf have a fallback if no selector is found
            if (Prospector.S != null)
                Prospector.CARD_CLICKED(this);
            else if (Pyramid.P != null)
                Pyramid.CARD_CLICKED(this);
        }

        base.OnMouseUpAsButton();
    }
}
