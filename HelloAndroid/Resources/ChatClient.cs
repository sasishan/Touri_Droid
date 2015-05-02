using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace TouriDroid
{
	public class ChatClient
	{
		private readonly string _username;
		private readonly string _targetGuideId;

		private readonly HubConnection _connection;
		private readonly IHubProxy _proxy;

		public event EventHandler<string> OnMessageReceived;

		public ChatClient(string myUsername, string targetGuideId)
		{
			//_platform = platform;
			_username = myUsername;
			_targetGuideId = targetGuideId;
			_connection = new HubConnection(Constants.DEBUG_BASE_URL, "username=" + _username+"&targetGuideId="+targetGuideId);
			_proxy = _connection.CreateHubProxy("ChatHub");
		}

		public async Task Connect()
		{			
			await _connection.Start();

			_proxy.On("messageReceived", (string platform, string message) =>
				{
					if (OnMessageReceived != null)
						OnMessageReceived(this, string.Format("{0}: {1}", platform, message));
				});

			await Send("Connected");
		}

		public Task Send(string message)
		{
			return _proxy.Invoke("Send", _username, message);
		}
	}
}

