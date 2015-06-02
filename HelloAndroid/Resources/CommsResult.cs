using System;

namespace TouriDroid
{
	public class CommsResult
	{
		public int result { get; set; }
		public string message { get; set; }

		public CommsResult ()
		{
			message = "";
		}

		public CommsResult (int p_result, string p_message)
		{
			result = p_result;
			if (p_message != null) {
				message = p_message;
			} else {
				message = "";
			}
		}

		public CommsResult (int p_result)
		{
			result = p_result;
			message = "";
		}

		public void SetFail()
		{
			result = Constants.FAIL;
		}

		public void SetFail(string p_message)
		{
			result = Constants.FAIL;
			message = p_message;
		}

		public void SetSuccess()
		{
			result = Constants.SUCCESS;
		}

		public void SetSuccess(string p_message)
		{
			result = Constants.SUCCESS;
			message = p_message;
		}

		public bool IsFail()
		{
			return (result == Constants.FAIL);
		}
	
		public bool IsSuccess()
		{
			return (result == Constants.SUCCESS);
		}
	}
}

