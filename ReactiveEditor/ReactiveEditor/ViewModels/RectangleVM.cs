using ReactiveUI;
using System.Windows.Media;

namespace ReactiveEditor.ViewModels
{
    public class RectangleVM : MovableVM
    {
        private Color color = Colors.Blue;

        public Color Color
        {
            get { return color; }
            set { this.RaiseAndSetIfChanged(ref color, value); }
        }

        public RectangleVM()
        {
        }

        public RectangleVM(RectangleVM other) : base(other)
        {
        }

        public override object Clone()
        {
            return new RectangleVM(this);
        }

        public override void Copy(object from)
        {
            base.Copy(from);
            if (from is RectangleVM other)
            {
                Color = other.Color;
            }
        }
    }
}