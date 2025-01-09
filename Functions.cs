
public class Functions
{
    public static void Shuffle(List<TrumpCard> deck, Random rand)
    {
        int length = deck.Count;
        while (length > 0)
        {
            length--;
            int n = rand.Next(length);
            TrumpCard tempCard = deck[n];
            deck[n] = deck[length];
            deck[length] = tempCard;
        }
    }

    public static void Welcome()
    {
        Console.WriteLine("Welcome to the CLI Blackjack game");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("GOAL: Turn the starting $100 into $500");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("RULES: Standard rules with single deck and a soft 17 dealer hit");
        Console.WriteLine("RULES: MIN bet of 10 and bets can only be incremented in factors of 5");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("GENERAL COMMANDS");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("PLAY || P - Play the next hand");
        Console.WriteLine("HELP - Reprint game intro");
        Console.WriteLine("QUIT || Q - Exit the game");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("GAME COMMANDS");
        Console.WriteLine("---------------------------------");
        Console.WriteLine("HIT || H - Recieve a new card from the dealer");
        Console.WriteLine("STAND || S - Exit the game");
        Console.WriteLine("DOUBLE || DOWN || D - Double down");
        Console.WriteLine("SURRENDER || FF - Surrender the hand before any other action and receieve 50% of bet back");
        Console.WriteLine("---------------------------------");
    }

    // HandValue()
    public static int HandValue(List<TrumpCard> hand)
    {
        int value = 0;
        int aceCount = 0;

        foreach (TrumpCard card in hand)
        {
            value += card.value;
            if (card.value == 1)
            {
                aceCount++;
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (value < 12 && hand[i].value == 1)
            {
                value += 10;
            }
            if (value == 21)
            {
                return value;
            }
        }

        return value;
    }

    // Hit()
    public static void Hit(List<TrumpCard> deck, List<TrumpCard> hand)
    {
        hand.Add(deck[0]);
        deck.Remove(deck[0]);
    }

    // Reset Deck and hands
    public static void Reset(List<TrumpCard> deck, List<TrumpCard> hand)
    {
        int length = hand.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            deck.Add(hand[i]);
            hand.Remove(hand[i]);
        }
    }

    // DealerDraw()
    public static int DealerDraw(List<TrumpCard> deck, List<TrumpCard> dealerHand)
    {
        int handValue = HandValue(dealerHand);
        int aceCount = 0;
        int sixCount = 0;

        // Check for soft 17
        if (dealerHand.Count == 2)
        {
            foreach (TrumpCard card in deck)
            {
                if (card.value == 6)
                {
                    sixCount++;
                }
                if (card.value == 1)
                {
                    aceCount++;
                }
            }
        }

        // Loop until valid hand
        while (handValue < 21)
        {
            if (handValue < 17)
            {
                Hit(deck, dealerHand);
                handValue = HandValue(dealerHand);
                Console.WriteLine($"The dealer draws... {dealerHand[dealerHand.Count - 1].identity} of {dealerHand[dealerHand.Count - 1].suit}");
                if (handValue == 21)
                {
                    Console.WriteLine("");
                    return handValue;
                }
            }
            // Check for soft 17 to hit otherwise stand
            else if (handValue == 17)
            {
                if (sixCount == 1 && aceCount == 1)
                {
                    Hit(deck, dealerHand);
                    handValue = HandValue(dealerHand);
                    Console.WriteLine($"The dealer draws... {dealerHand[dealerHand.Count - 1].identity} of {dealerHand[dealerHand.Count - 1].suit}");

                    if (handValue == 21)
                    {
                        Console.WriteLine("");
                        return handValue;
                    }
                }
                else
                {
                    Console.WriteLine("");
                    return handValue;
                }
            }
            else
            {
                Console.WriteLine("");
                return handValue;
            }
        }

        Console.WriteLine("");
        Console.WriteLine($"Dealer went bust with a final hand value of {handValue}\n");
        return 0;
    }

    // Game state() Dealer hand
    public static void GameState(List<TrumpCard> deck, List<TrumpCard> playerHand, List<TrumpCard> dealerHand, Random rand)
    {
        int dealerCount = dealerHand.Count;
        int playerCount = playerHand.Count;
        string[] dealerCardArray = new string[dealerHand.Count];
        string[] playerCardArray = new string[playerHand.Count];
        string joinedString = "";

        // Print Dealer information
        Console.Write($"The Dealer's revealed cards are: ");
        for (int i = 0; i < dealerCount; i++)
        {
            dealerCardArray[i] = $"{dealerHand[i].identity} of {dealerHand[i].suit}";
        }
        joinedString = String.Join(", ", dealerCardArray);
        Console.WriteLine($"{joinedString}\n");

        joinedString = "";
        // Print Current Player Hand
        Console.WriteLine("Your current cards:");
        for (int i = 0; i < playerCount; i++)
        {
            playerCardArray[i] = $"{playerHand[i].identity} of {playerHand[i].suit}";
        }
        joinedString = String.Join(", ", playerCardArray);
        Console.WriteLine($"{joinedString}\n");

    }

    // Split() ---- Implement last once everything else is done
}
