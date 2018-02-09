namespace ReactiveEditor.ViewModels
{
    public interface IMovable : IVisual
    {
        double RotationAngle { get; set; }
        bool IsMoving { get; set; }

        void Rotate();
    }
}