using ReactiveUI;
using System;

namespace ReactiveEditor.ViewModels
{
    public class CircleVM : MovableVM
    {
        public CircleVM()
        {
            this.WhenAnyValue(x => x.Width).Subscribe(w => this.Height = w);
            this.WhenAnyValue(x => x.Height).Subscribe(h => this.Width = h);
        }
    }
}