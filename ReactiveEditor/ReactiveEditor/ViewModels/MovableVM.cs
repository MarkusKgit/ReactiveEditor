using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveEditor.ViewModels
{
    public abstract class MovableVM : VisualVM, IMovable
    {
        private double rotationAngle;

        public double RotationAngle
        {
            get { return rotationAngle; }
            set { this.RaiseAndSetIfChanged(ref rotationAngle, value); }
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

        protected MovableVM() : this(null)
        {
        }

        protected MovableVM(MovableVM other) : base(other)
        {
            //Limit Rotation Angle to 0 <= r < 360
            this.WhenAnyValue(x => x.RotationAngle)
                .Where(r => r >= 360 || r < 0)
                .Subscribe(r => this.RotationAngle = Math.Abs(r) % 360);
        }

        public override string ToString()
        {
            var typeName = this.GetType().ToString();
            if (typeName.Contains("."))
                typeName = typeName.Split('.').Last();
            return $"{typeName}: x: {this.Left:F0}, y: {this.Top:F0}, w: {this.Width:F0}, h: {this.Height:F0} r: {this.RotationAngle:F1}";
        }

        public override void Copy(object from)
        {
            base.Copy(from);
            if (from is MovableVM other)
            {
                IsMoving = other.IsMoving;
                IsSelected = other.IsSelected;
            }
        }
    }
}