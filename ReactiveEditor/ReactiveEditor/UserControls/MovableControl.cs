using ReactiveEditor.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ReactiveEditor.UserControls
{
    public abstract class MovableControl<T> : ReactiveUserControl<T> where T : ReactiveObject, IMovable
    {
        private Point transformedMouseDownLocation;
        private double oldX;
        private double oldY;

        //Used to transform MouseClick-Locations, not actually applied to the Control
        private readonly RotateTransform rotateTransform;

        protected MovableControl()
        {
            this.WhenAnyValue(x => x.DataContext).BindTo(this, x => x.ViewModel);
            rotateTransform = new RotateTransform();
            //Set up disposable subscriptions
            this.WhenActivated(d =>
            {
                //Update the rotatetransform when RotationAngle changes
                d.Invoke(this.Bind(this.ViewModel, x => x.RotationAngle, x => x.rotateTransform.Angle));
                //Hook up mouse events for movement handling
                d.Invoke(this.Events()
                    .MouseLeftButtonDown
                    .Where(e => e.ClickCount == 1)
                    .Subscribe(x => LeftMouseDownHandler(x)));
                d.Invoke(this.Events()
                    .MouseMove
                    .Sample(TimeSpan.FromMilliseconds(20)) // Limit mouse move updates
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(e => MouseMoveHandler(e)));
                d.Invoke(this.Events()
                    .MouseLeftButtonUp
                    .Subscribe(_ => LeftMouseUpHandler()));
                d.Invoke(this.Events()
                    .MouseLeave
                    .Subscribe(_ => MouseLeaveHandler()));
            });
        }

        private void LeftMouseDownHandler(MouseButtonEventArgs e)
        {
            oldX = ViewModel.Left;
            oldY = ViewModel.Top;
            var mousePoint = e.GetPosition(this);
            //Transform MousePoint with the rotation to get the real x,y Point
            transformedMouseDownLocation = rotateTransform.Transform(mousePoint);
            //Signal we are moving this instance manually
            ViewModel.IsMoving = true;
            //Capture the mouse so we dont drag the cursor out of the bounds of the control
            Mouse.Capture(this);
            //Bubble event based on Selection to allow for selection/movement
            e.Handled = ViewModel.IsSelected;
        }

        private void MouseMoveHandler(MouseEventArgs e)
        {
            var mousePoint = e.GetPosition(this);
            var transformedMousePoint = rotateTransform.Transform(mousePoint);
            if (ViewModel.IsMoving)
            {
                var deltaX = transformedMousePoint.X - transformedMouseDownLocation.X;
                var deltaY = transformedMousePoint.Y - transformedMouseDownLocation.Y;
                var newX = ViewModel.Left + deltaX;
                var newY = ViewModel.Top + deltaY;
                // If Left or Top were changed externally since the last update only allow movement in the opposite direction (clipping)
                if (oldX == ViewModel.Left || (oldX < ViewModel.Left && newX > oldX) || (oldX > ViewModel.Left && newX < oldX))
                {
                    ViewModel.Left = newX;
                    oldX = newX;
                }
                if (oldY == ViewModel.Top || (oldY < ViewModel.Top && newY > oldY) || (oldY > ViewModel.Top && newY < oldY))
                {
                    ViewModel.Top = newY;
                    oldY = newY;
                }
            }
        }

        private void LeftMouseUpHandler()
        {
            ViewModel.IsMoving = false;
            Mouse.Capture(null);
        }

        private void MouseLeaveHandler()
        {
            if (ViewModel.IsMoving)
                Mouse.Capture(this);
        }
    }
}