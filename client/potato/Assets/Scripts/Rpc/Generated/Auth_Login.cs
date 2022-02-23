using System;
using System.Collections;
using System.Threading.Tasks;

namespace Torikime
{
	namespace Auth
	{
		namespace Login
		{
			

			public class Rpc : Torikime.IRpc
			{
				public delegate void ResponseCallback(Response response);
				public void Request(Request request, ResponseCallback callback)
				{
					callback(new Response());
				}

                public IEnumerator RequestCoroutine(Request request, ResponseCallback callback)
                {
                    yield return null;
                    callback(new Response());
                }

                public async Task<Response> RequestAsync(Request request)
                {
                    await Task.Delay(1000);
                    return new Response();
                }



			}
		}
	}
}