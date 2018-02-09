namespace ReactiveEditor.ViewModels
{
    public interface IConnection : IVisual
    {
        IMovable FirstMovable { get; set; }
        IMovable SecondMovable { get; set; }
    }
}