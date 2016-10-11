using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MusicPlayer.Controls
{
    public class MenuButton : BaseButton
    {
        private ContextMenu _contextMenu;

        public static DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(PlacementMode), typeof(MenuButton));

        public PlacementMode Placement
        {
            get { return (PlacementMode)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _contextMenu = ContextMenu;
            ContextMenu = null;
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (_contextMenu != null)
            {
                _contextMenu.PlacementTarget = this;
                _contextMenu.Placement = Placement;
                _contextMenu.IsOpen = true;
            }
        }
    }
}
