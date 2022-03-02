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
                    //Debug.Log("Notification received: " + notification.From + "[" + notification.Message + "]");
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
                    var unit = new PlayerUnit(networkService, new UnitId(notification.UnitId), notification.Position.ToVector3(), notification.Direction, notification.Avatar);
                    unitService.Register(unit);
                };

                var unitDespawn = networkService.Session.GetRpc<Torikime.Unit.Despawn.Rpc>();
                unitDespawn.OnNotification += (notification) =>
                {
                    unitService.UnregisterByUnitId(new UnitId(notification.UnitId));
                };

                var unitMove = networkService.Session.GetRpc<Torikime.Unit.Move.Rpc>();
                unitMove.OnNotification += unitService.OnReceiveMove;
            }

            networkService.StartReceive();
            StartCoroutine(DoPingPong());

            {
                var rpc = networkService.Session.GetRpc<Torikime.Unit.SpawnReady.Rpc>();
                yield return rpc.RequestCoroutine(new Torikime.Unit.SpawnReady.Request { AreaId = 0 }, (response) => {
                    var unit = new ControllablePlayerUnit(networkService, new UnitId(response.UnitId), response.Position.ToVector3(), response.Direction, response.Avatar);
                    unitService.Register(unit);
                });
            }
        }

        public long __now;
        public long __serverTime;
        public long __diffTime;
        public long __sendGap;
        public long __receiveGap;
        public long __latency;
        public long __gap;
        public long __subjectiveLatency;

        private IEnumerator DoPingPong()
        {
            while (true)
            {
                var pingpong = networkService.Session.GetRpc<Torikime.Diagnosis.PingPong.Rpc>();
                var request = new Torikime.Diagnosis.PingPong.Request();
                DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                request.SendTime = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
                //Debug.Log("ping sent");
                yield return pingpong.RequestCoroutine(request, (response) =>
                {
                    __now = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
                    __sendGap = response.ReceiveTime - request.SendTime;
                    __receiveGap = __now - response.ReceiveTime;
                    __latency = __now - request.SendTime;
                    __gap = (__sendGap + __receiveGap + __latency) / 2;
                    __serverTime = response.SendTime + __gap + __latency;
                    __diffTime = __serverTime - __now - __latency;
                    var str = $"client send: {request.SendTime}\n" +
                        $"server received: {response.ReceiveTime}\n" +
                        $"server send: {response.SendTime}\n" +
                        $"client received: {__now}\n" +
                        $"latency: {__latency}\n" +
                        $"send gap: {response.ReceiveTime - request.SendTime}\n" +
                        $"receive gap: {__now - response.SendTime}\n" +
                        $"gap: {__gap}\n" +
                        $"client time: {__now}\n" +
                        $"server time: {response.SendTime}\n" +
                        $"adj time: {__serverTime}\n" +
                        $"diff time: {__diffTime}\n";

                    callOnMainThread.Enqueue(() => {
                        //Debug.Log("pong received");
                        __subjectiveLatency = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds - request.SendTime;
                        pingText.text = str + $"subjective latency: {__subjectiveLatency}\n";
                        networkService.ServerTimeDifference = __diffTime;
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
