using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace QRCodeHandling
{
    public static class QRCodeExtension
    {
        private static Guid _id;
        private static SpatialGraphNode _node;

        private static SpatialGraphNode _getNode(QRCode qrCode)
        {
            if (_id != qrCode.SpatialGraphNodeId)
            {
                if (qrCode.SpatialGraphNodeId == System.Guid.Empty)
                    return null;

                _node = SpatialGraphNode.FromStaticNodeId(qrCode.SpatialGraphNodeId);
            }

            return _node;
        }

        public static Pose? GetPose(this QRCode qrCode)
        {
            SpatialGraphNode node = _getNode(qrCode);

            if (node == null)
                return null;

            if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                // to these objects so apply the inverse
                if (CameraCache.Main.transform.parent != null)
                    pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);

                Debug.Log("Id= " + qrCode.SpatialGraphNodeId + " QRPose = " + pose.position.ToString("F7") + " QRRot = " + pose.rotation.ToString("F7"));

                return pose;
            }
            else
                Debug.LogWarning("Cannot locate " + qrCode.SpatialGraphNodeId);

            return null;
        }
    }
}
