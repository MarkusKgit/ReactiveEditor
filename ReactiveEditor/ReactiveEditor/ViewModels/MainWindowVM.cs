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

        private ReactiveList<IVisual> visuals;

        public ReactiveList<IVisual> Visuals
        {
            get { return visuals; }
            set { this.RaiseAndSetIfChanged(ref visuals, value); }
        }

        private ReactiveList<IVisual> selectedVisuals;

        public ReactiveList<IVisual> SelectedVisuals
        {
            get { return selectedVisuals; }
            set { this.RaiseAndSetIfChanged(ref selectedVisuals, value); }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<IShape>> shapes;

        public IEnumerable<IShape> Shapes
        {
            get { return shapes.Value; }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<IConnection>> connections;

        public IEnumerable<IConnection> Connections
        {
            get { return connections.Value; }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<IShape>> selectedShapes;

        public IEnumerable<IShape> SelectedShapes
        {
            get { return selectedShapes.Value; }
        }

        private readonly ObservableAsPropertyHelper<IEnumerable<IConnection>> selectedConnections;

        public IEnumerable<IConnection> SelectedConnections
        {
            get { return selectedConnections.Value; }
        }

        public ReactiveCommand AddCircleCommand { get; private set; }

        public ReactiveCommand AddSquareCommand { get; private set; }

        public ReactiveCommand AddTriangleCommand { get; private set; }

        public ReactiveCommand<IVisual, Unit> EditCommand { get; private set; }

        public ReactiveCommand DeselectAllCommand { get; private set; }

        public ReactiveCommand RotateSelectedCommand { get; private set; }

        public ReactiveCommand DuplicateSelectedCommand { get; private set; }

        public ReactiveCommand DeleteSelectedCommand { get; private set; }

        public ReactiveCommand ToggleSpawnerCommand { get; private set; }

        public ReactiveCommand ConnectCommand { get; private set; }

        public MainWindowVM(INavigationService navigationService = null)
        {
            this.navigationService = navigationService ?? Locator.CurrentMutable.GetService<INavigationService>();

            Visuals = new ReactiveList<IVisual>
            {
                ChangeTrackingEnabled = true
            };
            SelectedVisuals = new ReactiveList<IVisual>
            {
                ChangeTrackingEnabled = true
            };
            shapes = Visuals.CountChanged.Select(_ => Visuals.Where(x => x is IShape).Cast<IShape>()).ToProperty(this, x => x.Shapes);
            connections = Visuals.CountChanged.Select(_ => Visuals.Where(x => x is IConnection).Cast<IConnection>()).ToProperty(this, x => x.Connections);
            selectedShapes = SelectedVisuals.CountChanged.Select(_ => SelectedVisuals.Where(x => x is IShape).Cast<IShape>()).ToProperty(this, x => x.SelectedShapes);
            selectedConnections = SelectedVisuals.CountChanged.Select(_ => SelectedVisuals.Where(x => x is IConnection).Cast<IConnection>()).ToProperty(this, x => x.SelectedConnections);

            var shp1 = AddCircle();
            var shp2 = AddTriangle();
            var shp3 = AddSquare();
            AddConnection(shp1, shp2);
            AddConnection(shp2, shp3);

            AddCircleCommand = ReactiveCommand.Create(AddCircle);

            //Only allow Squares when there are at least 2 Circles
            var canCreateSquares = Visuals
                .Changed
                .ToUnit()
                .Select(_ => Visuals.Count(m => m is CircleVM) >= 2);
            AddSquareCommand = ReactiveCommand.Create(AddSquare, canCreateSquares);

            //Limit Triangles to Number of Squares / 2
            var canCreateTriangles = Visuals
                .Changed
                .ToUnit()
                .Select(_ => Visuals.Count(m => m is TriangleVM) < Visuals.Count(m => m is SquareVM) / 2)
                .StartWith(false);
            AddTriangleCommand = ReactiveCommand.Create(AddTriangle, canCreateTriangles);

            var oneSelected = SelectedVisuals.CountChanged.Select(c => c == 1);
            EditCommand = ReactiveCommand.Create<IVisual>(x => EditVisual(x), oneSelected);
            //Some commands only make sense when something is selected
            var anythingSelected = SelectedVisuals.CountChanged.Select(c => c > 0);
            DeselectAllCommand = ReactiveCommand.Create(DeselectAll, anythingSelected);
            RotateSelectedCommand = ReactiveCommand.Create(RotateSelected, anythingSelected);
            DuplicateSelectedCommand = ReactiveCommand.Create(DuplicateSelected, anythingSelected);
            DeleteSelectedCommand = ReactiveCommand.Create(DeleteSelected, anythingSelected);
            ToggleSpawnerCommand = ReactiveCommand.Create(ToggleSpawner);

            var canConnect = SelectedVisuals.CountChanged.Select(_ => SelectedShapes != null && SelectedShapes.Count() == 2).StartWith(false);
            ConnectCommand = ReactiveCommand.Create(ConnectVisuals, canConnect);

            //When one item is moved manually move all the other selected movables too
            Visuals.ItemChanged
                .Where(x => x.Sender is IShape && (x.Sender as IShape).IsMoving)
                .Select(x => new { Sender = x.Sender as IShape, Pos = new Point(x.Sender.Left, x.Sender.Top) })
                .Buffer(2, 1)
                .Where(x => x[0].Sender == x[1].Sender)
                .Subscribe(x => MovableMoved(x[0].Sender, x[0].Pos, x[1].Pos));
        }

        private void ConnectVisuals()
        {
            if (SelectedShapes == null || SelectedShapes.Count() != 2)
                return;
            AddConnection(SelectedShapes.First(), SelectedShapes.Last());
        }

        private void EditVisual(IVisual visual)
        {
            if (visual == null)
                return;
            var editVM = new EditVM { Title = "Edit " + visual.GetType().Name.TrimEnd("VM".ToCharArray()), EditableVM = visual };
            navigationService.ShowModalEditView(editVM);
        }

        private CircleVM AddCircle()
        {
            return AddShape(new CircleVM { Width = GetRandomDouble(1) * 150.0 + 10.0 }) as CircleVM;
        }

        private SquareVM AddSquare()
        {
            return AddShape(new SquareVM { Width = GetRandomDouble(1) * 50.0 + 10.0 }) as SquareVM;
        }

        private TriangleVM AddTriangle()
        {
            return AddShape(new TriangleVM { Height = GetRandomDouble(1) * 100.0 + 10.0, Width = GetRandomDouble(1) * 100 + 10.0 }) as TriangleVM;
        }

        private IShape AddShape(IShape shape)
        {
            var randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
            shape.Color = randomColor;
            shape.Left = GetRandomDouble(1) * (drawAreaWidth > 0 ? (drawAreaWidth - shape.Width) : 200.0);
            shape.Top = GetRandomDouble(1) * (drawAreaHeight > 0 ? (drawAreaHeight - shape.Height) : 200.0);
            Visuals.Add(shape);
            return shape;
        }

        private void AddConnection(IShape first, IShape second)
        {
            var connection = new ConnectionVM(first, second);
            Visuals.Insert(0, connection);
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
            var selectedShapes = SelectedShapes.ToList();
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
                Visuals.Add(item);
            }
        }

        private void DeleteSelected()
        {
            Visuals.RemoveAll(new List<IConnection>(SelectedConnections));
            var connectionsToRemove = new List<IConnection>();
            foreach (var shape in SelectedShapes)
            {
                var conns = Connections.Where(x => x.FirstMovable == shape || x.SecondMovable == shape);
                if (conns != null && conns.Count() > 0)
                    connectionsToRemove.AddRange(conns);
            }
            Visuals.RemoveAll(connectionsToRemove);
            Visuals.RemoveAll(new List<IShape>(SelectedShapes));
        }

        private void DeselectAll()
        {
            if (SelectedVisuals == null || SelectedVisuals.Count < 1)
                return;
            for (int i = SelectedVisuals.Count - 1; i >= 0; i--)
            {
                Visuals[i].IsSelected = false;
            }
            SelectedVisuals.Clear();
        }

        private void MovableMoved(IShape sender, Point oldPos, Point newPos)
        {
            var deltaX = newPos.X - oldPos.X;
            var deltaY = newPos.Y - oldPos.Y;
            if (Math.Abs(deltaX) < 1e-6 && Math.Abs(deltaY) < 1e-6)
                return;

            foreach (var item in SelectedShapes.Where(x => x != sender))
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
            foreach (var item in selectedVisuals)
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
            foreach (var item in SelectedShapes)
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