using ReactiveEditor.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ReactiveEditor.ViewModels
{
    public class MainWindowVM : ReactiveObject
    {
        private readonly Random rnd = new Random(DateTime.Now.Millisecond);

        private IDisposable rectangleSpawner;

        private double drawAreaWidth;

        public double DrawAreaWidth
        {
            get { return drawAreaWidth; }
            set { this.RaiseAndSetIfChanged(ref drawAreaWidth, value); }
        }

        private double drawAreaHeight;

        public double DrawAreaHeight
        {
            get { return drawAreaHeight; }
            set { this.RaiseAndSetIfChanged(ref drawAreaHeight, value); }
        }

        private ReactiveList<IMovable> movables;

        public ReactiveList<IMovable> Movables
        {
            get { return movables; }
            set { this.RaiseAndSetIfChanged(ref movables, value); }
        }

        private ReactiveList<IMovable> selectedMovables;

        public ReactiveList<IMovable> SelectedMovables
        {
            get { return selectedMovables; }
            set { this.RaiseAndSetIfChanged(ref selectedMovables, value); }
        }

        public ReactiveCommand AddCircleCommand { get; private set; }

        public ReactiveCommand AddSquareCommand { get; private set; }

        public ReactiveCommand AddTriangleCommand { get; private set; }

        public ReactiveCommand DeselectAllCommand { get; private set; }

        public ReactiveCommand RotateSelectedCommand { get; private set; }

        public ReactiveCommand DeleteSelectedCommand { get; private set; }

        public ReactiveCommand ToggleSquareSpawnerCommand { get; private set; }

        public MainWindowVM()
        {
            Movables = new ReactiveList<IMovable>
            {
                ChangeTrackingEnabled = true
            };
            SelectedMovables = new ReactiveList<IMovable>
            {
                ChangeTrackingEnabled = true
            };
            AddCircle();
            AddSquare();
            AddTriangle();

            //Limit Circles to 5
            var canCreateCircles = Movables.Changed.ToUnit().Select(_ => Movables.Count(m => m is CircleVM) < 5).StartWith(true);
            AddCircleCommand = ReactiveCommand.Create(AddCircle, canCreateCircles);

            //Only allow Squares when there are at least 2 Circles
            var canCreateSquares = Movables.Changed.ToUnit().Select(_ => Movables.Count(m => m is CircleVM) >= 2);
            AddSquareCommand = ReactiveCommand.Create(AddSquare, canCreateSquares);

            AddTriangleCommand = ReactiveCommand.Create(AddTriangle);

            //Some commands only make sense when something is selected
            var anythingSelected = SelectedMovables.CountChanged.Select(c => c > 0);
            DeselectAllCommand = ReactiveCommand.Create(DeselectAll, anythingSelected);
            RotateSelectedCommand = ReactiveCommand.Create(RotateSelected, anythingSelected);
            DeleteSelectedCommand = ReactiveCommand.Create(DeleteSelected, anythingSelected);
            ToggleSquareSpawnerCommand = ReactiveCommand.Create(ToggleSquareSpawner, canCreateSquares);

            //When one item is moved manually move all the other selected movables too
            Movables.ItemChanged
                .Where(x => x.Sender.IsMoving)
                .Select(x => new { x.Sender, Pos = new Point(x.Sender.Left, x.Sender.Top) })
                .Buffer(2, 1)
                .Where(x => x[0].Sender == x[1].Sender)
                .Subscribe(x => MovableMoved(x[0].Sender, x[0].Pos, x[1].Pos));
        }

        private void AddCircle()
        {
            AddMovable(new CircleVM { Width = rnd.NextDouble() * 50 + 20 });
        }

        private void AddSquare()
        {
            var randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            AddMovable(new SquareVM { Width = rnd.NextDouble() * 50 + 20, Color = randomColor });
        }

        private void AddTriangle()
        {
            AddMovable(new TriangleVM { Height = rnd.NextDouble() * 50 + 20, Width = rnd.NextDouble() * 50 + 20 });
        }

        private void AddMovable(IMovable movable)
        {
            movable.Left = rnd.NextDouble() * (drawAreaWidth > 0 ? (drawAreaWidth - movable.Width) : 200);
            movable.Top = rnd.NextDouble() * (drawAreaHeight > 0 ? (drawAreaHeight - movable.Height) : 200);
            Movables.Add(movable);
        }

        private void DeleteSelected()
        {
            Movables.RemoveAll(new List<IMovable>(selectedMovables));
        }

        private void DeselectAll()
        {
            if (SelectedMovables == null || SelectedMovables.Count < 1)
                return;
            for (int i = SelectedMovables.Count - 1; i >= 0; i--)
            {
                Movables[i].IsSelected = false;
            }
            SelectedMovables.Clear();
        }

        private void MovableMoved(IMovable sender, Point oldPos, Point newPos)
        {
            var deltaX = newPos.X - oldPos.X;
            var deltaY = newPos.Y - oldPos.Y;
            if (Math.Abs(deltaX) < 1e-6 && Math.Abs(deltaY) < 1e-6)
                return;

            var minX = double.PositiveInfinity;
            var minY = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var maxY = double.NegativeInfinity;
            foreach (var item in selectedMovables)
            {
                if (item != sender)
                {
                    item.Left = item.Left + deltaX;
                    item.Top = item.Top + deltaY;
                }
                if (item.Left < minX)
                    minX = item.Left;
                if (item.Top < minY)
                    minY = item.Top;
                if (item.Right > maxX)
                    maxX = item.Right;
                if (item.Bottom > maxY)
                    maxY = item.Bottom;
            }

            if (minX < 1e-6)
            {
                sender.Left = sender.Left + Math.Abs(minX);
            }

            if (minY < 0)
            {
                sender.Top = sender.Top + Math.Abs(minY);
            }

            if (maxX > drawAreaWidth)
            {
                sender.Left = sender.Left - (maxX - drawAreaWidth);
            }

            if (maxY > drawAreaHeight)
            {
                sender.Top = sender.Top - (maxY - drawAreaHeight);
            }
        }

        private void RotateSelected()
        {
            foreach (var item in selectedMovables)
            {
                item.Rotate();
            }
        }

        private void ToggleSquareSpawner()
        {
            if (rectangleSpawner == null)
            {
                rectangleSpawner = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ToUnit()
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(AddSquareCommand);
            }
            else
            {
                rectangleSpawner.Dispose();
                rectangleSpawner = null;
            }
        }
    }
}