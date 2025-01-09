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