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

        private ReactiveList<IShape> shapes;

        public ReactiveList<IShape> Shapes
        {
            get { return shapes; }
            set { this.RaiseAndSetIfChanged(ref shapes, value); }
        }

        private ReactiveList<IShape> selectedShapes;

        public ReactiveList<IShape> SelectedShapes
        {
            get { return selectedShapes; }
            set { this.RaiseAndSetIfChanged(ref selectedShapes, value); }
        }

        public ReactiveCommand AddCircleCommand { get; private set; }

        public ReactiveCommand AddSquareCommand { get; private set; }

        public ReactiveCommand AddTriangleCommand { get; private set; }

        public ReactiveCommand<IShape, Unit> EditCommand { get; private set; }

        public ReactiveCommand DeselectAllCommand { get; private set; }

        public ReactiveCommand RotateSelectedCommand { get; private set; }

        public ReactiveCommand DuplicateSelectedCommand { get; private set; }

        public ReactiveCommand DeleteSelectedCommand { get; private set; }

        public ReactiveCommand ToggleSpawnerCommand { get; private set; }

        public MainWindowVM(INavigationService navigationService = null)
        {
            this.navigationService = navigationService ?? Locator.CurrentMutable.GetService<INavigationService>();

            Shapes = new ReactiveList<IShape>
            {
                ChangeTrackingEnabled = true
            };
            SelectedShapes = new ReactiveList<IShape>
            {
                ChangeTrackingEnabled = true
            };

            AddCircleCommand = ReactiveCommand.Create(AddCircle);

            //Only allow Squares when there are at least 2 Circles
            var canCreateSquares = Shapes
                .Changed
                .ToUnit()
                .Select(_ => Shapes.Count(m => m is CircleVM) >= 2);
            AddSquareCommand = ReactiveCommand.Create(AddSquare, canCreateSquares);

            //Limit Triangles to Number of Squares / 2
            var canCreateTriangles = Shapes
                .Changed
                .ToUnit()
                .Select(_ => Shapes.Count(m => m is TriangleVM) < Shapes.Count(m => m is SquareVM) / 2)
                .StartWith(false);
            AddTriangleCommand = ReactiveCommand.Create(AddTriangle, canCreateTriangles);

            var oneSelected = SelectedShapes.CountChanged.Select(c => c == 1);
            EditCommand = ReactiveCommand.Create<IShape>(param => EditShape(param), oneSelected);
            //Some commands only make sense when something is selected
            var anythingSelected = SelectedShapes.CountChanged.Select(c => c > 0);
            DeselectAllCommand = ReactiveCommand.Create(DeselectAll, anythingSelected);
            RotateSelectedCommand = ReactiveCommand.Create(RotateSelected, anythingSelected);
            DuplicateSelectedCommand = ReactiveCommand.Create(DuplicateSelected, anythingSelected);
            DeleteSelectedCommand = ReactiveCommand.Create(DeleteSelected, anythingSelected);
            ToggleSpawnerCommand = ReactiveCommand.Create(ToggleSpawner);

            //When one item is moved manually move all the other selected movables too
            Shapes.ItemChanged
                .Where(x => x.Sender.IsMoving)
                .Select(x => new { x.Sender, Pos = new Point(x.Sender.Left, x.Sender.Top) })
                .Buffer(2, 1)
                .Where(x => x[0].Sender == x[1].Sender)
                .Subscribe(x => MovableMoved(x[0].Sender, x[0].Pos, x[1].Pos));
        }

        private void EditShape(IShape shape)
        {
            if (shape == null)
                return;
            var editVM = new EditVM { Title = "Edit " + shape.GetType().Name.TrimEnd("VM".ToCharArray()), EditableVM = shape };
            navigationService.ShowModalEditView(editVM);
        }

        private void AddCircle()
        {
            AddShape(new CircleVM { Width = GetRandomDouble(1) * 150.0 + 10.0 });
        }

        private void AddSquare()
        {
            AddShape(new SquareVM { Width = GetRandomDouble(1) * 50.0 + 10.0 });
        }

        private void AddTriangle()
        {
            AddShape(new TriangleVM { Height = GetRandomDouble(1) * 100.0 + 10.0, Width = GetRandomDouble(1) * 100 + 10.0 });
        }

        private void AddShape(IShape shape)
        {
            var randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            shape.Color = randomColor;
            shape.Left = GetRandomDouble(1) * (drawAreaWidth > 0 ? (drawAreaWidth - shape.Width) : 200.0);
            shape.Top = GetRandomDouble(1) * (drawAreaHeight > 0 ? (drawAreaHeight - shape.Height) : 200.0);
            Shapes.Add(shape);
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
            var newItems = new List<IShape>();
            for (int i = selectedShapes.Count - 1; i >= 0; i--)
            {
                var oldItem = selectedShapes[i];
                var newItem = oldItem.Clone() as IShape;
                oldItem.IsSelected = false;
                newItem.Left += 10;
                newItem.Top += 10;
                newItems.Add(newItem);
            }
            foreach (var item in newItems)
            {
                Shapes.Add(item);
            }
        }

        private void DeleteSelected()
        {
            Shapes.RemoveAll(new List<IShape>(selectedShapes));
        }

        private void DeselectAll()
        {
            if (SelectedShapes == null || SelectedShapes.Count < 1)
                return;
            for (int i = SelectedShapes.Count - 1; i >= 0; i--)
            {
                Shapes[i].IsSelected = false;
            }
            SelectedShapes.Clear();
        }

        private void MovableMoved(IMovable sender, Point oldPos, Point newPos)
        {
            var deltaX = newPos.X - oldPos.X;
            var deltaY = newPos.Y - oldPos.Y;
            if (Math.Abs(deltaX) < 1e-6 && Math.Abs(deltaY) < 1e-6)
                return;

            foreach (var item in selectedShapes.Where(x => x != sender))
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
            foreach (var item in selectedShapes)
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
            foreach (var item in selectedShapes)
            {
                item.Rotate();
            }
        }

        private void CreateRandomMovable()
        {
            var roll = rnd.Next(0, 2);
            switch (roll)
            {
                case 0:
                    AddCircle();
                    break;
                case 1:
                    AddTriangle();
                    break;
                case 2:
                    AddSquare();
                    break;
                default:
                    break;
            }
        }

        private void ToggleSpawner()
        {
            if (rectangleSpawner == null)
            {
                rectangleSpawner = Observable
                .Interval(TimeSpan.FromMilliseconds(500))
                .ToUnit()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => CreateRandomMovable());
            }
            else
            {
                rectangleSpawner.Dispose();
                rectangleSpawner = null;
            }
        }
    }
}