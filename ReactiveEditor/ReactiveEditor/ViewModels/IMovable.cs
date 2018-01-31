namespace ReactiveEditor.ViewModels
{
    public interface IMovable
    {
        double Left { get; set; }
        double Top { get; set; }
        double Right { get; }
        double Bottom { get; }
        double Height { get; set; }
        double Width { get; set; }

        double RotationAngle { get; set; }
        bool IsSelected { get; set; }
        bool IsMoving { get; set; }

        void Rotate();
    }
}