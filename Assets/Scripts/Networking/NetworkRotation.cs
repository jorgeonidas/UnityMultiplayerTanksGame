using Project.Utility.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Player;
using Project.Utility;

namespace Project.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkRotation : MonoBehaviour
    {
        [Header("Reference values")]
        [SerializeField]
        [GreyOut]
        private float oldTankRotation;

        [Header("Reference values")]
        [SerializeField]
        [GreyOut]
        private float oldBarrelRotation;

        [Header("Class References")]
        [SerializeField]
        private PlayerManager playerManager;

        private NetworkIdentity networkIdentity;
        private PlayerRortation player;

        private float stillCounter = 0;

        // Start is called before the first frame update
        void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();

            player = new PlayerRortation();
            player.tankRotation = 0;
            player.barrelRotation = 0;

            if (!networkIdentity.IsControlling())
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (networkIdentity.IsControlling())
            {
                //si el tanke o el cañon han cambiado de rotacion  enviarla al server
                if(oldTankRotation != transform.localEulerAngles.z || oldBarrelRotation != playerManager.GetLastRotation())
                {
                    oldTankRotation = transform.localEulerAngles.z;
                    oldBarrelRotation = playerManager.GetLastRotation();
                    stillCounter = 0;
                    SendData();
                }
                else//actualizar cada segundo si está estatico
                {
                    stillCounter += Time.deltaTime;
                    if(stillCounter >= 1)
                    {
                        stillCounter = 0;
                        SendData();
                    }
                }
            }
        }

        private void SendData()
        {
            //Actualiza rotacion del player y torreta
            player.tankRotation = transform.localEulerAngles.z.TwoDecimals();
            player.barrelRotation = playerManager.GetLastRotation().TwoDecimals();

            networkIdentity.GetSocked().Emit("updateRotation", new JSONObject(JsonUtility.ToJson(player)));
        }
    }
}
