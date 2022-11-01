namespace UI.MouseHover
{
    public interface IMouseHoverBehaviour
    {
        public void OnHoverEnter(Hoverable hoverable);
        public void OnHoverLeave(Hoverable hoverable);
    }
}