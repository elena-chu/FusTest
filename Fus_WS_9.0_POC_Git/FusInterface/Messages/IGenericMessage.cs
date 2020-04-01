using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Fus.Interfaces.Messages
{
	public interface IGenericMessage
	{
		GenericMessageReply DisplayMessage(
				GenericMesssageReplyOptions options,
				MessageId MsgCode,
				MessageParam param1,
				MessageParam param2,
				MessageParam param3,
				MessageParam param4,
				MessageParam param5,
				MessageParam param6,
				MessageParam actionMsg,
				ref bool bActionApproved);

		GenericMessageReply DisplayMessage(
			GenericMesssageReplyOptions options,
			MessageId MsgCode,
			MessageParam param1,
			MessageParam param2,
			MessageParam param3,
			MessageParam param4,
			MessageParam param5,
			MessageParam param6);

		void CloseMessage(MessageId MsgCode);

		event MessageRequestedEventHandler MessageRequested;
		event MessageCloseEventHandler MessageClose;
	}
}
