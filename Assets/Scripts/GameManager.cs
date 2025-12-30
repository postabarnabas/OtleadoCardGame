using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum TurnPhase
    {
        Give,
        Beat
    }
    public TurnPhase currentPhase = TurnPhase.Give;
    public Deck deck;
    public GameObject deckarea;
    public List<Player> players = new List<Player>();
    public GameObject cardPrefab;
    public Transform[] playerAreas;
    public int currentPlayerIndex = 0;
    public HandView[] handViews;
    public TMPro.TextMeshProUGUI currentPlayerText;
    public Transform tableArea;
    public Transform playedArea;
    public Transform beatArea;
    public GameObject playButton;
    public GameObject pickupButton;
    public GameObject beatButton;
    private List<Card> pickedUpThisTurn = new List<Card>();
    bool hasPickedUpThisTurn => pickedUpThisTurn.Count > 0;

    void Start()
    {
        StartGame();
    }
    void StartGame()
    {
        deck = new Deck();

        players.Add(new Player("Játékos 1"));
        players.Add(new Player("Játékos 2"));

        foreach (var player in players)
            player.AddCards(deck.DrawCards(5));

        for (int i = 0; i < players.Count; i++)
            handViews[i].Refresh(players[i]);

        currentPlayerIndex = 0;
        UpdateTurn();
        HidePickupButton();
        HideBeatButton();
    }
    void UpdateTurn()
    {
        for (int i = 0; i < handViews.Length; i++)
            handViews[i].SetActive(i == currentPlayerIndex);

        currentPlayerText.text =
            $"Aktuális játékos: Player {currentPlayerIndex + 1}";
    }
    //gombok elrejtése és előhívása
    #region
    void HidePlayButton()
    {
        playButton.SetActive(false);
    }
    void ShowPlayButton()
    {
        playButton.SetActive(true);
    }
    void HidePickupButton()
    {
        pickupButton.SetActive(false);
    }
    void ShowPickupButton()
    {
        pickupButton.SetActive(true);
    }
    void HideBeatButton()
    {
        beatButton.SetActive(false);
    }
    void ShowBeatButton()
    {
        beatButton.SetActive(true);
    }
    #endregion
    void EndTurn()
    {
        StartCoroutine(RefillAfterDelay(players[currentPlayerIndex]));

        // ha felvett → vissza arra, aki leadott
        if (hasPickedUpThisTurn)
        {
            currentPlayerIndex =
                (currentPlayerIndex - 1 + players.Count) % players.Count;
        }

        // kör újraindítása
        currentPhase = TurnPhase.Give;
        pickedUpThisTurn.Clear();

        // UI reset
        ShowPlayButton();
        HidePickupButton();
        HideBeatButton();

        UpdateTurn();
    }

    public void OnPlayButtonClicked()
    {
        if (currentPhase != TurnPhase.Give)
        {
            Debug.Log("Nem leadó fázis");
            return;
        }
        HandView currentHandView = handViews[currentPlayerIndex];
        Player currentPlayer = players[currentPlayerIndex];

        var selectedCardViews = currentHandView.GetSelectedCards();

        if (selectedCardViews.Count == 0)
        {
            Debug.Log("Nincs kiválasztott lap");
            return;
        }

        List<Card> selectedCards = new List<Card>();
        foreach (var cv in selectedCardViews)
            selectedCards.Add(cv.card);

        if (!IsValidSelection(selectedCards))
        {
            return;
        }

        foreach (var cv in selectedCardViews)
        {
            // logikai eltávolítás
            currentPlayer.Hand.Remove(cv.card);

            // UI eltávolítás kézből
            currentHandView.RemoveCard(cv);

            // UI megjelenítés az asztalon
            GameObject tableCard = Instantiate(cardPrefab);
            tableCard.transform.SetParent(playedArea, false);
            var tableCardView = tableCard.GetComponent<CardView>();
            tableCardView.SetCard(cv.card);
        }
        StartCoroutine(RefillAfterDelay(currentPlayer));
        currentPhase = TurnPhase.Beat;
        EnablePickupOnPlayedCards();
        HidePlayButton();
        ShowPickupButton();
        ShowBeatButton();
        SwitchToOtherPlayerForBeat();
    }
    void SwitchToOtherPlayerForBeat()
    {
        handViews[currentPlayerIndex].SetActive(false);

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        handViews[currentPlayerIndex].SetActive(true);

        currentPlayerText.text =
            $"Üt / felvesz: Player {currentPlayerIndex + 1}";
    }
    bool IsValidSelection(List<Card> cards)
    {
        int count = cards.Count;

        if (count == 1)
            return true;
        if (count == 2 || count == 4)
        {
            Debug.Log("1, 3 vagy 5 lapot lehet leadni.");
            return false;
        }
        if (count == 3)
        {
            return IsValidThree(cards);
        }
        if (count == 5)
        {
            return IsValidFive(cards);
        }
        return false;
    }
    bool IsValidThree(List<Card> cards)
    {
        CardRank r1 = cards[0].Rank;
        CardRank r2 = cards[1].Rank;
        CardRank r3 = cards[2].Rank;

        if (r1 == r2 && r2 == r3)
        {
            Debug.Log("Érvénytelen lapkombináció");
            return false;
        }
        if (r1 != r2 && r1 != r3 && r2 != r3)
        {
            Debug.Log("Érvénytelen lapkombináció");
            return false;
        }
        // egy pár és egy különböző
        return true;
    }
    bool IsValidFive(List<Card> cards)
    {
        List<CardRank> distinctRanks = new List<CardRank>();
        foreach (var c in cards)
        {
            if (!distinctRanks.Contains(c.Rank)) distinctRanks.Add(c.Rank);
        }

        List<int> counts = new List<int>();
        foreach (var rank in distinctRanks)
        {
            int count = cards.Count(c => c.Rank == rank);
            counts.Add(count);
        }

        counts.Sort();

        if (counts.SequenceEqual(new List<int> { 1, 2, 2 })) return true;
        if (counts.SequenceEqual(new List<int> { 1, 4 })) return true;
        Debug.Log("Érvénytelen lapkombináció");
        return false;

    }
    void RefillHand(Player player, HandView handView)
    {
        while (player.Hand.Count < 5 && deck.Cards.Count > 0)
        {
            Card c = deck.DrawCard();
            player.Hand.Add(c);
        }
        handView.Refresh(player);
    }
    IEnumerator RefillAfterDelay(Player player)
    {
        yield return new WaitForSeconds(0.7f);
        int index = players.IndexOf(player);
        RefillHand(player, handViews[index]);
        if (deck.Cards.Count == 0)
        {
            deckarea.SetActive(false);
        }

        CheckEndGame();
    }
    void EnablePickupOnPlayedCards()
    {
        foreach (Transform t in playedArea)
        {
            CardView cv = t.GetComponent<CardView>();
            if (cv != null)
            {
                cv.isPickupSelectable = true;
                cv.isSelected = false;
                cv.UpdateOutline();
            }
        }
    }
    public void OnPickupButtonClicked()
    {
        if (currentPhase != TurnPhase.Beat)
            return;

        List<CardView> selected = new();

        foreach (Transform t in playedArea)
        {
            CardView cv = t.GetComponent<CardView>();
            if (cv != null && cv.IsSelected)
                selected.Add(cv);
        }

        if (selected.Count == 0)
        {
            Debug.Log("Nincs kiválasztott lap felvételhez");
            return;
        }

        // 👉 FELVÉTEL
        foreach (var cv in selected)
        {
            pickedUpThisTurn.Add(cv.card);
            players[currentPlayerIndex].Hand.Add(cv.card);
            Destroy(cv.gameObject);
        }

        handViews[currentPlayerIndex].Refresh(players[currentPlayerIndex]);

        // 👉 ITT jön a döntés
        bool hasUnbeaten = false;
        foreach (Transform t in playedArea)
        {
            CardView cv = t.GetComponent<CardView>();
            if (cv != null && !cv.isSelected)
            {
                hasUnbeaten = true;
                break;
            }
        }
        if (!hasUnbeaten)
        {
            EndTurn();
        }
    }
    public void TryBeatWithCard(CardView attacker)
    {
        if (pickedUpThisTurn.Contains(attacker.card))
        {
            Debug.Log("❌ Felvett lappal ebben a körben nem üthetsz");
            return;
        }
        // összes leadott lap lekérése
        List<CardView> targets = new List<CardView>();
        foreach (Transform t in playedArea)
        {
            CardView cv = t.GetComponent<CardView>();
            if (cv != null)
                targets.Add(cv);
        }

        // melyeket tudja ütni?
        List<CardView> beatable = new List<CardView>();
        foreach (var target in targets)
        {
            if (!target.IsBeaten && CanBeat(attacker.card, target.card))
                beatable.Add(target);
        }
        if (beatable.Count == 0)
        {
            Debug.Log("❌ Ezzel a lappal egyet sem lehet ütni");
            return;
        }

        if (beatable.Count == 1)
        {
            BeatCard(attacker, beatable[0]);
            return;
        }

        Debug.Log("⚠ Több lap üthető → választás kell (később popup)");
    }
    bool CanBeat(Card attacker, Card target)
    {
        return (int)attacker.Rank > (int)target.Rank && (int)attacker.Suit==(int)target.Suit;
    }
    void BeatCard(CardView attacker, CardView target)
    {
        target.IsBeaten = true;
        target.BeatenBy = attacker;
        players[currentPlayerIndex].Hand.Remove(attacker.card);

        RectTransform attackerRt = attacker.GetComponent<RectTransform>();
        RectTransform targetRt = target.GetComponent<RectTransform>();

        // 1️⃣ target világpozíció
        Vector3 targetWorldPos = targetRt.position;

        // 2️⃣ parent váltás
        attackerRt.SetParent(beatArea, false);

        // 3️⃣ világ → beatArea lokális pozíció
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            beatArea as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, targetWorldPos),
            null,
            out localPoint
        );

        // 4️⃣ finom eltolás (fél takarás)
        float yOffset = -targetRt.rect.height * 0.25f;
        attackerRt.anchoredPosition = localPoint + new Vector2(0f, yOffset);

        attackerRt.localScale = Vector3.one;
        attackerRt.SetAsLastSibling();

    }
    public void OnBeatButtonClicked()
    {
        if (currentPhase != TurnPhase.Beat)
            return;

        List<CardView> toDestroy = new();

        foreach (Transform t in playedArea)
        {
            CardView target = t.GetComponent<CardView>();
            if (target != null && target.IsBeaten)
            {
                toDestroy.Add(target);
                if (target.BeatenBy != null)
                    toDestroy.Add(target.BeatenBy);
            }
        }

        if (toDestroy.Count == 0)
        {
            Debug.Log("Nincs elütött lap");
            return;
        }

        foreach (var cv in toDestroy)
            Destroy(cv.gameObject);

        // 👉 ITT jön a lényeg
        bool hasUnbeaten = false;
        foreach (Transform t in playedArea)
        {
            CardView cv = t.GetComponent<CardView>();
            if (cv != null && !cv.IsBeaten)
            {
                hasUnbeaten = true;
                break;
            }
        }
        if (!hasUnbeaten)
        {
            EndTurn();
        }
    }

    void CheckEndGame()
    {
        if (players[0].Hand.Count == 0)
        {
            EndGame(winnerIndex: 0, loserIndex: 1);
            return;
        }

        if (players[1].Hand.Count == 0)
        {
            EndGame(winnerIndex: 1, loserIndex: 0);
            return;
        }
    }
    void EndGame(int winnerIndex, int loserIndex)
    {
        Debug.Log($"Winner: Player {winnerIndex}");
        Debug.Log($"Loser: Player {loserIndex}");
        currentPlayerText.text = "Győztes: Player" + winnerIndex
                         + "\n" +"Vesztes: Player" + loserIndex;

        // Gombok tiltása
        HideBeatButton();
        HidePickupButton();
        HidePlayButton();
    }
}
