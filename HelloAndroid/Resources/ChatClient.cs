using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace TouriDroid
{
	public class Message 
	{
		public string fromUser {get;set;}
		public string message { get; set; }
	}

	public class ChatClient
	{
		public string _myUsername;
		public string _targetGuideId;


		private readonly HubConnection _connection;
		private readonly IHubProxy _proxy;

		public event EventHandler<Message> OnMessageReceived;
		public event EventHandler<string> ReceivedMyConnectionId;
		public event EventHandler<string> ReceiveMyUserName;

		public ChatClient(string myUsername, string targetGuideId)
		{
			//_platform = platform;
			_myUsername = myUsername;
			_targetGuideId = targetGuideId;
			_connection = new HubConnection(Constants.DEBUG_BASE_URL, "username=" + _myUsername+"&targetGuideId="+targetGuideId);
			_proxy = _connection.CreateHubProxy("ChatHub");
		}

		public async Task Connect()
		{			
			_proxy.On("messageReceived", (string fromUser, string message) =>
				{
					if (OnMessageReceived != null)
					{
						Message m = new Message ();
						m.fromUser = fromUser;
						m.message = message;
						OnMessageReceived(this, m);
					}
				});

			_proxy.On("receiveMyUserName", (string username) =>
				{
					if (ReceiveMyUserName != null)
						ReceiveMyUserName(this, username);
				});

			await _connection.Start();

		//	await Send("Connected");
		}

		public Task SendMyUsername()
		{
			return _proxy.Invoke("SendMyUserName");
		}

		public Task SendPrivateMessage(string message)
		{
			return _proxy.Invoke("SendPrivateMessage", message, _myUsername, _targetGuideId);
		}

		public Task Send(string message)
		{
			return _proxy.Invoke("Send", _myUsername, message);
		}
	}
}

