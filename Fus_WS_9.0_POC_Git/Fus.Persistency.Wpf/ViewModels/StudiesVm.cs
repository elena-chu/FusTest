using Ws.Dicom.Interfaces.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ws.Dicom.Persistency.UI.Wpf.ViewModels
{
    class StudiesVm : BindableBase
    {
        public ObservableCollection<StudyVm> Studies { get; } = new ObservableCollection<StudyVm>();

        public ICommand StudySelectedCommand { get; }

        public StudiesVm(ICommand studySelectedCommand)
        {
            StudySelectedCommand = studySelectedCommand;
        }

        public StudiesVm()
        {
            StudySelectedCommand = new DelegateCommand(() => { }); // nop
        }
    }
}
