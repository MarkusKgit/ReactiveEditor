namespace ReactiveEditor.ViewModels
{
    public class RectangleVM : ShapeVM
    {
        public RectangleVM() : this(null)
        {
        }

        public RectangleVM(RectangleVM other) : base(other)
        {
        }

        public override object Clone()
        {
            return new RectangleVM(this);
        }
    }
}