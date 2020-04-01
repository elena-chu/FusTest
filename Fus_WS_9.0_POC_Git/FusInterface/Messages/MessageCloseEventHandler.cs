using System;

namespace Ws.Fus.Interfaces.Messages
{
	public delegate void MessageCloseEventHandler(object sender, MessageCloseEventArgs ea);

	public class MessageCloseEventArgs : EventArgs
	{
		public MessageCloseEventArgs(string messageId)
		{
			MessageId = messageId;
		}

		public readonly string MessageId;
	}
}