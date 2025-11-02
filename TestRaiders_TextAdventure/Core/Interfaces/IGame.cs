namespace TestRaiders_TextAdventure.Core.Interfaces
{
    internal interface IGame
    {
        void Start();
        void ProcessCommand(string command);
        void ShowHelp();
        void Quit();

    }
}
