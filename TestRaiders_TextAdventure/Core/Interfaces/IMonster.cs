namespace TestRaiders_TextAdventure.Core.Interfaces
{
    public interface IMonster
    {
        string Name { get; }
        bool IsAlive { get; }
        int Attack();
        string Die();
    }
}
