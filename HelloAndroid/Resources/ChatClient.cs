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
		public string _targetUserName;
		public string _myId;

		private readonly HubConnection _connection;
		private readonly IHubProxy _proxy;

		public event EventHandler<Message> OnMessageReceived;
		public event EventHandler<string> ReceiveMyUserName;

		public ChatClient(string myUsername, string targetUserName)
		{
			//_platform = platform;
			_myUsername = myUsername;
			_targetUserName = targetUserName;
			_connection = new HubConnection(Constants.DEBUG_BASE_URL, "username=" + _myUsername+"&targetUserName="+_targetUserName);
			_proxy = _connection.CreateHubProxy("ChatHub");
		}

		public async Task Connect()
		{			
			_proxy.On("messageReceived", (string fromUser, string message, string messageId) =>
				{
					if (OnMessageReceived != null)
					{
						//acknowledge to the server we received this message
						_proxy.Invoke("AcknowledgeMessage", messageId);

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

		public void disconnect()
		{
			_connection.Stop ();
		}

		public Task SendMyUsername()
		{
			return _proxy.Invoke("SendMyUserName");
		}

		public Task SendPrivateMessage(string message, string targetUsername)
		{
			while (true) {
				try {
					return _proxy.Invoke ("SendPrivateMessage", message, _myUsername, targetUsername);
				} catch (Exception e) {
					_connection.Start ();
				}
			}
		}

		public Task Send(string message)
		{
			return _proxy.Invoke("Send", _myUsername, message);
		}
	}
}

