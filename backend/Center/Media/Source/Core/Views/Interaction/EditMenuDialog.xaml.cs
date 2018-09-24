// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditMenuDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditMenuDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// Interaction logic for EditMenuDialog.xaml
    /// </summary>
    public partial class EditMenuDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditMenuDialog"/> class.
        /// </summary>
        public EditMenuDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the undo command.
        /// </summary>
        public ICommand UndoCommand
        {
            get
            {
                return new RelayCommand(this.Undo, this.CanExecuteUndo);
            }
        }

        /// <summary>
        /// Gets the redo command.
        /// </summary>
        public ICommand RedoCommand
        {
            get
            {
                return new RelayCommand(this.Redo, this.CanExecuteRedo);
            }
        }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(this.Delete, this.CanExecuteDelete);
            }
        }

        /// <summary>
        /// Gets the Cut command.
        /// </summary>
        public ICommand CutCommand
        {
            get
            {
                return new RelayCommand(this.Cut, this.CanExecuteCut);
            }
        }

        /// <summary>
        /// Gets the copy command.
        /// </summary>
        public ICommand CopyCommand
        {
            get
            {
                return new RelayCommand(this.Copy, this.CanExecuteCopy);
            }
        }

        /// <summary>
        /// Gets the paste command.
        /// </summary>
        public ICommand PasteCommand
        {
            get
            {
                return new RelayCommand(this.Paste, this.CanExecutePaste);
            }
        }

        /// <summary>
        /// Gets the select all command.
        /// </summary>
        public ICommand SelectAllCommand
        {
            get
            {
                return new RelayCommand(this.SelectAll, this.CanExecuteSelectAll);
            }
        }

        private bool CanExecuteUndo(object o)
        {
            return ApplicationCommands.Undo.CanExecute(null, this.UndoMenuItem);
        }

        private void Undo()
        {
            ApplicationCommands.Undo.Execute(null, this.UndoMenuItem);
            this.Close();
        }

        private bool CanExecuteRedo(object o)
        {
            return ApplicationCommands.Redo.CanExecute(null, this.UndoMenuItem);
        }

        private void Redo()
        {
            ApplicationCommands.Redo.Execute(null, this.UndoMenuItem);
            this.Close();
        }

        private bool CanExecuteDelete(object o)
        {
            var result = false;

            if (!(this.DataContext is EditMenuPrompt context))
            {
                return result;
            }

            result = context.DeleteSelectedLayoutElements.CanExecute(null);
            return result;
        }

        private void Delete()
        {
            if (!(this.DataContext is EditMenuPrompt context))
            {
                return;
            }

            context.DeleteSelectedLayoutElements.Execute(null);
            this.Close();
        }

        private void Cut()
        {
            ApplicationCommands.Cut.Execute(null, this);
            this.Close();
        }

        private bool CanExecuteCut(object o)
        {
            return ApplicationCommands.Copy.CanExecute(null, this);
        }

        private void Copy()
        {
            ApplicationCommands.Copy.Execute(null, this);
            this.Close();
        }

        private bool CanExecuteCopy(object o)
        {
            return ApplicationCommands.Copy.CanExecute(null, this);
        }

        private void Paste()
        {
            ApplicationCommands.Paste.Execute(null, this);
            this.Close();
        }

        private bool CanExecutePaste(object o)
        {
            return ApplicationCommands.Paste.CanExecute(null, this);
        }

        private bool CanExecuteSelectAll(object o)
        {
            return ApplicationCommands.SelectAll.CanExecute(null, this.UndoMenuItem);
        }

        private void SelectAll()
        {
            ApplicationCommands.SelectAll.Execute(null, this.UndoMenuItem);
            this.Close();
        }
    }
}
