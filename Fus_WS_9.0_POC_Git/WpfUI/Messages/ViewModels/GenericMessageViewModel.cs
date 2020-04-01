using NirDobovizki.MvvmMonkey;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.ViewModels;
using Ws.Fus.Interfaces.Messages;

namespace WpfUI.Messages.ViewModels
{

	[TypeDescriptionProvider(typeof(MethodBinding))]
	public class GenericMessageViewModel : ViewModelBase
	{
		public GenericMessageViewModel()
		{
			Buttons = new ObservableCollection<GenericMessageButton>();
		}

		public Action CloseWindow;

		private GenericMessageType _messageType;
		public GenericMessageType MessageType
		{
			get { return _messageType; }
			set { Set(ref _messageType, value); }
		}

		private string _messageText;
		public string MessageText
		{
			get { return _messageText; }
			set { Set(ref _messageText, value); }
		}

		private bool _hasAction;
		public bool HasAction
		{
			get { return _hasAction; }
			set { Set(ref _hasAction, value); }
		}

		private string _actionText;
		public string ActionText
		{
			get { return _actionText; }
			set { Set(ref _actionText, value); }
		}

		private bool _actionChecked;
		public bool ActionChecked
		{
			get { return _actionChecked; }
			set { Set(ref _actionChecked, value); }
		}

		public ObservableCollection<GenericMessageButton> Buttons { get; private set; }

		private string _messageId;
		public string MessageId
		{
			get { return _messageId; }
			set { Set(ref _messageId, value); }
		}

		private GenericMessageReply _selectedButtonResult;
		public GenericMessageReply SelectedButtonResult
		{
			get { return _selectedButtonResult; }
			set { Set(ref _selectedButtonResult, value); }
		}

		public void SelectButton(object buttonResult)
		{
			SelectedButtonResult = (GenericMessageReply)buttonResult;
			CloseWindow();
		}

	}
}
