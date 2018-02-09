using ReactiveUI;
using System.Windows.Media;

namespace ReactiveEditor.ViewModels
{
    public abstract class ShapeVM : MovableVM, IShape
    {
        private Color color = Colors.Blue;

        public Color Color
        {
            get { return color; }
            set { this.RaiseAndSetIfChanged(ref color, value); }
        }

        protected ShapeVM(ShapeVM other) : base(other)
        {
        }

        public override void Copy(object from)
        {
            base.Copy(from);
            if (from is ShapeVM other)
            {
                Color = other.Color;
            }
        }
    }
}