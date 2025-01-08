using static MyExtensions;

// Initialize random class
Random rng = new Random();
int playerCash = 100;

// Create Cards + Deck
List<string> Suits = new List<string>
{
    "Spades", "Clubs", "Diamonds", "Hearts"
};

List<string> Identities = new List<string>
{
    "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King"
};

List<int> Values = new List<int>
{
    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10
};

// Build the deck 
List<TrumpCard> Deck = new List<TrumpCard>();

foreach (string suit in Suits)
{
    for (int i = 0; i < Identities.Count; i++)
    {
        TrumpCard tempCard = new TrumpCard(suit, Identities[i], Values[i]);
        Deck.Add(tempCard);
    }
}

// Shuffle and create lists for player and dealer cards
List<TrumpCard> DealerHand = new List<TrumpCard>();
List<TrumpCard> PlayerHand = new List<TrumpCard>();
List<TrumpCard> SplitHand = new List<TrumpCard>();
Shuffle(Deck, rng);

bool quit = false;
Welcome();

// Gameplay Loop
while (!quit)
{
    Console.Write("Input a general command (P/Q/H): ");
    string input = Console.ReadLine().ToUpper();
    Console.WriteLine("");

    switch (input)
    {
        case var rule when input == "QUIT" || input == "Q":
            Console.WriteLine("Thank you for playing");
            quit = true;
            break;
        case var rule when input == "PLAY" || input == "P":
            playerCash += PlayRound(Deck, PlayerHand, DealerHand, playerCash, rng);
            break;
        case var rule when input == "HELP" || input == "H":
            Welcome();
            break;
        default:
            Console.WriteLine("That is not a valid command. Please type HELP or H to view commands\n");
            break;
    }

    // Win and loss conditions
    if (playerCash > 499)
    {
        Console.WriteLine("Congratulations, you have defeated the dealer!");
        quit = true;
    }

    if (playerCash < 11)
    {
        Console.WriteLine("You do not have enough cash to continue playing...");
        Console.WriteLine("Better luck next time");
        quit = true;
    }
}

return 0;

// Card class
public class TrumpCard
{
    public string suit;
    public string identity;
    public int value;

    public TrumpCard(string suit = null, string identity = null, int value = 0)
    {
        this.suit = suit;
        this.identity = identity;
        this.value = value;
    }
}

// Functions
static class MyExtensions
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

    //PlayRound()
    public static int PlayRound(List<TrumpCard> deck, List<TrumpCard> playerHand, List<TrumpCard> dealerHand, int cash, Random rand)
    {
        bool firstAction = true;
        bool validBet = false;
        bool isBust = false;
        string command = "";
        int playerHandValue = 0;
        int dealerHandValue = 0;
        int bet = 0;
        int returnValue = 0;

        // Receive bet from player
        while (!validBet)
        {
            Console.WriteLine("Please place a bet in an increment of 10 with a MIN of $10");
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"CURRENT CASH TOTAL: ${cash}\n");
            Console.Write("Bet amount: $");
            bet = int.Parse(Console.ReadLine());
            Console.WriteLine("");

            if (bet >= 10 && bet % 10 == 0 && bet <= cash)
            {
                cash -= bet;
                validBet = true;
            }
            else if (bet > cash)
            {
                Console.WriteLine("You do not have enough cash to place that bet...\n");
            }
            else
            {
                Console.WriteLine("That is not a valid bet\n");
            }
        }

        // Deal first card to dealer and 2 cards to player
        Hit(deck, dealerHand);
        for (int j = 0; j < 2; j++)
        {
            Hit(deck, playerHand);
        }

        playerHandValue = HandValue(playerHand);

        GameState(deck, playerHand, dealerHand, rand);

        while (playerHandValue < 21)
        {
            Console.Write("Input a game command: ");
            command = Console.ReadLine().ToUpper();
            Console.WriteLine("");

            if (firstAction == true)
            {
                if (command == "DOUBLE" || command == "D")
                {
                    if (bet <= cash)
                    {
                        cash -= bet;
                        bet *= 2;
                        Hit(deck, playerHand);
                        playerHandValue = HandValue(playerHand);
                        GameState(deck, playerHand, dealerHand, rand);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("You do not have the cash left to Double down");
                    }
                }
                if (command == "SURRENDER" || command == "FF")
                {
                    return (bet / 2);
                }
            }

            if (command == "HIT" || command == "H")
            {
                // Receive card and print for player
                Hit(deck, playerHand);
                playerHandValue = HandValue(playerHand);
                GameState(deck, playerHand, dealerHand, rand);
            }
            else if (command == "STAND" || command == "S")
            {
                break;
            }
            else
            {
                Console.WriteLine("That is not a valid command\n");
            }

            firstAction = false;
        }

        // Check if busted
        if (playerHandValue > 21)
        {
            isBust = true;
        }
        else
        {
            //Dealer draw until handValue > 17
            dealerHandValue = DealerDraw(deck, dealerHand);
            GameState(deck, playerHand, dealerHand, rand);
        }
        
        // Player and Dealer Hand comaprisons and payout
        if (playerHandValue > 21)
        {
            Console.WriteLine("You went bust...");
            returnValue = -bet;
        }
        else if (playerHandValue > dealerHandValue)
        {
            if (playerHandValue == 21)
            {
                Console.WriteLine("Blackjack!");
                returnValue = bet + bet / 2;
            }
            else
            {
                Console.WriteLine("You won this hand!");
                returnValue = bet + bet / 2;
            }
        }
        else if (playerHandValue == dealerHandValue)
        {
            Console.WriteLine("Draw...Go agane");
            returnValue = 0;
        }
        else
        {
            Console.WriteLine("You went bust...");
            returnValue = -bet;
        }

        Console.WriteLine($"${returnValue} in the bank\n");
        Reset(deck, playerHand);
        Reset(deck, dealerHand);
        Shuffle(deck, rand);
        return returnValue;

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

    // Split() ---- Implement last once everything else is done\
}