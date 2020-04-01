namespace Ws.Fus.Interfaces.Messages
{
	public struct MessageParam
	{
		public MessageParam(int value)
		{
			_id = 0;
			_paramValue = value.ToString();
		}

		public MessageParam(float value)
		{
			_id = 0;
			_paramValue = value.ToString();
		}
		public MessageParam(double value)
		{
			_id = 0;
			_paramValue = value.ToString();
		}

		public MessageParam(float value, int n)
		{
			_id = 0;
			_paramValue = value.ToString($"F{n.ToString()}");
		}
		public MessageParam(double value, int n)
		{
			_id = 0;
			_paramValue = value.ToString($"F{n.ToString()}");
		}
		public MessageParam(MessageId id)
		{
			_id = id;
			_paramValue = null;
		}

		/// <summary>
		/// Do NOT use this in regular cases!
		/// Can be used for strings that were already loaded from string table
		/// or strings like "ini key" name that we want to display "as is" to the user.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static MessageParam FromStringParam(string value)
		{
			return new MessageParam { _id = 0, _paramValue = value };
		}

		private MessageId _id;
		private string _paramValue;

		public MessageId Id { get { return _id; } }
		public string ParamValue { get { return _paramValue; } }
	}
}