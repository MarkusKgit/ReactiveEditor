using ReactiveEditor.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private ReactiveList<IMovable> interPoints;
        public ReactiveList<IMovable> InterPoints
        {
            get { return interPoints; }
            set { this.RaiseAndSetIfChanged(ref interPoints, value); }
        }

        private List<Point> connectionPoints;
        public List<Point> ConnectionPoints
        {
            get { return connectionPoints; }
            private set { this.RaiseAndSetIfChanged(ref connectionPoints, value); }
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
            InterPoints = new ReactiveList<IMovable> { ChangeTrackingEnabled = true };
            InterPoints.Add(
                new SquareVM
                {
                    Left = (first.Left + second.Left) / 3,
                    Top = (first.Top + second.Top) / 3,
                    Width = 15,
                    Color = System.Windows.Media.Colors.Black
                }
                );
            InterPoints.Add(
                new CircleVM
                {
                    Left = (first.Left + second.Left) / 3 * 2,
                    Top = (first.Top + second.Top) / 3 * 2,
                    Width = 15,
                    Color = System.Windows.Media.Colors.Black
                }
                );

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
            var interPointsChanged = InterPoints.Changed.ToUnit().Merge(InterPoints.ItemChanged.ToUnit());
            firstChanged
                .Merge(secondChanged)
                .Merge(interPointsChanged)
                .Sample(TimeSpan.FromMilliseconds(100)) // 10 Hz update rate for the connections
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => RecalculateBounds());
        }

        private void RecalculateBounds()
        {
            var interPointCenters = InterPoints.Select(interPoint => new Point(interPoint.Left + interPoint.Width / 2, interPoint.Top + interPoint.Height / 2));
            var connectionPoints = new List<Point>();
            var connectionCenter1 = new Point((firstMovable.Right + firstMovable.Left) / 2, (firstMovable.Top + firstMovable.Bottom) / 2);
            var connectionCenter2 = new Point((secondMovable.Right + secondMovable.Left) / 2, (secondMovable.Top + secondMovable.Bottom) / 2);
            connectionPoints.Add(connectionCenter1);
            connectionPoints.AddRange(interPointCenters);
            connectionPoints.Add(connectionCenter2);

            var leftConnectionCenter = connectionCenter1.X < connectionCenter2.X ? connectionCenter1 : connectionCenter2;
            var rightConnectionCenter = connectionCenter1.X > connectionCenter2.X ? connectionCenter1 : connectionCenter2;
            var maxX = connectionPoints.Select(p => p.X).Max();
            var maxY = connectionPoints.Select(p => p.Y).Max();
            this.Width = maxX;
            this.Height = maxY;
            this.Left = 0;
            this.Top = 0;

            ConnectionPoints = new List<Point>(connectionPoints);
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