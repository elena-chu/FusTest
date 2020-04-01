using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Overlays
{
	public interface IUiModeChanges
	{
        event UiModeChangedEventHandler ModeChanged;
        event CanEnterUiModeChangedEventHandler CanEnterModeChanged;

        event LayerVisiblityChangedEventHandler LayerVisiblityChanged;
        event CanShowHideLayerChangedEventHandler CanShowHideLayerChanged;


        void EnterMode(UiMode newMode, string subGuiMode);
		void ExitMode(UiMode modeToLeave);
		bool CanEnterMode(UiMode newMode, string subGuiMode);
		UiMode GetMode();

		void ShowLayer(UiMode layer);
		void HideLayer(UiMode layer);
		bool CanShowHideLayer(UiMode layer);
		bool IsLayerVisible(UiMode layer);

        /** Delete overlays functionality ---------------------*/
        event SelectedOverlaysChangedEventHandler SelectedOverlaysChanged;

        bool IsDeleteEnabled();
        bool IsDeleteAllEnabled();
        void Delete();
        void DeleteAll();
        /* -----------------------------------------------------*/
    }
}
