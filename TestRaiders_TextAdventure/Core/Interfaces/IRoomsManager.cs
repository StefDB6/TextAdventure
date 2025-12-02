namespace TestRaiders_TextAdventure.Core.Interfaces
{
    public interface IRoomsManager
    {
        string Go(Direction direction);
        string Look();
        string Take(string itemId);
        void Fight();
        bool HasWon();
        bool IsGameOver { get; }
    }
}
