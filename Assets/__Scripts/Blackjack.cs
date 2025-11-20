using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;  // We’ll need this line later in the chapter

[RequireComponent(typeof(Deckbj))]                                              // a
[RequireComponent(typeof(JsonParseLayoutbj))]
public class Blackjack : MonoBehaviour
{
    private static Blackjack B; // A private Singleton for Prospector

    [Header("Dynamic")]
    public List<CardBlackjack> drawPile;
 public List<CardBlackjack> discard;
    public List<CardBlackjack> dealer;
    public List<CardBlackjack> mine;
      public List<CardBlackjack> hitPile;
    public CardBlackjack target;
    public int cardshit = 0;
       public int carddeal = 0;
    public int cardsdealt = 0; 
     public int mycount = 0;  
   public  int dealercount  = 0;
   public Text Aceamnt;  
      public Text Urcount;
    private Transform layoutAnchor;

    private Deckbj deck;
    private JsonLayoutbj jsonLayout;

    // A Dictionary to pair mine layout IDs and actual Cards
    private Dictionary<int, CardBlackjack> mineIdToCardDict;                 // a


    void Start()
    {
        // Set the private Singleton. We’ll use this later.
        if (B != null) Debug.LogError("Attempted to set S more than once!");  // b
        B = this;

        jsonLayout = GetComponent<JsonParseLayoutbj>().layout;

        deck = GetComponent<Deckbj>();
        // These two lines replace the Start() call we commented out in Deck
        deck.InitDeck();
        Deckbj.Shuffle(ref deck.cards);

        drawPile = ConvertCardsToCardBlackjacks(deck.cards);

        Layoutmine();

       // MoveToTarget(Draw());
        UpdateDrawPile();

           for (int i = 0; i < mine.Count; i++){
            mycount += mine[i].rank;
        }

        for (int i = 0; i < dealer.Count; i++){
            dealercount += dealer[i].rank;
        }

           Urcount.text = "Your Count: " + mycount.ToString();
    }

    /// <summary>
    /// Converts each Card in a List(Card) into a List(CardBlackjack) so that it
    ///  can be used in the Prospector game.
    /// </summary>
    /// <param name="listCard">A List(Card) to be converted</param>
    /// <returns>A List(CardBlackjack) of the converted cards</returns>
    List<CardBlackjack> ConvertCardsToCardBlackjacks(List<cardbj> listCard)
    {
        List<CardBlackjack> listCP = new List<CardBlackjack>();
        CardBlackjack cp;
        foreach (cardbj card in listCard)
        {
            cp = card as CardBlackjack;                                      // c
            listCP.Add(cp);
        }
        return (listCP);
    }

    /// <summary>
    /// Pulls a single card from the beginning of the drawPile and returns it
    /// Note: There is no protection against trying to draw from an empty pile!
    /// </summary>
    /// <returns>The top card of drawPile</returns>
    CardBlackjack Draw()
    {
        CardBlackjack cp = drawPile[0];
        //mine.Add(cp); 
      // Pull the 0th CardBlackjack
        drawPile.RemoveAt(0);  
        
         // Then remove it from drawPile
        return (cp);                      // And return it
    }

