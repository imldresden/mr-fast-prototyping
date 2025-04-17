using Microsoft.MixedReality.QR;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QRCodeHandling
{
    public delegate void QrCodeEventHandler(QRCode qrCode);

    public class QRCodesManager : Singleton<QRCodesManager>
    {
        private struct QrEvent
        {
            public QRCode QRCode;
            public string EventName;

            public QrEvent(QRCode qrCode, string eventName)
            {
                QRCode = qrCode;
                EventName = eventName;
            }
        }

        [Tooltip("Determines if the QR codes scanner should be automatically started.")]
        public bool AutoStartQRTracking = true;

        public bool IsTrackerRunning { get; private set; }

        public bool IsSupported { get; private set; }

        private Queue<QrEvent> pendingActions = new Queue<QrEvent>();
        public event EventHandler<bool> QRCodesTrackingStateChanged;
        public event QrCodeEventHandler QRCodeAdded;
        public event QrCodeEventHandler QRCodeUpdated;
        public event QrCodeEventHandler QRCodeRemoved;

        private SortedDictionary<Guid, QRCode> qrCodesList = new SortedDictionary<Guid, QRCode>();

        private QRCodeWatcher qrTracker;
        private bool capabilityInitialized = false;
        private QRCodeWatcherAccessStatus accessStatus;
        private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> capabilityTask;

        #region Getter
        public Guid GetIdForQRCode(string qrCodeData)
        {
            lock (qrCodesList)
            {
                foreach (var ite in qrCodesList)
                    if (ite.Value.Data == qrCodeData)
                        return ite.Key;
            }
            return new Guid();
        }

        public IList<QRCode> GetList()
        {
            lock (qrCodesList)
            {
                return new List<QRCode>(qrCodesList.Values);
            }
        }
        #endregion

        #region Unity Methods
        // Use this for initialization
        async protected virtual void Start()
        {
            IsSupported = QRCodeWatcher.IsSupported();
            capabilityTask = QRCodeWatcher.RequestAccessAsync();
            accessStatus = await capabilityTask;
            capabilityInitialized = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (qrTracker == null && capabilityInitialized && IsSupported)
            {
                if (accessStatus == QRCodeWatcherAccessStatus.Allowed)
                    SetupQRTracking();
                else
                    Debug.Log("Capability access status : " + accessStatus);
            }

            lock (pendingActions)
            {
                while (pendingActions.Count > 0)
                {
                    try
                    {
                        QrEvent qrEvent = pendingActions.Dequeue();

                        switch (qrEvent.EventName)
                        {
                            case "added":   QRCodeAdded?.Invoke(qrEvent.QRCode);                break;
                            case "updated": QRCodeUpdated?.Invoke(qrEvent.QRCode);              break;
                            case "removed": QRCodeRemoved?.Invoke(qrEvent.QRCode);              break;
                            case "stopped": QRCodesTrackingStateChanged?.Invoke(this, false);   break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning(ex.Message);
                    }
                }
            }
        }
        #endregion

        #region QrCodeManager Lifecycle Management
        private void SetupQRTracking()
        {
            try
            {
                qrTracker = new QRCodeWatcher();
                IsTrackerRunning = false;
                qrTracker.Added += QRCodeWatcher_Added;
                qrTracker.Updated += QRCodeWatcher_Updated;
                qrTracker.Removed += QRCodeWatcher_Removed;
                qrTracker.EnumerationCompleted += QRCodeWatcher_EnumerationCompleted;
            }
            catch (Exception ex)
            {
                Debug.Log("QRCodesManager : exception starting the tracker " + ex.ToString());
            }

            if (AutoStartQRTracking)
                StartQRTracking();
        }

        public void StartQRTracking()
        {
            if (IsTrackerRunning)
                return;
            if (qrTracker == null)
                return;

            Debug.Log("QRCodesManager starting QRCodeWatcher");
            try
            {
                qrTracker.Start();
                IsTrackerRunning = true;
                QRCodesTrackingStateChanged?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                Debug.Log("QRCodesManager starting QRCodeWatcher Exception:" + ex.ToString());
            }
        }

        public void StopQRTracking()
        {
            if (!IsTrackerRunning)
                return;

            IsTrackerRunning = false;
            if (qrTracker != null)
            {
                qrTracker.Stop();
                qrCodesList.Clear();
            }

            lock (pendingActions)
            {
                pendingActions.Enqueue(new QrEvent(null, "stopped"));
            }
        }
        #endregion

        #region QrCodeWatcher Events
        private void QRCodeWatcher_Added(object sender, QRCodeAddedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Added");

            lock (qrCodesList)
            {
                qrCodesList[args.Code.Id] = args.Code;
            }

            lock (pendingActions)
            {
                pendingActions.Enqueue(new QrEvent(args.Code, "added"));
            }
        }

        private void QRCodeWatcher_Updated(object sender, QRCodeUpdatedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Updated");

            bool found = false;
            lock (qrCodesList)
            {
                if (qrCodesList.ContainsKey(args.Code.Id))
                {
                    found = true;
                    qrCodesList[args.Code.Id] = args.Code;
                }
            }

            if (!found)
                return;

            lock (pendingActions)
            {
                pendingActions.Enqueue(new QrEvent(args.Code, "updated"));
            }
        }

        private void QRCodeWatcher_Removed(object sender, QRCodeRemovedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Removed");

            bool found = false;
            lock (qrCodesList)
            {
                if (qrCodesList.ContainsKey(args.Code.Id))
                {
                    qrCodesList.Remove(args.Code.Id);
                    found = true;
                }
            }

            if (!found)
                return;

            lock (pendingActions)
            {
                pendingActions.Enqueue(new QrEvent(args.Code, "removed"));
            }
        }

        private void QRCodeWatcher_EnumerationCompleted(object sender, object e)
        {
            Debug.Log("QRCodesManager QrTracker_EnumerationCompleted");
        } 
        #endregion
    }

}