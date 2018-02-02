using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveEditor.ViewModels
{
    public class EditVM : ReactiveObject
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { this.RaiseAndSetIfChanged(ref title, value); }
        }

        private IDuplicatable editableVMCopy;

        public IDuplicatable EditableVMCopy
        {
            get { return editableVMCopy; }
            set { this.RaiseAndSetIfChanged(ref editableVMCopy, value); }
        }

        private IDuplicatable editableVM;

        public IDuplicatable EditableVM
        {
            get { return editableVM; }
            set { this.RaiseAndSetIfChanged(ref editableVM, value); }
        }

        public EditVM()
        {
            this.WhenAnyValue(x => x.EditableVM).Where(x => x != null).Subscribe(_ => EditableVMCopy = EditableVM?.Clone() as IDuplicatable);
        }

        public void ApplyChanges()
        {
            EditableVM.Copy(editableVMCopy);
        }
    }
}