    /// <summary>
    /// Positions the initial tableau of cards, a.k.a. the "mine"
    /// </summary>
    void Layoutmine()
    {
        
        // Create an empty GameObject to serve as an anchor for the tableau   // a
        if (layoutAnchor == null)
        {
            // Create an empty GameObject named _LayoutAnchor in the Hierarchy
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;             // Grab its Transform
        }

        CardBlackjack cp;
        Text rankindi;
        // Generate the Dictionary to match mine layout ID to CardBlackjack
        mineIdToCardDict = new Dictionary<int, CardBlackjack>();             // b
        

        // Iterate through the JsonLayoutSlots pulled from the JSON_Layout
        foreach (JsonLayoutSlotbj slot in jsonLayout.slots)
        {
            cp = Draw();

            rankindi = cp.GetComponentInChildren<Text>(true);
 // Pull a card from the top (beginning) of the draw Pile
            cp.faceUp = slot.faceUp;    // Set its faceUp to the value in SlotDef
                                        // Make the CardBlackjack a child of layoutAnchor
            cp.transform.SetParent(layoutAnchor);

            // Convert the last char of the layer string to an int (e.g. "Row 0")
            int B = int.Parse(slot.layer[slot.layer.Length - 1].ToString());  // c

            // Set the localPosition of the card based on the slot information
            cp.SetLocalPos(new Vector3(
            jsonLayout.multiplier.x * slot.x,
            jsonLayout.multiplier.y * slot.y,
            -B));                                                       // d

            cp.layoutID = slot.id;
            cp.layoutSlot = slot;
            cardsdealt++;



        
            // CardBlackjacks in the mine have the state CardState.mine
            if (cardsdealt > 2){
            cp.state = ebCardState.dealercards;
            dealer.Add(cp);
            } else {
            cp.state = ebCardState.mine;
              mine.Add(cp);
            }

            if (cp.rank == 1 && cp.state != ebCardState.dealercards){
               
             //  rankindi.gameObject.SetActive(true);
             // rankindi.transform.localPosition = new Vector3(0, 1.5f, 0);

                cp.state = ebCardState.ace;
                
            }
            // Set the sorting layer of all SpriteRenderers on the Card
            cp.SetSpriteSortingLayer(slot.layer);

           // Add this CardBlackjack to the List<mine>

            // Add this CardBlackjack to the mineIDtoCardDict Dictionary
            mineIdToCardDict.Add(slot.id, cp);         
            
             
                           // c

        }
    }

    /// <summary>
    /// Moves the current target card to the hitPile
    /// </summary>
    /// <param name="cp">The CardBlackjack to be moved</param>
    void MoveToDiscard(CardBlackjack cp)
    {
       //  Text rankindi = cp.GetComponentInChildren<Text>(true);
        // Set the state of the card to discard

        if (cp.rank == 1){
        cp.state = ebCardState.ace;
     //   rankindi.gameObject.SetActive(true);
        //   rankindi.transform.localPosition = new Vector3(cp.transform.position.x, 1.5f, 0);



        } else {
        cp.state = ebCardState.discard;
        }
        mine.Add(cp);  // Add it to the hitPile List<>
        cp.transform.SetParent(layoutAnchor); // Update its transform parent
        float staggerindext = jsonLayout.hitPile.xStagger * cardshit;
        // Position it on the hitPile
        cp.SetLocalPos(new Vector3(
        jsonLayout.multiplier.x * (jsonLayout.hitPile.x + staggerindext),
        jsonLayout.multiplier.y * jsonLayout.hitPile.y,
        0));
       cardshit++;
       

        cp.faceUp = true;

        // Place it on top of the pile for depth sorting
        cp.SetSpriteSortingLayer(jsonLayout.hitPile.layer);               // a
        cp.SetSortingOrder(-200 + (hitPile.Count * 3));  
        mycount += cp.rank;  

        if (mycount > 21){
              Urcount.text = "GAME OVER, BUST!"; 
        } else{
        Urcount.text = "Your Count: " + mycount.ToString();
        }             // b
    }
        void MoveToDealer(CardBlackjack cp)
    {
      
        dealer.Add(cp);  // Add it to the hitPile List<>
        cp.transform.SetParent(layoutAnchor); // Update its transform parent
        float staggerindext = jsonLayout.DealerhitPile.xStagger * carddeal;
        // Position it on the hitPile
        cp.SetLocalPos(new Vector3(
        jsonLayout.multiplier.x * (jsonLayout.DealerhitPile.x + staggerindext),
        jsonLayout.multiplier.y * jsonLayout.DealerhitPile.y,
        0));
       carddeal++;
       

        cp.faceUp = true;

        // Place it on top of the pile for depth sorting
        cp.SetSpriteSortingLayer(jsonLayout.DealerhitPile.layer);               // a
        cp.SetSortingOrder(-200 + (hitPile.Count * 3));  
        dealercount += cp.rank;                // b
    }
    /// <summary>
    /// Make cp the new target card
    /// </summary>
    /// <param name="cp">The CardBlackjack to be moved</param>
  /*  void MoveToTarget(CardBlackjack cp)
    {
        // If there is currently a target card, move it to hitPile
        if (target != null) MoveToDiscard(target);

        // Use MoveToDiscard to move the target card to the correct location
        MoveToDiscard(cp);                                                    // c

        // Then set a few additional things to make cp the new target
        target = cp; // cp is the new target
        cp.state = ebCardState.target;

        // Set the depth sorting so that cp is on top of the hitPile
        cp.SetSpriteSortingLayer("Target");                                 // c
        cp.SetSortingOrder(0);
    }*/

