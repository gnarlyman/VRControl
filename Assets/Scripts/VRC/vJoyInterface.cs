﻿using System;
using UnityEngine;
using vJoyInterfaceWrap;

namespace VRC
{
    public class VJoyInterface : MonoBehaviour
    {
        private enum VJoyStatus
        {
            Unknown,
            NotInstalled,
            VersionMismatch,
            DeviceUnavailable,
            DeviceOwned,
            DeviceError,
            DeviceMisconfigured,
            DeviceNotAquired,
            Ready,
        }
        
        [Range(0f, 90f)]
        public float joystickDeadzoneDegrees = 0f;
        [Range(0f, 90f)]
        public float joystickMaxDegrees = 90f;
        [Range(0f, 100f)]
        public float throttleDeadzonePercentage = 0f;
        [Range(0f, 100f)]
        public float directionalThrustersDeadzonePercentage = 0f;
        [Range(8, 128)]
        public uint maxButtons = 32;

        public uint deviceId = 1;

        private vJoy vjoy;
        private vJoy.JoystickState iReport;
        
        // tracking button state
        private uint buttons;
        
        private static VJoyStatus vJoyStatus = VJoyStatus.Unknown;
        

        private static void SetStatus(VJoyStatus status)
        {
            vJoyStatus = status;
            
            Debug.Log($"vJoy status changed to: {status}");
        }
        
        private void OnEnable()
        {
            vjoy = new vJoy();
            
            if (!vjoy.vJoyEnabled())
            {
                SetStatus(VJoyStatus.NotInstalled);
                enabled = false;
                return;
            }
            
            uint dllVer = 0, drvVer = 0;
            if (vjoy.DriverMatch(ref dllVer, ref drvVer))
            {
                Debug.Log($"vJoy Driver Version Matches vJoy DLL Version ({dllVer:X})");
            }
            else
            {
                Debug.LogError($"vJoy Driver Version ({drvVer:X}) " +
                               $"does NOT match vJoy DLL Version ({dllVer:X})");
                SetStatus(VJoyStatus.VersionMismatch);
                enabled = false;
                return;
            }
            
            var deviceStatus = vjoy.GetVJDStatus(deviceId);
            switch (deviceStatus)
            {
                case VjdStat.VJD_STAT_FREE:
                case VjdStat.VJD_STAT_OWN:
                    // We can continue if the device is free or we own it
                    break;
                case VjdStat.VJD_STAT_MISS:
                    Debug.LogError($"vJoy Device {deviceId} is not installed or is disabled");
                    SetStatus(VJoyStatus.DeviceUnavailable);
                    enabled = false;
                    return;
                case VjdStat.VJD_STAT_BUSY:
                    Debug.LogError($"vJoy Device {deviceId} is owned by another application");
                    SetStatus(VJoyStatus.DeviceOwned);
                    enabled = false;
                    return;
                case VjdStat.VJD_STAT_UNKN:
                    Debug.LogError($"vJoy Device {deviceId} unknown error");
                    SetStatus(VJoyStatus.DeviceError);
                    enabled = false;
                    return;
                default:
                    Debug.LogError("Unknown vJoy device status error");
                    SetStatus(VJoyStatus.DeviceError);
                    enabled = false;
                    return;
            }
            
            if (!IsDeviceValid(deviceId))
            {
                Debug.LogError("vJoy device is not configured correctly");
                SetStatus(VJoyStatus.DeviceMisconfigured);
                enabled = false;
                return;
            }

            if (deviceStatus != VjdStat.VJD_STAT_FREE)
            {
                if (deviceStatus == VjdStat.VJD_STAT_OWN)
                {
                    Debug.Log($"vJoy device {deviceId} already acquired");
                }
            }
            else
            {
                if (vjoy.AcquireVJD(deviceId))
                {
                    Debug.Log($"Acquired vJoy device {deviceId}");
                }
                else
                {
                    Debug.LogError($"Unable to acquire vJoy device {deviceId}");
                    SetStatus(VJoyStatus.DeviceNotAquired);
                    enabled = false;
                    return;
                }
            }

            SetStatus(VJoyStatus.Ready);
        }

        private void OnDisable()
        {
            if (vJoyStatus != VJoyStatus.Ready) return;
            
            vjoy.RelinquishVJD(deviceId);
            SetStatus(VJoyStatus.Unknown);
        }

