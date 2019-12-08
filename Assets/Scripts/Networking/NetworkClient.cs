using System.Collections;
using System.Collections.Generic;
using Project.Utility;
using UnityEngine;
using SocketIO;
using System;
using Project.Player;

namespace Project.Networking
{
    public class NetworkClient : SocketIOComponent
    {
        [Header("Network Clinet")]
        [SerializeField]
        private Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefab;

        public static string ClientID { get; private set; }

        private Dictionary<string, NetworkIdentity> serverObjects;

        public override void Start()
        {
            base.Start();
            Initialize();
            SetupEvents();
        }

        private void Initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
        }

        public override void Update()
        {
            base.Update();
        }

        private void SetupEvents()
        {
            On("open", (E) =>
            {
                Debug.Log("Connection mate to the server");
            });

            On("register", (E) =>
            {
                ClientID = E.data["id"].ToString().RemoveQuotes();

                Debug.LogFormat("Our Client's ID ({0})", ClientID);
            });

            On("spawn", (E) =>
            {
                string id = E.data["id"].ToString().Replace("\"", string.Empty).Trim();

                GameObject go = Instantiate(playerPrefab, networkContainer);
                go.name = string.Format("Payer({0})",id);
                NetworkIdentity networkIdentity = go.GetComponent<NetworkIdentity>();
                networkIdentity.SetControllerID(id);
                networkIdentity.SetSocketReference(this);
                serverObjects.Add(id, networkIdentity);
            });

            On("disconected", (E) =>
            {
                string id = E.data["id"].ToString();

                GameObject go = serverObjects[id].gameObject;
                Destroy(go);
                serverObjects.Remove(id);
            });

            On("updatePosition", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();
                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                //actualizo posicion del juador correspondiente
                NetworkIdentity networkIdentity = serverObjects[id];
                networkIdentity.transform.position = new Vector3(x,y,0);
            });

            On("updateRotation", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();
                float tankRotation = E.data["tankRotation"].f;
                float barrelRotation = E.data["barrelRotation"].f;

                //actualizo la rotacion del jugador correspondiente
                NetworkIdentity networkIdentity = serverObjects[id];
                networkIdentity.transform.localEulerAngles = new Vector3(0, 0, tankRotation);
                networkIdentity.GetComponent<PlayerManager>().SetBarrelRotation(barrelRotation);

            });
        }

    }
    //serializables para ser enviados via JSON
    [Serializable]
    public class Player
    {
        public string id;
        public Position position;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
    }

    [Serializable]
    public class PlayerRortation
    {
        public float tankRotation;
        public float barrelRotation;
    }
}



