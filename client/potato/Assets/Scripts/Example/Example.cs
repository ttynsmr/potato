using System.Collections.Generic;
using UnityEngine;

namespace Potato
{
    public class Example : MonoBehaviour
    {
        private Potato.Network.NetworkService networkService;

        public GameObject trailPrefab;

        private GameObject myTrail;

        private Dictionary<int, GameObject> trails = new Dictionary<int, GameObject>();

        private void Start()
        {
            myTrail = Instantiate(trailPrefab);

            networkService = FindObjectOfType<Potato.Network.NetworkService>();
            networkService.Connect("127.0.0.1", 28888);

            {
                var sendMessage = networkService.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
                sendMessage.OnNotification += (notification) =>
                {
                    Debug.Log("Notification received: " + notification.From + "[" + notification.Message + "]");
                };

                Debug.Log("Request sent");
                var request = new Torikime.Chat.SendMessage.Request();
                request.Message = "Hello world";
                sendMessage.Request(request, (response) =>
                    {
                        Debug.Log("Response");
                    });
            }

            {
                var updateMousePosition = networkService.Session.GetRpc<Torikime.Example.UpdateMousePosition.Rpc>();
                updateMousePosition.OnNotification += (notification) =>
                {
                    var trail = trails[notification.SessionId];
                    if (trail == null)
                    {
                        return;
                    }
                    trail.transform.position = notification.Position.ToVector3();
                };

                var spawn = networkService.Session.GetRpc<Torikime.Example.Spawn.Rpc>();
                spawn.OnNotification += (notification) =>
                {
                    trails.Add(notification.SessionId, Instantiate(trailPrefab));
                    trails[notification.SessionId].transform.position = notification.Position.ToVector3();
                    trails[notification.SessionId].name = "Trail " + notification.SessionId;
                };

                var despawn = networkService.Session.GetRpc<Torikime.Example.Despawn.Rpc>();
                despawn.OnNotification += (notification) =>
                {
                    var trail = trails[notification.SessionId];
                    if (trail == null)
                    {
                        return;
                    }

                    trails.Remove(notification.SessionId);
                    Destroy(trail);
                };
            }
        }

        private void Update()
        {
            var screenPosision = Input.mousePosition;
            screenPosision.z = 1;
            myTrail.transform.position = Camera.current.ScreenToWorldPoint(screenPosision);

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
                request.Position = myTrail.transform.position.ToVector3();
                StartCoroutine(rpc.RequestCoroutine(request, (response) => { }));
            }
        }

        private void OnDestroy()
        {
            networkService.Session.Disconnect();
        }
    }
}
