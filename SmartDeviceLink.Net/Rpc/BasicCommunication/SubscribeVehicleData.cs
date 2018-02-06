using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Rpc.Common
{
    public class SubscribeVehicleData : RpcRequest
    {
        public override FunctionID FunctionId { get; set; } = FunctionID.SubscribeVehicleData;
        public bool Speed { get; set; } = true;
        public bool Rpm { get; set; } = true;
        public bool ExternalTemperature { get; set; } = true;
        public bool FuelLevel { get; set; } = true;
        public bool Prndl { get; set; } = true;
        public bool TirePressure { get; set; } = true;
        public bool EngineTorque { get; set; } = true;
        public bool Odometer { get; set; } = true;
        public bool Gps { get; set; } = true;
        public bool FuelLevelState { get; set; } = true;
        public bool InstantFuelConsumption { get; set; } = true;
        public bool BeltStatus { get; set; } = true;
        public bool BodyInformation { get; set; } = true;
        public bool DeviceStatus { get; set; } = true;
        public bool DriverBraking { get; set; } = true;
        public bool WiperStatus { get; set; } = true;
        public bool HeadLampStatus { get; set; } = true;
        public bool AccPedalPosition { get; set; } = true;
        public bool SteeringWheelAngle { get; set; } = true;
        public bool ECallInfo { get; set; } = true;
        public bool AirbagStatus { get; set; } = true;
        public bool EmergencyEvent { get; set; } = true;
        public bool ClusterModeStatus { get; set; } = true;
        public bool MyKey { get; set; } = true;
    }

}
