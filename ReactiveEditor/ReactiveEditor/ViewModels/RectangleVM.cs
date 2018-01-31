using ReactiveUI;
using System.Windows.Media;

namespace ReactiveEditor.ViewModels
{
    public class RectangleVM : MovableVM
    {
        private Color color = Colors.Blue;

        public Color Color
        {
            get { return color; }
            set { this.RaiseAndSetIfChanged(ref color, value); }
        }
    }
}