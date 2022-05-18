using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Potato
{
    public class Example : MonoBehaviour
    {
        private Potato.Network.NetworkService networkService;
        private UnitService unitService;

        public GameObject trailPrefab;

        public Text sessionCountText;
        public Text pingText;

        public GameObject panel;
        public InputField hostInputField;
        public InputField nameInputField;
        public Button connectButton;
        public GameObject nowLoading;

        public Camera mainCamera;

        public string serverHost = "127.0.0.1";
        public string serverPort = "28888";

        public Action<Google.Protobuf.IMessage> OnReceiveMessage { get; set; }
        public Action OnRpcReady { get; set; }

        private readonly Queue<Diagnosis.Gizmo.Notification> gizmoQueue = new();

        private readonly ConcurrentQueue<Action> queue = new();

        private GameObject nodes;

        private IEnumerator Start()
        {
            nowLoading.SetActive(true);

            nodes = new GameObject("Area Nodes");

            var task = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Assets/Examples/Map1/Map1.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            unitService = FindObjectOfType<UnitService>();

            yield return new WaitUntil(() => task.isDone);

            nowLoading.SetActive(false);

            yield return LoginSequence();
        }

        private IEnumerator LoginSequence()
        {
            connectButton.enabled = false;
            nameInputField.Select();

            var web = UnityWebRequest.Get("https://asia-northeast1-potato-343314.cloudfunctions.net/function-1");
            yield return web.SendWebRequest();
            Debug.Log(web.result);
            Debug.Log(web.downloadHandler.text);
            if (web.downloadHandler.text.Length > 0)
            {
                hostInputField.text = web.downloadHandler.text;
            }

            connectButton.enabled = true;

            void SubmitLogin()
            {
                hostInputField.gameObject.SetActive(false);
                nameInputField.gameObject.SetActive(false);
                connectButton.gameObject.SetActive(false);
                serverHost = hostInputField.text;
            }

            connectButton.onClick.AddListener(SubmitLogin);
            nameInputField.onSubmit.AddListener((_) => { SubmitLogin(); });

            yield return new WaitWhile(() => connectButton.gameObject.activeSelf);

            networkService = FindObjectOfType<Potato.Network.NetworkService>();
            var session = networkService.Connect(serverHost, int.Parse(serverPort));
            Potato.RpcHolder.Rpcs = Potato.RpcBuilder.Build(session);
            session.OnPayloadReceived = (payload) =>
            {
                var rpc = Potato.RpcHolder.GetRpc(payload);

                if (rpc == null)
                {
                    Debug.LogError($"payload.Header.contract_id: {payload.Header.contract_id} not found {payload.Header}");
                }

                if (rpc is Diagnosis.PingPong.Rpc)
                {
                    try
                    {
                        // call immediately(in network thread)
                        rpc.ReceievePayload(payload);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                if (rpc != null)
                {
                    queue.Enqueue(() => {
                        var message = rpc.ReceievePayload(payload);
#if UNITY_EDITOR
                        OnReceiveMessage?.Invoke(message);
#endif
                    });
                }
            };
            OnRpcReady?.Invoke();

            networkService.OnDisconnectedCallback += OnDisconnected;

            {
                var sendMessage = Potato.RpcHolder.GetRpc<Potato.Chat.SendMessage.Rpc>();
                sendMessage.OnNotification += (notification) =>
                {
                    //Debug.Log("Notification received: " + notification.From + "[" + notification.Message + "]");
                };

                //Debug.Log("Request sent");
                //var request = new Chat.SendMessage.Request
                //{
                //    Message = "Hello world"
                //};
                //sendMessage.Request(request, (response) =>
                //{
                //    Debug.Log("Response");
                //});
            }

            {
                var unitSpawn = Potato.RpcHolder.GetRpc<Potato.Unit.Spawn.Rpc>();
                unitSpawn.OnNotification += unitService.OnReceiveSpawn;

                var unitDespawn = Potato.RpcHolder.GetRpc<Potato.Unit.Despawn.Rpc>();
                unitDespawn.OnNotification += unitService.OnReceiveDespawn;

                var unitMove = Potato.RpcHolder.GetRpc<Potato.Unit.Move.Rpc>();
                unitMove.OnNotification += unitService.OnReceiveMove;

                var unitStop = Potato.RpcHolder.GetRpc<Potato.Unit.Stop.Rpc>();
                unitStop.OnNotification += unitService.OnReceiveStop;

                var unitKnockback = Potato.RpcHolder.GetRpc<Potato.Unit.Knockback.Rpc>();
                unitKnockback.OnNotification += unitService.OnReceiveKnockback;
            }

            {
                var gizmo = Potato.RpcHolder.GetRpc<Potato.Diagnosis.Gizmo.Rpc>();
                gizmo.OnNotification += (notification) => { gizmoQueue.Enqueue(notification); };
            }

            {
                var battleSkillCast = Potato.RpcHolder.GetRpc<Potato.Battle.SkillCast.Rpc>();
                battleSkillCast.OnNotification += unitService.OnReceiveSkillCast;
            }

            {
                var syncCharacterStatus = Potato.RpcHolder.GetRpc<Potato.Battle.SyncParameters.Rpc>();
                syncCharacterStatus.OnNotification += unitService.OnReceiveCharacterStatus;
            }

            bool serverIsReady = false;
            {
                RpcHolder.GetRpc<Auth.ServerReady.Rpc>().OnNotification += (notification) => {
                    serverIsReady = true;
                };
            }

            bool transportOrder = false;
            uint transportToAreaId = 0;
            {
                var transport = Potato.RpcHolder.GetRpc<Potato.Area.Transport.Rpc>();
                transport.OnNotification += (notification) => {
                    Debug.Log(notification.ToString());
                    transportToAreaId = notification.AreaId;
                    transportOrder = true;

                    StartCoroutine(TransportSequence(notification));
                };
            }

            networkService.StartReceive();
            yield return new WaitUntil(() => serverIsReady);

            StartCoroutine(DoPingPong());

            yield return new WaitUntil(() => timeSynchronized);
            panel.SetActive(false);

            yield return RequestLogin(nameInputField.text, string.Empty);

            yield return new WaitUntil(() => transportOrder);
        }

        private IEnumerator TransportSequence(Potato.Area.Transport.Notification notification)
        {
            nowLoading.SetActive(true);

            var controllableUnit = unitService.GetControllableUnit();
            if (controllableUnit != null)
            {
                controllableUnit.ControlLock = true;
            }

            yield return new WaitForSeconds(1.0f);

            // destroy all units
            unitService.Reset();

            Debug.Log($"Area transport notification received: transport id:{notification.TransportId} area:{notification.AreaId} unit:{notification.UnitId}");
            var transport = Potato.RpcHolder.GetRpc<Potato.Area.Transport.Rpc>();
            yield return transport.RequestCoroutine(new Potato.Area.Transport.Request(new Potato.Area.Transport.Request() { TransportId = notification.TransportId }), (response) =>
            {
                Debug.Log("Transport response");
            });

            yield return RequestAreaConstituteData(notification.AreaId);

            yield return RequestSpawnReady(notification.AreaId);

            yield return new WaitForSeconds(1.0f);

            controllableUnit = unitService.GetControllableUnit();
            if (controllableUnit != null)
            {
                controllableUnit.ControlLock = false;
            }

            nowLoading.SetActive(false);
        }

        private IEnumerator RequestLogin(string userId, string password)
        {
            return Potato.RpcHolder.GetRpc<Potato.Auth.Login.Rpc>().RequestCoroutine(
                new Potato.Auth.Login.Request { UserId = userId, Password = password },
                (response) =>
                {
                    Debug.Log($"Login response {response.Ok}, {response.Token}");
                });
        }

        private object RequestAreaConstituteData(uint transportToAreaId)
        {
            var rpc = Potato.RpcHolder.GetRpc<Potato.Area.ConstitutedData.Rpc>();
            return rpc.RequestCoroutine(new Potato.Area.ConstitutedData.Request { AreaId = transportToAreaId }, (response) =>
            {
                foreach (var t in nodes.transform)
                {
                    Destroy((t as Transform).gameObject);
                }
                foreach (var trigger in response.Triggers)
                {
                    Debug.Log($"trigger transport to area[{trigger.AreaId}]: {trigger.Position.ToVector3()}");
                    var t = new GameObject("Area Transport Trigger");
                    t.transform.parent = nodes.transform;
                    t.transform.SetPositionAndRotation(trigger.Position.ToVector3(), Quaternion.identity);
                    var c = t.AddComponent<BoxCollider>();
                    c.center = trigger.Offset.ToVector3();
                    c.size = trigger.Size.ToVector3();
                }
            });
        }

        private IEnumerator RequestSpawnReady(uint transportToAreaId)
        {
            var rpc = Potato.RpcHolder.GetRpc<Potato.Unit.SpawnReady.Rpc>();
            return rpc.RequestCoroutine(new Potato.Unit.SpawnReady.Request { AreaId = transportToAreaId }, (response) =>
            {
                var unit = new ControllablePlayerUnit(networkService, new UnitId(response.UnitId), response.Position.ToVector3(), response.Direction, response.Avatar);
                unitService.Register(unit);

                foreach (var neighbor in response.Neighbors)
                {
                    unitService.OnReceiveSpawn(neighbor.Spawn);

                    if (neighbor.Move != null)
                    {
                        unitService.OnReceiveMove(neighbor.Move);
                    }

                    if (neighbor.Stop != null)
                    {
                        unitService.OnReceiveStop(neighbor.Stop);
                    }
                }
            });
        }

        private void OnDisconnected(Network.Session _)
        {
            networkService.OnDisconnectedCallback -= OnDisconnected;

            Debug.LogWarning("Session disconnected.");
            if (!Application.isPlaying)
            {
                return;
            }

            Potato.RpcHolder.Clear();
            unitService.Reset();

            hostInputField?.gameObject.SetActive(true);
            nameInputField?.gameObject.SetActive(true);
            connectButton?.gameObject.SetActive(true);
            panel?.SetActive(true);

            StartCoroutine(LoginSequence());
        }

        private bool timeSynchronized = false;
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
            while (networkService != null && networkService.Session != null)
            {
                var pingpong = RpcHolder.GetRpc<Diagnosis.PingPong.Rpc>();
                var request = new Diagnosis.PingPong.Request
                {
                    SendTime = (long)(DateTime.UtcNow - TimeHelpers.UnixEpoch).TotalMilliseconds
                };
                //Debug.Log("ping sent");
                var context = SynchronizationContext.Current;
                yield return pingpong.RequestCoroutine(request, (response) =>
                {
                    __now = (long)(DateTime.UtcNow - TimeHelpers.UnixEpoch).TotalMilliseconds;
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

                    context.Post(_ => {
                        //Debug.Log("pong received");
                        __subjectiveLatency = (long)(DateTime.UtcNow - TimeHelpers.UnixEpoch).TotalMilliseconds - request.SendTime;
                        pingText.text = str + $"subjective latency: {__subjectiveLatency}\n";
                        networkService.ServerTimeDifference = __diffTime;
                        timeSynchronized = true;
                    }, null);
                });

                yield return new WaitForSeconds(5);
            }
        }

        private void Update()
        {
            while (queue.TryDequeue(out var action))
            {
                action();
            }
        }

        private void LateUpdate()
        {
            if (networkService == null || networkService.Session == null)
            {
                return;
            }

            if (Time.frameCount % 600 == 0)
            {
                var request = new Potato.Diagnosis.SeverSessions.Request();
                var rpc = Potato.RpcHolder.GetRpc<Potato.Diagnosis.SeverSessions.Rpc>();
                rpc.Request(request, (response) =>
                    {
                        sessionCountText.text = "Current session count is " + response.SessionCount;
                    });
            }
        }

        private void OnDrawGizmos()
        {
            while (gizmoQueue.Count > 0)
            {
                var g = gizmoQueue.Dequeue();
                Gizmos.color = Color.green;
                Gizmos.DrawLine(g.Begin.ToVector3(), g.End.ToVector3());
            }
        }
    }
}
