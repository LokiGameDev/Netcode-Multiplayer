
public interface IInteractable
{
    public string Prompt { get; set; }

    public void Interact(ulong clientID);
}
