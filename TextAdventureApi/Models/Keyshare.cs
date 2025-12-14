public class KeyShare
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Naam zoals game deze kamer noemt
    public string RoomId { get; set; } = default!;

    // Het deel van de decryptiesleutel
    public string Share { get; set; } = default!;

    // Minimum rol om te mogen ophalen
    public string MinRole { get; set; } = "Player"; // of "Admin"
}
