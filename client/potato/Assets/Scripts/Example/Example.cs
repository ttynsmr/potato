using UnityEngine;

public class Example : MonoBehaviour
{
    private Potato.Network.NetworkService networkService;

    private void Start()
    {
        networkService = FindObjectOfType<Potato.Network.NetworkService>();
        networkService.Connect("127.0.0.1", 28888);

        {
            var rpc = networkService.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
            rpc.OnNotification += (notification) =>
            {
                Debug.Log("Notification received: " + notification.From + "[" + notification.Message + "]");
            };

            Debug.Log("Request sent");
            var request = new Torikime.Chat.SendMessage.Request();
            request.Message = "Hello world";
            rpc.Request(request, (response) =>
                {
                    Debug.Log("Response");
                });
        }

        {
            var rpc = networkService.Session.GetRpc<Torikime.Example.UpdateMousePosition.Rpc>();
            rpc.OnNotification += (notification) =>
            {
                Debug.Log("Torikime.Example.UpdateMousePosition.Rpc: " + notification.Position);
            };
        }
    }

    private void Update()
    {
        {
            var rpc = networkService.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
            // Debug.Log("Request sent");
            var request = new Torikime.Chat.SendMessage.Request();
            request.Message = "Hello world2";
            StartCoroutine(rpc.RequestCoroutine(request, (response) =>
            {
                // Debug.Log("Torikime.Chat.SendMessage.Rpc: " + response.MessageId);
                // rpc.RequestAsync(new Torikime.Chat.SendMessage.Request()).ContinueWith((task) =>
                // {
                //     Debug.Log(task.Result);
                // }).Wait();
            }));
        }

        {
            var request = new Torikime.Diagnosis.SeverSessions.Request();
            var rpc = networkService.Session.GetRpc<Torikime.Diagnosis.SeverSessions.Rpc>();
            rpc.Request(request, (response) =>
                {
                    Debug.Log("Current session count is " + response.SessionCount);
                });
        }

        {
            var rpc = networkService.Session.GetRpc<Torikime.Example.UpdateMousePosition.Rpc>();
            // Debug.Log("Request sent");
            var request = new Torikime.Example.UpdateMousePosition.Request();
            request.Position = Input.mousePosition.ToVector3();
            StartCoroutine(rpc.RequestCoroutine(request, (response) => { }));
        }
    }

    private void OnDestroy()
    {
        networkService.Session.Disconnect();
    }
}
