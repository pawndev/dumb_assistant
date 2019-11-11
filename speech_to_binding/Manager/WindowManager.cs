using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace speech_to_binding.Manager
{
    public enum WindowAction
    {
        ALREADY_DONE,
        DONE
    }

    public class WindowManager
    {
        private MainWindow _window;

        public List<GridLength> originalColumnSize = new List<GridLength>();

        public string LastCommand
        {
            get
            {
                return this._window.lastCommand.Text;
            }
            set
            {
                this._window.lastCommand.Text = value;
            }
        }

        public WindowManager(MainWindow window)
        {
            this._window = window;
            foreach (ColumnDefinition col in this._window.mainGrid.ColumnDefinitions)
            {
                this.originalColumnSize.Add(col.Width);
            }
        }

        public WindowAction Minimize()
        {
            this._window.WindowState = WindowState.Minimized;
            return WindowAction.DONE;
        }

        public WindowAction Maximize()
        {
            this._window.WindowState = WindowState.Maximized;
            return WindowAction.DONE;
        }

        public WindowAction Normal()
        {
            this._window.WindowState = WindowState.Normal;
            return WindowAction.DONE;
        }

        public void Kill()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public WindowAction ShowCommands()
        {
            if (this._window.mainGrid.ColumnDefinitions[0].Width == this.originalColumnSize[0])
            {
                return WindowAction.ALREADY_DONE;
            }

            for (int index = 0; index < this.originalColumnSize.Count; index += 1)
            {
                this._window.mainGrid.ColumnDefinitions[index].Width = this.originalColumnSize[index];
            }

            return WindowAction.DONE;
        }

        public WindowAction UnshowCommands()
        {
            if (this._window.mainGrid.ColumnDefinitions[0].Width != this.originalColumnSize[0])
            {
                return WindowAction.ALREADY_DONE;
            }

            this._window.mainGrid.ColumnDefinitions[0].Width = new GridLength(this._window.Width - 5, GridUnitType.Star);
            this._window.mainGrid.ColumnDefinitions[1].Width = new GridLength(5, GridUnitType.Pixel);
            this._window.mainGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);

            return WindowAction.DONE;
        }
    }
}
