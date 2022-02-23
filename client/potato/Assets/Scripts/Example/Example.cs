using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        var s = FindObjectOfType<Potato.Network.NetworkService>();
        s.Connect("127.0.0.1", 28888);

        var rpc = s.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
        rpc.OnNotification += (notification) =>
        {
            Debug.Log("Notification received");
        };

        Debug.Log("Request sent");
        var request = new Torikime.Chat.SendMessage.Request();
        request.Message = "Hello world";
        rpc.Request(request, (response) =>
            {
                Debug.Log("Response");
            });
    }

    void Update()
    {
        var s = FindObjectOfType<Potato.Network.NetworkService>();
        var rpc = s.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();

        Debug.Log("Request sent");
        StartCoroutine(rpc.RequestCoroutine(new Torikime.Chat.SendMessage.Request(), (response) =>
        {
            Debug.Log(response);
            // rpc.RequestAsync(new Torikime.Chat.SendMessage.Request()).ContinueWith((task) =>
            // {
            //     Debug.Log(task.Result);
            // }).Wait();
        }));
    }
}
