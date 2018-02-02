using ReactiveEditor.Helpers;
using ReactiveEditor.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ReactiveEditor.ViewModels
{
    public class MainWindowVM : ReactiveObject
    {
        private readonly Random rnd = new Random(DateTime.Now.Millisecond);

        private readonly INavigationService navigationService;

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

        public ReactiveCommand<IMovable, Unit> EditCommand { get; private set; }

        public ReactiveCommand DeselectAllCommand { get; private set; }

        public ReactiveCommand RotateSelectedCommand { get; private set; }

        public ReactiveCommand DuplicateSelectedCommand { get; private set; }

        public ReactiveCommand DeleteSelectedCommand { get; private set; }

        public ReactiveCommand ToggleSquareSpawnerCommand { get; private set; }

        public MainWindowVM(INavigationService navigationService = null)
        {
            this.navigationService = navigationService ?? Locator.CurrentMutable.GetService<INavigationService>();

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

            var oneSelected = SelectedMovables.CountChanged.Select(c => c == 1);
            EditCommand = ReactiveCommand.Create<IMovable>(param => EditMovable(param), oneSelected);
            //Some commands only make sense when something is selected
            var anythingSelected = SelectedMovables.CountChanged.Select(c => c > 0);
            DeselectAllCommand = ReactiveCommand.Create(DeselectAll, anythingSelected);
            RotateSelectedCommand = ReactiveCommand.Create(RotateSelected, anythingSelected);
            DuplicateSelectedCommand = ReactiveCommand.Create(DuplicateSelected, anythingSelected);
            DeleteSelectedCommand = ReactiveCommand.Create(DeleteSelected, anythingSelected);
            ToggleSquareSpawnerCommand = ReactiveCommand.Create(ToggleSquareSpawner, canCreateSquares);

            //When one item is moved manually move all the other selected movables too
            Movables.ItemChanged
                .Where(x => x.Sender.IsMoving)
                .Select(x => new { x.Sender, Pos = new Point(x.Sender.Left, x.Sender.Top) })
                .Buffer(2, 1)
                .Where(x => x[0].Sender == x[1].Sender)
                .Subscribe(x => MovableMoved(x[0].Sender, x[0].Pos, x[1].Pos));

            //Movables.ItemChanged
            //    .Where(x => x.PropertyName == nameof(IMovable.RotationAngle))
            //    .Select(x => x.Sender)
            //    .Subscribe(CheckBounds);
        }

        private void EditMovable(IMovable mv)
        {
            if (mv == null)
                return;
            var editVM = new EditVM { Title = "Edit " + mv.GetType().Name.TrimEnd("VM".ToCharArray()), EditableVM = mv };
            navigationService.ShowModalEditView(editVM);
        }

        private void AddCircle()
        {
            AddMovable(new CircleVM { Width = GetRandomDouble(1) * 50.0 + 20 });
        }

        private void AddSquare()
        {
            var randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            AddMovable(new SquareVM { Width = GetRandomDouble(1) * 50.0 + 20.0, Color = randomColor });
        }

        private void AddTriangle()
        {
            AddMovable(new TriangleVM { Height = GetRandomDouble(1) * 50.0 + 20.0, Width = GetRandomDouble(1) * 50 + 20 });
        }

        private void AddMovable(IMovable movable)
        {
            movable.Left = GetRandomDouble(1) * (drawAreaWidth > 0 ? (drawAreaWidth - movable.Width) : 200.0);
            movable.Top = GetRandomDouble(1) * (drawAreaHeight > 0 ? (drawAreaHeight - movable.Height) : 200.0);
            Movables.Add(movable);
        }

        private double GetRandomDouble(int precision)
        {
            if (precision < 0 || precision > 9)
                throw new ArgumentOutOfRangeException();
            int fac = (int)Math.Pow(10, precision);
            return rnd.Next(0, fac) / (fac * 1.0);
        }

        private void DuplicateSelected()
        {
            var newItems = new List<IMovable>();
            for (int i = selectedMovables.Count - 1; i >= 0; i--)
            {
                var oldItem = selectedMovables[i];
                var newItem = oldItem.Clone() as IMovable;
                oldItem.IsSelected = false;
                newItem.Left += 10;
                newItem.Top += 10;
                newItems.Add(newItem);
            }
            foreach (var item in newItems)
            {
                Movables.Add(item);
            }
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

            foreach (var item in selectedMovables.Where(x => x != sender))
            {
                item.Left = item.Left + deltaX;
                item.Top = item.Top + deltaY;
            }
            CheckBounds(sender);
        }

        private void CheckBounds(IMovable movedMovable)
        {
            var minX = double.PositiveInfinity;
            var minY = double.PositiveInfinity;
            var maxX = double.NegativeInfinity;
            var maxY = double.NegativeInfinity;
            foreach (var item in selectedMovables)
            {
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
                movedMovable.Left = movedMovable.Left + Math.Abs(minX);
            }

            if (minY < 0)
            {
                movedMovable.Top = movedMovable.Top + Math.Abs(minY);
            }

            if (maxX > drawAreaWidth)
            {
                movedMovable.Left = movedMovable.Left - (maxX - drawAreaWidth);
            }

            if (maxY > drawAreaHeight)
            {
                movedMovable.Top = movedMovable.Top - (maxY - drawAreaHeight);
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