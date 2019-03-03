using UnityEngine;

namespace GameEditor
{
	public class EditorMenuPopupModel : BasePopupModel
	{
		[PopupID]
		public const string POPUP_ID = "EditorMenu";

		public override string PopupModelID
		{
			get
			{
				return POPUP_ID;
			}
		}

		public bool CanClose
		{
			get; private set;
		}

		private ICampaignFilesHolder _campaignFilesHolder;

		public EditorMenuPopupModel(ICampaignFilesHolder filesHolder, bool canClose)
		{
			CanClose = canClose;
			_campaignFilesHolder = filesHolder;
		}

		public void NewCampaignEditorFile()
		{
			_campaignFilesHolder.LoadAsCurrentFile(new CampaignEditorFile());
		}

		public void ExitEditorScreen()
		{
			Debug.Log("Exit Editor!");
		}

		protected override bool CanEnterState(PopupModelState state)
		{
			if(state == PopupModelState.Closed)
			{
				return CanClose;
			}

			return base.CanEnterState(state);
		}
	}
}