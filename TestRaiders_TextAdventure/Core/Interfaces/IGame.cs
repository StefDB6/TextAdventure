namespace TestRaiders_TextAdventure.Core.Interfaces
{
    internal interface IGame
    {
        void Start();
        void Quit();
        void ProcessCommand(string command);
        string ShowHelp();
        string ShowInventory();
        string Move(string dir);
        string TakeItem();
        Direction? GetDirectionFromString(string input);
    }
}
