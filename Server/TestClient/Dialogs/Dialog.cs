using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using TestClient.GameService;

namespace TestClient
{
    public abstract class Dialog<T>
    {
        public void Show(DialogStyle style)
        {
            if (_isShowing)
                throw new InvalidOperationException();

            _isShowing = true;

            EnsurePopup(style);

            _popup.IsOpen = true;

            Application.Current.Host.Content.Resized += OnPluginSizeChanged;
        }

        public void Close()
        {
            _isShowing = false;

            if (_popup != null)
            {
                _popup.IsOpen = false;
                Application.Current.Host.Content.Resized -= OnPluginSizeChanged;
            }
            if (OnResponse != null)
                OnResponse(this, new DialogEventArgs<T> { Value = DialogContent.GetValue() });
        }

        protected abstract IDialogContent<T> DialogContent { get; }


        // Override this method to add your content to the dialog
        protected FrameworkElement GetContent()
        {
            // You could just use XamlReader to do everything except the event hookup.

            Grid grid = new Grid() { Width = 200, Height = 200, };
            Border border = new Border() { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(4), Background = new SolidColorBrush(Colors.White) };
            border.Child = (UIElement)this.DialogContent;
            grid.Children.Add(border);
            //grid.Children.Add(new TextBlock() { Text = "Dialog", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(8) });
            Button button = new Button() { Width = 50, Height = 24, Content = "OK", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(8) };
            grid.Children.Add(button);
            button.Click += (sender, args) => { Close(); };

            return grid;
        }

        public virtual event EventHandler<DialogEventArgs<T>> OnResponse;

        // Override this method if you want to do something (e.g. call Close) when you click
        // outside of the content
        protected virtual void OnClickOutside() { }

        // A Grid is the child of the Popup. If it is modal, it will contain a Canvas, which
        // will be sized to fill the plugin and prevent mouse interaction with the elements
        // outside of the popup. (Keyboard interaction is still possible, but hopefully when
        // Silverlight 2 RTMs, you can disable the root to take care of that.) The Grid isn't
        // strictly needed if there is always a Canvas, but it is handy for centering the content.
        //
        // The other child of the Grid is the content of the popup. This is obtained from the
        // GetContent method.

        private void EnsurePopup(DialogStyle style)
        {
            if (_popup != null)
                return;

            _popup = new Popup();
            _grid = new Grid();
            _popup.Child = _grid;

            if (style != DialogStyle.NonModal)
            {
                // If Canvas.Background != null, you cannot click through it

                _canvas = new Canvas();
                _canvas.MouseLeftButtonDown += (sender, args) => { OnClickOutside(); };

                if (style == DialogStyle.Modal)
                {
                    _canvas.Background = new SolidColorBrush(Colors.Transparent);
                }
                else if (style == DialogStyle.ModalDimmed)
                {
                    _canvas.Background = new SolidColorBrush(Color.FromArgb(0x20, 0x80, 0x80, 0x80));
                }

                _grid.Children.Add(_canvas);
            }

            _grid.Children.Add(_content = GetContent());

            UpdateSize();
        }

        private void OnPluginSizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            _grid.Width = Application.Current.Host.Content.ActualWidth;
            _grid.Height = Application.Current.Host.Content.ActualHeight;

            if (_canvas != null)
            {
                _canvas.Width = _grid.Width;
                _canvas.Height = _grid.Height;
            }
        }

        private bool _isShowing;
        private Popup _popup;
        private Grid _grid;
        private Canvas _canvas;
        private FrameworkElement _content;
    }

    public enum DialogStyle
    {
        NonModal,
        Modal,
        ModalDimmed
    };

    public class DialogEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
    }

    public class StamItem
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }

    public class DialogBidClass : Dialog<Bid?>
    {
        DialogBid cnt;
        public DialogBidClass()
        {
            cnt = new DialogBid();
        }

        protected override IDialogContent<Bid?> DialogContent
        {
            get { return cnt; }
        }
    }

}