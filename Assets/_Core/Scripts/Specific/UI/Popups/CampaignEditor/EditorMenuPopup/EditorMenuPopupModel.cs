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

		public ICampaignFilesHolder CampaignFilesHolder
		{
			get; private set;
		}

		public bool CanClose
		{
			get; private set;
		}

		public EditorMenuPopupModel(ICampaignFilesHolder filesHolder, bool canClose)
		{
			CanClose = canClose;
			CampaignFilesHolder = filesHolder;
		}

		public void NewCampaignEditorFile()
		{
			CampaignFilesHolder.LoadAsCurrentFile(CampaignEditorFile.CreateNew(), true);
			ForceClose();
		}

		public void ExitEditorScreen()
		{
			Debug.Log("Exit Editor!");
			ForceClose();
		}

		public void LoadFile(CampaignEditorFile fileToLoad)
		{
			CampaignFilesHolder.LoadAsCurrentFile(fileToLoad, false);
			ForceClose();
		}

		public void RemoveFile(CampaignEditorFile fileToRemove)
		{
			CampaignFilesHolder.RemoveFile(fileToRemove);
		}

		private void ForceClose()
		{
			CanClose = true;
			Close();
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