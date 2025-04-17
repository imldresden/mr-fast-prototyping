
using Microsoft.MixedReality.QR;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QRCodeHandling
{
    public class QRCodesVisualizer : MonoBehaviour
    {
        public GameObject qrCodePrefab;

        private SortedDictionary<Guid, GameObject> qrCodesObjectsList;
        private Queue<ActionData> pendingActions = new Queue<ActionData>();
        private bool clearExisting = false;

        struct ActionData
        {
            public enum Type
            {
                Added,
                Updated,
                Removed
            };
            public Type type;
            public Microsoft.MixedReality.QR.QRCode qrCode;

            public ActionData(Type type, Microsoft.MixedReality.QR.QRCode qRCode) : this()
            {
                this.type = type;
                qrCode = qRCode;
            }
        }

        #region Unity Mehtods
        // Use this for initialization
        void Start()
        {
            Debug.Log("QRCodesVisualizer start");
            qrCodesObjectsList = new SortedDictionary<Guid, GameObject>();

            QRCodesManager.Instance.QRCodesTrackingStateChanged += _qrCodesManager_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += _qrCodesManager_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += _qrCodesManager_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += _qrCodesManager_QRCodeRemoved;
            if (qrCodePrefab == null)
            {
                throw new System.Exception("Prefab not assigned");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (clearExisting)
            {
                clearExisting = false;
                foreach (var obj in qrCodesObjectsList)
                {
                    Destroy(obj.Value);
                }
                qrCodesObjectsList.Clear();
            }
        }
        #endregion

        #region QrCodeManager Events
        private void _qrCodesManager_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                clearExisting = true;
            }
        }

        private void _qrCodesManager_QRCodeAdded(QRCode qrCode)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeAdded");

            GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            qrCodeObject.GetComponent<QRCodeContent>().qrCode = qrCode;
            qrCodesObjectsList.Add(qrCode.Id, qrCodeObject);
        }

        private void _qrCodesManager_QRCodeUpdated(QRCode qrCode)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");

            if (qrCodesObjectsList.ContainsKey(qrCode.Id))
                return;

            GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            qrCodeObject.GetComponent<QRCodeContent>().qrCode = qrCode;
            qrCodesObjectsList.Add(qrCode.Id, qrCodeObject);

        }

        private void _qrCodesManager_QRCodeRemoved(QRCode qrCode)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeRemoved");

            if (!qrCodesObjectsList.ContainsKey(qrCode.Id))
                return;

            Destroy(qrCodesObjectsList[qrCode.Id]);
            qrCodesObjectsList.Remove(qrCode.Id);
        } 
        #endregion
    }

}