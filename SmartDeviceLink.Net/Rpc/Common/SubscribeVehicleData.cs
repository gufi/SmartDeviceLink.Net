using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Rpc.Common
{
    public class SubscribeVehicleData : RpcRequest
    {
        public override FunctionID FunctionId { get; set; } = FunctionID.SubscribeVehicleData;
        public bool speed { get; set; } = true;
        public bool rpm { get; set; } = true;
        public bool externalTemperature { get; set; } = true;
        public bool fuelLevel { get; set; } = true;
        public bool prndl { get; set; } = true;
        public bool tirePressure { get; set; } = true;
        public bool engineTorque { get; set; } = true;
        public bool odometer { get; set; } = true;
        public bool gps { get; set; } = true;
        public bool fuelLevel_State { get; set; } = true;
        public bool instantFuelConsumption { get; set; } = true;
        public bool beltStatus { get; set; } = true;
        public bool bodyInformation { get; set; } = true;
        public bool deviceStatus { get; set; } = true;
        public bool driverBraking { get; set; } = true;
        public bool wiperStatus { get; set; } = true;
        public bool headLampStatus { get; set; } = true;
        public bool accPedalPosition { get; set; } = true;
        public bool steeringWheelAngle { get; set; } = true;
        public bool eCallInfo { get; set; } = true;
        public bool airbagStatus { get; set; } = true;
        public bool emergencyEvent { get; set; } = true;
        public bool clusterModeStatus { get; set; } = true;
        public bool myKey { get; set; } = true;
    }

}
