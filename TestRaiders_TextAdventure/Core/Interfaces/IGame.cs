namespace TestRaiders_TextAdventure.Core.Interfaces
{
    internal interface IGame
    {
        void Start();
        void Quit();
        string ProcessCommand(string command);
        string ShowHelp();
        string ShowInventory();
        string Move(string dir);
        Direction? GetDirectionFromString(string input);
    }
}
