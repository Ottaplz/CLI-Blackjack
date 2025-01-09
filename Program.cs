public class Program
{
    public static int Main()
    {
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
        Functions.Shuffle(Deck, rng);

        bool quit = false;
        Functions.Welcome();

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
                    playerCash += GameLogic.PlayRound(Deck, PlayerHand, DealerHand, playerCash, rng);
                    break;
                case var rule when input == "HELP" || input == "H":
                    Functions.Welcome();
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

    }
}