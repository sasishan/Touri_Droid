using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Android.Util;

namespace TouriDroid
{
	public class Message 
	{
		public string fromUser {get;set;}
		public string message { get; set; }
	}

	public class ChatClient
	{
		private const string TAG = "ChatClient";
		public string _myUsername;
		public string _targetUserName;
		public string _myId;
		public bool   isConnected=false;

		public 	readonly HubConnection _connection;
		private IHubProxy _proxy;

		public event EventHandler<Message> OnMessageReceived;
		public event EventHandler<string> PingMe;

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
			Log.Debug (TAG, "In connect");	
			_proxy.On("messageReceived", (string fromUser, string message, string messageId) =>
				{
					if (OnMessageReceived != null)
					{
						//acknowledge to the server we received this message
						_proxy.Invoke("AcknowledgeMessage", messageId);
						Log.Debug (TAG, "Acknowledged message with id " + messageId);

						Message m = new Message ();
						m.fromUser = fromUser;
						m.message = message;
						OnMessageReceived(this, m);
					}
				});

			_proxy.On("PingClient", (string timestamp) =>
				{
					if (PingMe != null)
					{
						PingMe(this, timestamp);
					}
				});

			await _connection.Start();
			isConnected = true;

		//	await Send("Connected");
		}

		public void disconnect()
		{
			Log.Debug (TAG, "Disconnecting");
			_connection.Stop();

			_proxy = null;
		}

		public Task SendMyUsername()
		{
			return _proxy.Invoke("SendMyUserName");
		}

		public Task SendPrivateMessage(string message, string targetUsername)
		{
			Log.Debug (TAG, "Disconnecting");
			return _proxy.Invoke ("SendPrivateMessage", message, _myUsername, targetUsername);
		}

		public Task Send(string message)
		{
			return _proxy.Invoke("Send", _myUsername, message);
		}

		public Task PingServer()
		{
			Log.Debug (TAG, "In PingServer");
			isConnected = false;
			Task task = null;
			try
			{
				task = _proxy.Invoke ("PingClient", _myUsername);
			}
			catch (Exception e) {
				Log.Debug (TAG, e.Message);
			}
			return task;
		}
	}
}

