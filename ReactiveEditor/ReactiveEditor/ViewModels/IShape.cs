using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReactiveEditor.ViewModels
{
    public interface IShape : IMovable
    {
        Color Color { get; set; }
    }
}