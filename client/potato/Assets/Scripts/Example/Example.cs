using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Potato
{
    public class Example : MonoBehaviour
    {
        private Potato.Network.NetworkService networkService;
        private UnitService unitService;

        public GameObject trailPrefab;

        private GameObject myTrail;

        public Text sessionCountText;
        public Text pingText;

        public Camera mainCamera;

        private Dictionary<int, GameObject> trails = new Dictionary<int, GameObject>();

        private ConcurrentQueue<Action> callOnMainThread = new ConcurrentQueue<Action>();

        private IEnumerator Start()
        {
            myTrail = Instantiate(trailPrefab);

            unitService = FindObjectOfType<UnitService>();

            networkService = FindObjectOfType<Potato.Network.NetworkService>();
            var session = networkService.Connect("127.0.0.1", 28888);

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
                    if (!trails.ContainsKey(notification.SessionId))
                    {
                        return;
                    }

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
                    Debug.Log($"Spawn {notification.SessionId}");
                    trails.Add(notification.SessionId, Instantiate(trailPrefab));
                    trails[notification.SessionId].transform.position = notification.Position.ToVector3();
                    trails[notification.SessionId].name = "Trail " + notification.SessionId;

                    // if (notification.SessionId == networkService.Session.SessionId)
                    // {
                    //     Destroy(myTrail.gameObject);
                    //     myTrail = trails[notification.SessionId];
                    // }
                };

                var despawn = networkService.Session.GetRpc<Torikime.Example.Despawn.Rpc>();
                despawn.OnNotification += (notification) =>
                {
                    if (!trails.ContainsKey(notification.SessionId))
                    {
                        return;
                    }

                    var trail = trails[notification.SessionId];
                    if (trail == null)
                    {
                        return;
                    }

                    trails.Remove(notification.SessionId);
                    Destroy(trail);
                };

                var unitSpawn = networkService.Session.GetRpc<Torikime.Unit.Spawn.Rpc>();
                unitSpawn.OnNotification += (notification) =>
                {
                    var unit = new PlayerUnit(new UnitId(0), notification.Position.ToVector3(), notification.Direction);
                    unitService.Register(unit);
                };

                var unitDespawn = networkService.Session.GetRpc<Torikime.Unit.Despawn.Rpc>();
                unitDespawn.OnNotification += (notification) =>
                {
                    unitService.UnregisterByUnitId(new UnitId(notification.UnitId));
                };
            }

            networkService.StartReceive();
            StartCoroutine(DoPingPong());

            {
                var rpc = networkService.Session.GetRpc<Torikime.Unit.SpawnReady.Rpc>();
                yield return rpc.RequestCoroutine(new Torikime.Unit.SpawnReady.Request { AreaId = 0 }, (response) => { });
            }
        }

        private IEnumerator DoPingPong()
        {
            while (true)
            {
                var pingpong = networkService.Session.GetRpc<Torikime.Diagnosis.PingPong.Rpc>();
                var request = new Torikime.Diagnosis.PingPong.Request();
                DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                request.SendTime = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
                Debug.Log("ping sent");
                yield return pingpong.RequestCoroutine(request, (response) =>
                {
                    var now = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
                    var sendGap = response.ReceiveTime - request.SendTime;
                    var receiveGap = now - response.ReceiveTime;
                    var latency = now - request.SendTime;
                    var gap = (sendGap + receiveGap + latency) / 2;
                    var serverTime = response.SendTime + gap + latency;
                    var str = $"client send: {request.SendTime}\n" +
                        $"server received: {response.ReceiveTime}\n" +
                        $"server send: {response.SendTime}\n" +
                        $"client received: {now}\n" +
                        $"latency: {latency}\n" +
                        $"send gap: {response.ReceiveTime - request.SendTime}\n" +
                        $"receive gap: {now - response.SendTime}\n" +
                        $"gap: {gap}\n" +
                        $"client time: {now}\n" +
                        $"server time: {response.SendTime}\n" +
                        $"adj time: {serverTime}\n" +
                        $"diff time: {serverTime - now - latency}\n";

                    callOnMainThread.Enqueue(() => {
                        Debug.Log("pong received");
                        pingText.text = str + $"subjective latency: {(long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds - request.SendTime}\n";
                    });
                });

                while (callOnMainThread.TryDequeue(out var action))
                {
                    action();
                }

                yield return new WaitForSeconds(1);
            }
        }

        private void LateUpdate()
        {
            var screenPosision = Input.mousePosition;
            screenPosision.z = 1;
            myTrail.transform.position = mainCamera.ScreenToWorldPoint(screenPosision);

            // {
            //     var rpc = networkService.Session.GetRpc<Torikime.Chat.SendMessage.Rpc>();
            //     // Debug.Log("Request sent");
            //     var request = new Torikime.Chat.SendMessage.Request();
            //     request.Message = "Hello world2";
            //     StartCoroutine(rpc.RequestCoroutine(request, (response) =>
            //     {
            //         // Debug.Log("Torikime.Chat.SendMessage.Rpc: " + response.MessageId);
            //         // rpc.RequestAsync(new Torikime.Chat.SendMessage.Request()).ContinueWith((task) =>
            //         // {
            //         //     Debug.Log(task.Result);
            //         // }).Wait();
            //     }));
            // }

            {
                var request = new Torikime.Diagnosis.SeverSessions.Request();
                var rpc = networkService.Session.GetRpc<Torikime.Diagnosis.SeverSessions.Rpc>();
                rpc.Request(request, (response) =>
                    {
                        sessionCountText.text = "Current session count is " + response.SessionCount;
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
