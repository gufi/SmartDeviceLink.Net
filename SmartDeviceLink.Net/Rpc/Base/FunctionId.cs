namespace SmartDeviceLink.Net.Rpc.Base
{
    //find replace
    //[\D]+\((-\d+), "([\w]+)"\),
    //$2 = $1, \r\n
    // ReSharper disable once InconsistentNaming
    public enum FunctionID
    {
        SyncPData = 65537,
        OnSyncPData = 98305,
        EncodedSyncPData = 65536,
        OnEncodedSyncPData = 98304,

        // REQUESTS & RESPONSES
        RegisterAppInterface = 1,
        UnregisterAppInterface = 2,
        SetGlobalProperties = 3,
        ResetGlobalProperties = 4,
        AddCommand = 5,
        DeleteCommand = 6,
        AddSubMenu = 7,
        DeleteSubMenu = 8,
        CreateInteractionChoiceSet = 9,
        PerformInteraction = 10,
        DeleteInteractionChoiceSet = 11,
        Alert = 12,
        Show = 13,
        Speak = 14,
        SetMediaClockTimer = 15,
        PerformAudioPassThru = 16,
        EndAudioPassThru = 17,
        SubscribeButton = 18,
        UnsubscribeButton = 19,
        SubscribeVehicleData = 20,
        UnsubscribeVehicleData = 21,
        GetVehicleData = 22,
        // ReSharper disable once InconsistentNaming
        ReadDID = 23,
        // ReSharper disable once InconsistentNaming
        GetDTCs = 24,
        ScrollableMessage = 25,
        Slider = 26,
        // ReSharper disable once InconsistentNaming
        ShowConstantTBT = 27,
        AlertManeuver = 28,
        UpdateTurnList = 29,
        ChangeRegistration = 30,
        GenericResponse = 31,
        PutFile = 32,
        DeleteFile = 33,
        ListFiles = 34,
        SetAppIcon = 35,
        SetDisplayLayout = 36,
        DiagnosticMessage = 37,
        SystemRequest = 38,
        SendLocation = 39,
        DialNumber = 40,
        ButtonPress = 41,
        GetInteriorVehicleData = 43,
        SetInteriorVehicleData = 44,
        GetWayPoints = 45,
        SubscribeWayPoints = 46,
        UnsubscribeWayPoints = 47,
        GetSystemCapability = 48,
        SendHapticData = 49,
        // ReSharper disable once InconsistentNaming
        OnHMIStatus = 32768,
        OnAppInterfaceUnregistered = 32769,
        OnButtonEvent = 32770,
        OnButtonPress = 32771,
        OnVehicleData = 32772,
        OnCommand = 32773,
        // ReSharper disable once InconsistentNaming
        OnTBTClientState = 32774,
        OnDriverDistraction = 32775,
        OnPermissionsChange = 32776,
        OnAudioPassThru = 32777,
        OnLanguageChange = 32778,
        OnKeyboardInput = 32779,
        OnTouchEvent = 32780,
        OnSystemRequest = 32781,
        OnHashChange = 32782,
        OnInteriorVehicleData = 32783,
        OnWayPointChange = 32784,

        // MOCKED FUNCTIONS (NOT SENT FROM HEAD-UNIT)
        OnLockScreenStatus = -1, 
        OnSdlChoiceChosen = -1, 
        OnStreamRPC = -1, 
        StreamRPC = -1, 

    }
}
