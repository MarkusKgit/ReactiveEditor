using ReactiveEditor.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;

namespace ReactiveEditor.ViewModels
{
    public class ConnectionVM : VisualVM, IConnection
    {
        private double lineWidth = 2;
        public double LineWidth
        {
            get { return lineWidth; }
            set { this.RaiseAndSetIfChanged(ref lineWidth, value); }
        }

        private readonly double[] allowedLineWidths = { 2, 3, 4, 5 };
        public double[] AllowedLinedWidths => allowedLineWidths;

        private IMovable firstMovable;
        public IMovable FirstMovable
        {
            get { return firstMovable; }
            set { this.RaiseAndSetIfChanged(ref firstMovable, value); }
        }

        private IMovable secondMovable;
        public IMovable SecondMovable
        {
            get { return secondMovable; }
            set { this.RaiseAndSetIfChanged(ref secondMovable, value); }
        }

        private List<Point> connectionPoints;
        public List<Point> ConnectionPoints
        {
            get { return connectionPoints; }
            set { this.RaiseAndSetIfChanged(ref connectionPoints, value); }
        }

        protected ConnectionVM() : this(null)
        {
        }

        protected ConnectionVM(ConnectionVM other) : base(other)
        {
        }

        public ConnectionVM(IMovable first, IMovable second) : this(null)
        {
            FirstMovable = first;
            SecondMovable = second;

            var firstChanged = this.WhenAnyValue(
                x => x.FirstMovable.Left,
                x => x.FirstMovable.Top,
                x => x.FirstMovable.Right,
                x => x.FirstMovable.Bottom,
                x => x.FirstMovable.RotationAngle)
                .ToUnit();
            var secondChanged = this.WhenAnyValue(
                x => x.SecondMovable.Left,
                x => x.SecondMovable.Top,
                x => x.SecondMovable.Right,
                x => x.SecondMovable.Bottom,
                x => x.SecondMovable.RotationAngle)
                .ToUnit();
            firstChanged
                .Merge(secondChanged)
                .Sample(TimeSpan.FromMilliseconds(100)) // 10 Hz update rate for the connections
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => RecalculateBounds());
        }

        private void RecalculateBounds()
        {
            var connectionCenter1 = new Point((firstMovable.Right + firstMovable.Left) / 2, (firstMovable.Top + firstMovable.Bottom) / 2);
            var connectionCenter2 = new Point((secondMovable.Right + secondMovable.Left) / 2, (secondMovable.Top + secondMovable.Bottom) / 2);
            var leftConnectionCenter = connectionCenter1.X < connectionCenter2.X ? connectionCenter1 : connectionCenter2;
            var rightConnectionCenter = connectionCenter1.X > connectionCenter2.X ? connectionCenter1 : connectionCenter2;
            var minLeft = Math.Min(connectionCenter1.X, connectionCenter2.X);
            var minTop = Math.Min(connectionCenter1.Y, connectionCenter2.Y);
            this.Width = Math.Abs(connectionCenter1.X - connectionCenter2.X);
            this.Height = Math.Abs(connectionCenter1.Y - connectionCenter2.Y);
            this.Left = minLeft;
            this.Top = minTop;
            if (leftConnectionCenter.Y < rightConnectionCenter.Y)
            {
                ConnectionPoints = new List<Point>
                {
                    new Point(0, 0),
                    new Point(Width, Height)
                };
            }
            else
            {
                ConnectionPoints = new List<Point>
                {
                    new Point(0, Height),
                    new Point(Width, 0)
                };
            }
        }

        public override object Clone()
        {
            return new ConnectionVM(this);
        }

        public override void Copy(object from)
        {
            base.Copy(from);
            if (from is ConnectionVM other)
            {
                LineWidth = other.LineWidth;
                FirstMovable = other.FirstMovable;
                SecondMovable = other.SecondMovable;
            }
        }

        public override string ToString()
        {
            if (ConnectionPoints != null && ConnectionPoints.Count == 2)
                return $"Connection x0: {ConnectionPoints[0].X} x1: {ConnectionPoints[1].X} y0: {ConnectionPoints[0].Y} y1: {ConnectionPoints[1].Y}";
            else
                return base.ToString();
        }
    }
}