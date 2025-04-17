//using MasterNetworking.EventHandling;
using Microsoft.MixedReality.QR;
using System;
using UnityEngine;

namespace QRCodeHandling
{
    public class QRCoordinateSystemHandler : MonoBehaviour
    {
        public event EventHandler LockChanged;

        public GameObject ContentRootGO;
        public GameObject QRCodeGO;
        public string MarkingString;
        public bool ShowQRCodeGO = true;
        public bool ContentOnlyActiveWhenQRTracked = false;

        public Vector3 TranslationOffset
        {
            get => _translationOffset;
            set
            {
                _translationOffset = value;
                ContentRootGO.transform.localPosition = new Vector3(
                    x: _translationOffset.x,
                    y: _translationOffset.z,
                    z: _translationOffset.y
                );
            }
        }
        [SerializeField]
        private Vector3 _translationOffset = Vector3.zero;

        public Vector3 RotationOffset
        {
            get => _rotationOffset;
            set
            {
                _rotationOffset = value;
                ContentRootGO.transform.localRotation = Quaternion.Euler(0, 0, 0);
                ContentRootGO.transform.Rotate(_rotationOffset);
            }
        }
        [SerializeField]
        private Vector3 _rotationOffset = Vector3.zero;

        [SerializeField]
        private bool _locked = false;
        [SerializeField]
        private bool allowLocking = false;

        public bool Locked
        {
            get => _locked;
            set
            {
                bool _oldLocked = _locked;
                _locked = value;

                QRCodeGO.SetActive(!_locked);

                if (_locked != _oldLocked)
                    LockChanged?.Invoke(this, null);
            }
        }

        #region Unity Methods
        // Use this for initialization
        private void Start()
        {
            Debug.Log("QRCodesVisualizer start");

            QRCodesManager.Instance.QRCodesTrackingStateChanged += _qrCodesManager_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += _qrCodesManager_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += _qrCodesManager_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += _qrCodesManager_QRCodeRemoved;

            if (ContentOnlyActiveWhenQRTracked)
            {
                ContentRootGO.SetActive(false);
                QRCodeGO.SetActive(false);
            }
            if (!ShowQRCodeGO)
                QRCodeGO.SetActive(false);


            //JsonRpcHandler.Instance.AddNotificationDelegate("LockCoordinateSystem", _onNotification_LockCoordinateSystem);
            //JsonRpcHandler.Instance.AddNotificationDelegate("SetCoordinateSystem", _onNotification_SetCoordinateSystem);

            TranslationOffset = _translationOffset;
            RotationOffset = _rotationOffset;
        }
        #endregion

        #region QrCodeManager Events
        private void _qrCodesManager_QRCodesTrackingStateChanged(object sender, bool e)
        {
        }

        private void _qrCodesManager_QRCodeAdded(QRCode qrCode)
        {
            Pose? pose = qrCode.GetPose();
            if (pose == null)
                return;

            if (qrCode.Data != MarkingString) 
                return;
            if (ContentOnlyActiveWhenQRTracked)
            {
                ContentRootGO.SetActive(true);
                if (ShowQRCodeGO)
                    QRCodeGO.SetActive(true);
            }
            _updateQRCodeGO(qrCode);

            if (Locked)
                return;



            transform.SetPositionAndRotation(
                position: pose.Value.position,
                rotation: pose.Value.rotation
            );
            //Debug.Log($"Pos: ({pose.Value.position.x}, {pose.Value.position.y}, {pose.Value.position.z}) | Rot: ({pose.Value.rotation.eulerAngles.x}, {pose.Value.rotation.eulerAngles.y}, {pose.Value.rotation.eulerAngles.z})");
        }

        private void _qrCodesManager_QRCodeUpdated(QRCode qrCode)
        {
            Pose? pose = qrCode.GetPose();
            if (pose == null)
                return;

            if (qrCode.Data != MarkingString)
                return;

            if (ContentOnlyActiveWhenQRTracked)
            {
                ContentRootGO.SetActive(true);
                if (ShowQRCodeGO)
                    QRCodeGO.SetActive(true);
            }

            _updateQRCodeGO(qrCode);

            if (Locked)
                return;

            

            transform.SetPositionAndRotation(
                position: pose.Value.position,
                rotation: pose.Value.rotation
            );
            Debug.Log($"Pos: ({pose.Value.position.x}, {pose.Value.position.y}, {pose.Value.position.z}) | Rot: ({pose.Value.rotation.eulerAngles.x}, {pose.Value.rotation.eulerAngles.y}, {pose.Value.rotation.eulerAngles.z})");
        }

        private void _qrCodesManager_QRCodeRemoved(QRCode qrCode)
        {
            if (ContentOnlyActiveWhenQRTracked && (qrCode.Data == MarkingString || MarkingString == "" || MarkingString == null))
            {
                ContentRootGO.SetActive(false);
                QRCodeGO.SetActive(false);
            }
        }
        #endregion

        private void _updateQRCodeGO(QRCode qrCode)
        {
            float physicalSize = qrCode.PhysicalSideLength;
            QRCodeGO.transform.localPosition = new Vector3(physicalSize / 2, physicalSize / 2, 0.0f);
            QRCodeGO.transform.localScale = new Vector3(physicalSize, physicalSize, physicalSize);
        }


        //private void _onNotification_LockCoordinateSystem(JsonRpcMessage message)
        //{
        //    if(allowLocking)
        //        Locked = !Locked;
        //}

        //private void _onNotification_SetCoordinateSystem(JsonRpcMessage message)
        //{
        //    TranslationOffset = new Vector3(
        //        x: message.Data["translationX"].ToObject<float>(),
        //        y: message.Data["translationY"].ToObject<float>(),
        //        z: message.Data["translationZ"].ToObject<float>()
        //    );
        //    RotationOffset = new Vector3(
        //        x: message.Data["rotationX"].ToObject<float>(),
        //        y: message.Data["rotationY"].ToObject<float>(),
        //        z: message.Data["rotationZ"].ToObject<float>()
        //    );
        //}


        #region Editor Methods
        [ContextMenu("Update Offsets")]
        private void _editor_updateOffsets()
        {
            TranslationOffset = _translationOffset;
            RotationOffset = _rotationOffset;
        }
        #endregion
    }

}
