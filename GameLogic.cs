public class GameLogic
{
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
        Functions.Hit(deck, dealerHand);
        for (int j = 0; j < 2; j++)
        {
            Functions.Hit(deck, playerHand);
        }

        playerHandValue = Functions.HandValue(playerHand);

        Functions.GameState(deck, playerHand, dealerHand, rand);

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
                        Functions.Hit(deck, playerHand);
                        playerHandValue = Functions.HandValue(playerHand);
                        Functions.GameState(deck, playerHand, dealerHand, rand);
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
                Functions.Hit(deck, playerHand);
                playerHandValue = Functions.HandValue(playerHand);
                Functions.GameState(deck, playerHand, dealerHand, rand);
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
            dealerHandValue = Functions.DealerDraw(deck, dealerHand);
            Functions.GameState(deck, playerHand, dealerHand, rand);
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
        Functions.Reset(deck, playerHand);
        Functions.Reset(deck, dealerHand);
        Functions.Shuffle(deck, rand);
        return returnValue;
    }
}
