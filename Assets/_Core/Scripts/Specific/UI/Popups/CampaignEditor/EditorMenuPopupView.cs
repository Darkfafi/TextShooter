using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
	public class EditorMenuPopupView : BasePopupView
	{
		[SerializeField]
		private Button _newButton;

		[SerializeField]
		private Button _exitButton;

		private EditorMenuPopupModel _editorPopupModel;

		protected override void OnViewReady()
		{
			_editorPopupModel = MVCUtil.GetModel<EditorMenuPopupModel>(this);

			_newButton.onClick.AddListener(_editorPopupModel.NewCampaignEditorFile);
			_exitButton.onClick.AddListener(_editorPopupModel.ExitEditorScreen);
		}

		protected override void OnViewDestroy()
		{
			_newButton.onClick.RemoveAllListeners();
			_exitButton.onClick.RemoveAllListeners();

			_editorPopupModel = null;
		}
	}
}