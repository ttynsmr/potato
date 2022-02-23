using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        var s = FindObjectOfType<Potato.Network.NetworkService>();
        var rpc = s.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
        rpc.OnNotification += (notification) =>
        {
            Debug.Log("Notification received");
        };

        rpc.Request(new Torikime.Chat.SendMessage.Request(), (response) =>
            {
                Debug.Log("Response");
            });
    }

    void Update()
    {
        var s = FindObjectOfType<Potato.Network.NetworkService>();
        var rpc = s.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
        StartCoroutine(rpc.RequestCoroutine(new Torikime.Chat.SendMessage.Request(), (response) =>
        {
            Debug.Log(response);
            rpc.RequestAsync(new Torikime.Chat.SendMessage.Request()).ContinueWith((task) =>
            {
                Debug.Log(task.Result);
            }).Wait();
        }));
    }
}
