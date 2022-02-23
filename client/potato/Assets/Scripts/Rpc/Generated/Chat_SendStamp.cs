using System;
using System.Collections;
using System.Threading.Tasks;

namespace Torikime
{
	namespace Chat
	{
		namespace SendStamp
		{
			

			public class Rpc : Torikime.IRpc
			{


				public event Action<Notification> OnNotification;

			}
		}
	}
}