    void Aceswap(CardBlackjack cp){
 
         int rankrank = cp.rank;
            if (cp.state == ebCardState.ace){
                if (cp.rank  == 1){
                    cp.rank = 11; 
                    mycount += 10;
                } else if (cp.rank == 11 ) {
                    cp.rank = 1;
                    mycount -= 10;
                
                }
            }
               if (Aceamnt != null)
               Aceamnt.gameObject.SetActive(true);
                Aceamnt.text = "This Ace now equals "+ cp.rank.ToString();
                Invoke ("disabletext", 2f);
               
                
         }

    public void reloadscene(){
        SceneManager.LoadScene("BlackJack");
    }
    
    void disabletext(){
        Aceamnt.gameObject.SetActive(false);
    }
    /// <summary>
    /// Arranges all the cards of the drawPile to show how many are left
    /// </summary>
    void UpdateDrawPile()
    {
        CardBlackjack cp;
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
            cp.state = ebCardState.drawpile;
            // Set depth sorting
            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10 * i);
        }
    }

    /// <summary>
    /// This turns cards in the mine face-up and face-down
    /// </summary>
    public void SetmineFaceUps()
    {                                            // d
        CardBlackjack coverCP;
        foreach (CardBlackjack cp in mine)
        {
            bool faceUp = true; // Assume the card will be face-up

            // Iterate through the covering cards by mine layout ID
            foreach (int coverID in cp.layoutSlot.hiddenBy)
            {
                coverCP = mineIdToCardDict[coverID];
                // If the covering card is null or still in the mine...
                if (coverCP == null || coverCP.state == ebCardState.mine)
                {
                    faceUp = false; // then this card is face-down
                }
            }
            cp.faceUp = faceUp; // Set the value on the card
        }
    }

    public void Hit(){
         B.MoveToDiscard(B.Draw());  // Draw a new target card
         B.UpdateDrawPile();
         
    }

    public void Stay(){
        
        for (int i = 0; i < dealer.Count; i++){
            dealer[i].faceUp = true;
        }

        while (dealercount < 17){
            B.MoveToDealer(B.Draw());
            B.UpdateDrawPile();
        }

        if (dealercount < 22 && dealercount > mycount){
            Urcount.text = "YOU LOSE! DEALER COUNT: " + dealercount.ToString();
        } else {
               Urcount.text = "YOU WIN! ";
        }


    }


    /// <summary>
    /// Handler for any time a card in the game is clicked
    /// </summary>
    /// <param name="cp">The CardBlackjack that was clicked</param>
    static public void CARD_CLICKED(CardBlackjack cp)
    {
        // The reaction is determined by the state of the clicked card
        switch (cp.state)
        {
            case ebCardState.target:
                // Clicking the target card does nothing
                break;
            case ebCardState.ace:
                B.Aceswap(cp);
             break;
            case ebCardState.drawpile:
                // Clicking *any* card in the drawPile will draw the next card
                // Call two methods on the Prospector Singleton S
                      // Restack the drawPile
                break;
            case ebCardState.mine:
               
                break;
        }
    }

}