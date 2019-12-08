using Project.Utility.Attributes;
using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Networking
{
    public class NetworkIdentity : MonoBehaviour
    {
        [Header("Helpfull values")]
        [SerializeField]
        [GreyOut]
        private string id;
        [SerializeField]
        [GreyOut]
        private bool isControlling;

        private SocketIOComponent socket;

        // Start is called before the first frame update
        void Awake()
        {
            isControlling = false;
        }

        public void SetControllerID(string ID)
        {
            id = ID;
            isControlling = (NetworkClient.ClientID == ID) ? true : false;//chequea id entrante vs la que hemos guardado del server
        }

        public void SetSocketReference(SocketIOComponent Socket)
        {
            socket = Socket;
        }

        public string GetId()
        {
            return id;
        }

        public bool IsControlling()
        {
            return isControlling;
        }

        public SocketIOComponent GetSocked()
        {
            return socket;
        }
    }
}


