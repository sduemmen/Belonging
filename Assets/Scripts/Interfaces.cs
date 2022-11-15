public interface IDisplayContext
{
    public bool DisplayContextActive { get; }
    
    public void ShowDisplayContext();
    public void HideDisplayContext();
}
