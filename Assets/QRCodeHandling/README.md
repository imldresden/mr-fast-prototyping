# QR Code Handling

Own implementation on a QR Code Tracking for the HoloLens 2.
This is based on the following two implementations:
* https://github.com/chgatla-microsoft/QRTracking
* https://github.com/yl-msft/QRTracking

## Prerequesits

This code needs the following additional packages or other prerequisits:
* Unity 2020 LTS
* MRTK (>= 2.73) with OpenXR (>= 1.3.1)
* NuGet for Unity (>= 3.0.2)
* MixedReality.QR NuGet Package (>= 0.5.3013)

## Example Scenes

For both scenes there also exists a prefab corresponding to it, to be used in other projects.

### Sample QR Code Handling Scene - QR Code Visualizer
* Shows how the QR code can be used to extract data and show different objects aligned to it

### Sample QR Code Handling Scene - QR Coordinate System Handling
* Shows how the QR Code can be used to define a coordinate system
* This system can be altered by different Offset
* It is also possible to communicate over the __MasterNetworking__ code