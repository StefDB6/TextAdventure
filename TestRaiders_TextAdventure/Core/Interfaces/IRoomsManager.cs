namespace TestRaiders_TextAdventure.Core.Interfaces
{
    public interface IRoomsManager
    {
        string Go(Direction direction);
        string Look();
        string Take(string itemId);
        string Fight();
        bool CheckWin();
        bool IsGameOver { get; }
    }
}
