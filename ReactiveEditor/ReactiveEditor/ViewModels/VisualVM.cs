using ReactiveUI;

namespace ReactiveEditor.ViewModels
{
    public abstract class VisualVM : ReactiveObject, IVisual
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

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set { this.RaiseAndSetIfChanged(ref isSelected, value); }
        }

        protected VisualVM() : this(null)
        {
        }

        protected VisualVM(VisualVM other)
        {
            //Recalculate Right when either Left or Width changed
            right = this.WhenAnyValue(x => x.Left, x => x.Width, (l, w) => l + w).ToProperty(this, x => x.Right);
            //Recaculate Bottom when either Top or Height changed
            bottom = this.WhenAnyValue(x => x.Top, x => x.Height, (t, h) => t + h).ToProperty(this, x => x.Bottom);
            Copy(other);
        }

        public abstract object Clone();

        public virtual void Copy(object from)
        {
            if (from is VisualVM other)
            {
                Left = other.Left;
                Top = other.Top;
                Height = other.Height;
                Width = other.Width;
            }
        }
    }
}