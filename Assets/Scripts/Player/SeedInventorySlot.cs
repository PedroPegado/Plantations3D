[System.Serializable]
public class SeedInventorySlot
{
    public SeedData seed;
    public int quantity;

    public SeedInventorySlot(SeedData seed, int quantity)
    {
        this.seed = seed;
        this.quantity = quantity;
    }
}