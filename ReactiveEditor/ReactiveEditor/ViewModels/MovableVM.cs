using ReactiveUI;
using System.Linq;

namespace ReactiveEditor.ViewModels
{
    public abstract class MovableVM : ReactiveObject, IMovable
    {
        private double left;

        public double Left
        {
            get { return left; }
            set { this.RaiseAndSetIfChanged(ref left, value); }
        }

        private double top;

        public double Top
        {
            get { return top; }
            set { this.RaiseAndSetIfChanged(ref top, value); }
        }

        private double height;

        public double Height
        {
            get { return height; }
            set { this.RaiseAndSetIfChanged(ref height, value); }
        }

        private double width;

        public double Width
        {
            get { return width; }
            set { this.RaiseAndSetIfChanged(ref width, value); }
        }

        private readonly ObservableAsPropertyHelper<double> right;

        public double Right
        {
            get { return right.Value; }
        }

        private readonly ObservableAsPropertyHelper<double> bottom;

        public double Bottom
        {
            get { return bottom.Value; }
        }

        private double rotationAngle;

        public double RotationAngle
        {
            get { return rotationAngle; }
            set { this.RaiseAndSetIfChanged(ref rotationAngle, value); }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { this.RaiseAndSetIfChanged(ref isSelected, value); }
        }

        private bool isMoving;

        public bool IsMoving
        {
            get { return isMoving; }
            set { this.RaiseAndSetIfChanged(ref isMoving, value); }
        }

        public void Rotate()
        {
            this.RotationAngle += 22.5;
        }

        protected MovableVM()
        {
            right = this.WhenAnyValue(x => x.Left, x => x.Width, (l, w) => l + w).ToProperty(this, x => x.Right);
            bottom = this.WhenAnyValue(x => x.Top, x => x.Height, (t, h) => t + h).ToProperty(this, x => x.Bottom);
        }

        public override string ToString()
        {
            var typeName = this.GetType().ToString();
            if (typeName.Contains("."))
                typeName = typeName.Split('.').Last();
            return $"{typeName}: Left: {this.Left:F0}, Right: {this.Right:F0}";
        }
    }
}