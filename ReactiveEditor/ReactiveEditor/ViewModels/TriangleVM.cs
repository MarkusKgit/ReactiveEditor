namespace ReactiveEditor.ViewModels
{
    public class TriangleVM : MovableVM
    {
        public TriangleVM()
        {
        }

        public TriangleVM(TriangleVM other) : base(other)
        {
        }

        public override object Clone()
        {
            return new TriangleVM(this);
        }

        public override void Copy(object from)
        {
            base.Copy(from);
        }
    }
}