        private void Update()
        {
            iReport.bDevice = (byte) deviceId;
            
            iReport.Buttons = buttons;
            
            // ReSharper disable once InvertIf
            if (!vjoy.UpdateVJD(deviceId, ref iReport))
            {
                SetStatus(VJoyStatus.DeviceError);
                enabled = false;
            }
        }

        public void SetButton(uint buttonNumber, bool pressed)
        {
            var buttonIndex = (int)buttonNumber - 1;
            if (buttonNumber == 0)
            {
                throw new System.IndexOutOfRangeException(
                    "Button number 0 is too low, button numbers are zero indexed");
            }
            if (buttonIndex >= maxButtons)
            {
                throw new System.IndexOutOfRangeException(
                    $"Button index {buttonIndex} is too high");
            }

            if (pressed)
            {
                buttons |= (uint)1 << buttonIndex;
            }
            else
            {
                buttons &= ~((uint)1 << buttonIndex);
            }
        }

        public void SetZAxis(float axis)
        {
            var z = ScaleAxis(axis, HID_USAGES.HID_USAGE_Z);
            Debug.Log($"ZAxis:{z} RawAxis:{axis}");
            iReport.AxisZ = z;
        }

        private int ConvertAxisRatioToAxisInt(float axisRatio, HID_USAGES hid)
        {
            long min = 0, max = 0;
            var gotMin = vjoy.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_X, ref min);
            var gotMax = vjoy.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_X, ref max);
            if (!gotMin || !gotMax)
            {
                Debug.LogWarning($"Error getting min/max of HID axis {hid}");
                return 0;
            }

            // Get an absolute ratio where 0 is -Max, .5 is 0, and 1 is +Max
            var absRatio = axisRatio / 2f + .5f;
            var range = max - min;
            return (int)((long)(range * absRatio) + min);
        }

        /// <summary>scale value m from 0 - 1 to device range</summary>
        /// <param name="m">the value to scale</param>
        /// <param name="hid">the HID axis type</param>
        private int ScaleAxis(float m, HID_USAGES hid)
        {
            // reference range
            const float rMin = 0,  rMax = 1;
            // target range
            long tMin = 0, tMax = 0;
            var gotMin = vjoy.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_X, ref tMin);
            var gotMax = vjoy.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_X, ref tMax);
            if (!gotMin || !gotMax)
            {
                Debug.LogWarning($"Error getting min/max of HID axis {hid}");
                return 0;
            }
            
            // scale m to target scale
            var result = (m - rMin) / (rMax - rMin) * (tMax - tMin) + tMin;
            return (int)result;
        }
        
        /**
         * Checks to make sure the vJoy device has all the required configuration
         * @note Make sure to update this when adding code that adds buttons, axis, haptics, etc
         */
        private bool IsDeviceValid(uint did)
        {
            var buttonN = vjoy.GetVJDButtonNumber(did);
            var hatN = vjoy.GetVJDDiscPovNumber(did);

            if (buttonN < 8)
            {
                Debug.LogWarning($"vJoy device has {buttonN} buttons, at least 8 are required");
                return false;
            }

            if (hatN < 4)
            {
                Debug.LogWarning($"vJoy device has {hatN} directional pov hat switches, " +
                                 "4 configured as directional are required");
                return false;
            }

            var xAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_X);
            var yAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_Y);
            var rzAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_RZ);
            if (!xAxis || !yAxis || !rzAxis)
            {
                Debug.LogWarning("vJoy device is missing one of the X/Y/Rz axis needed for the joystick " +
                                       $"[X:{xAxis}, Y: {yAxis}, Rz:{rzAxis}]");
                return false;
            }

            var zAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_Z);
            if (!zAxis)
            {
                Debug.LogWarning("vJoy device is missing the Z axis needed for the throttle");
                return false;
            }

            var rxAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_RX);
            var ryAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_RY);
            var sliderAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_SL0);
            if (!rxAxis || !ryAxis || !sliderAxis)
            {
                Debug.LogWarning("vJoy device is missing one of the Rx/Ry/Slider axis needed " +
                                 $"for the thruster axis [Rx:{rxAxis}, Ry: {ryAxis}, Slider:{sliderAxis}]");
                return false;
            }

            var dialAxis = vjoy.GetVJDAxisExist(did, HID_USAGES.HID_USAGE_SL1);
            
            if (dialAxis) return true;
            
            Debug.LogWarning("vJoy device is missing the Dial/Slider2 axis needed for the map zoom axis");
            return false;
        }
    }
}