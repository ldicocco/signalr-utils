using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace  Ldc.SignalR
{
	public class HubExt : Hub
	{
		public Task _result(Guid id, Object result)
		{
			return null;
		}
	}
}
