using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BanHang.Hubs
{
	public class ChatHub : Hub
	{
		public void SendMessage(string name, string message)
		{
			// Xử lý và lưu tin nhắn vào cơ sở dữ liệu ở đây

			// Gửi tin nhắn đến tất cả các client
			Clients.All.message(name, message);
		}
	}
}