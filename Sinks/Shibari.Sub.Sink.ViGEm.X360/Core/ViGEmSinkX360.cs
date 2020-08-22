﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.ExceptionServices;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Serilog;
using Shibari.Sub.Core.Shared.Types.Common;
using Shibari.Sub.Core.Shared.Types.Common.Sinks;
using Shibari.Sub.Core.Shared.Types.DualShock3;

namespace Shibari.Sub.Sink.ViGEm.X360.Core
{
    internal class ಠ_ಠAttribute : Attribute
    {
    }

    [ExportMetadata("Name", "ViGEm Xbox 360 Sink")]
    [Export(typeof(ISinkPlugin))]
    public class ViGEmSinkX360 : SinkPluginBase, ISinkPlugin
    {
        private readonly Dictionary<DualShock3Buttons, Xbox360Button> _btnMap;
        private readonly ViGEmClient _client;

        private readonly Dictionary<IDualShockDevice, IXbox360Controller> _deviceMap =
            new Dictionary<IDualShockDevice, IXbox360Controller>();

        private readonly Dictionary<DualShock3Axes, Xbox360Slider> _triggerAxisMap;
        private readonly Dictionary<DualShock3Axes, Xbox360Axis> _XaxisMap;
        private readonly Dictionary<DualShock3Axes, Xbox360Axis> _YaxisMap;

        public ViGEmSinkX360()
        {
            _btnMap = new Dictionary<DualShock3Buttons, Xbox360Button>
            {
                {DualShock3Buttons.Select, Xbox360Button.Back},
                {DualShock3Buttons.LeftThumb, Xbox360Button.LeftThumb},
                {DualShock3Buttons.RightThumb, Xbox360Button.RightThumb},
                {DualShock3Buttons.Start, Xbox360Button.Start},
                {DualShock3Buttons.LeftShoulder, Xbox360Button.LeftShoulder},
                {DualShock3Buttons.RightShoulder, Xbox360Button.RightShoulder},
                {DualShock3Buttons.Triangle, Xbox360Button.Y},
                {DualShock3Buttons.Circle, Xbox360Button.B},
                {DualShock3Buttons.Cross, Xbox360Button.A},
                {DualShock3Buttons.Square, Xbox360Button.X},
                {DualShock3Buttons.DPadUp, Xbox360Button.Up},
                {DualShock3Buttons.DPadDown, Xbox360Button.Down},
                {DualShock3Buttons.DPadLeft, Xbox360Button.Left},
                {DualShock3Buttons.DPadRight, Xbox360Button.Right},
                {DualShock3Buttons.Ps, Xbox360Button.Guide}
            };

            _XaxisMap = new Dictionary<DualShock3Axes, Xbox360Axis>
            {
                {DualShock3Axes.LeftThumbX, Xbox360Axis.LeftThumbX},
                {DualShock3Axes.RightThumbX, Xbox360Axis.RightThumbX}
            };

            _YaxisMap = new Dictionary<DualShock3Axes, Xbox360Axis>
            {
                {DualShock3Axes.LeftThumbY, Xbox360Axis.LeftThumbY},
                {DualShock3Axes.RightThumbY, Xbox360Axis.RightThumbY}
            };

            _triggerAxisMap = new Dictionary<DualShock3Axes, Xbox360Slider>
            {
                {DualShock3Axes.LeftTrigger, Xbox360Slider.LeftTrigger},
                {DualShock3Axes.RightTrigger, Xbox360Slider.RightTrigger}
            };

            try
            {
                _client = new ViGEmClient();
            }
            catch (Exception ex)
            {
                Log.Fatal("ViGEm allocation failed, are you sure ViGEmBus is installed? Error details: {Exception}",
                    ex);
            }
        }

        public event RumbleRequestReceivedEventHandler RumbleRequestReceived;

        [HandleProcessCorruptedStateExceptions]
        public void DeviceArrived(IDualShockDevice device)
        {
            var target = _client.CreateXbox360Controller();
            target.AutoSubmitReport = false;

            _deviceMap.Add(device, target);

            if (Configuration.Rumble.IsEnabled)
            {
                target.FeedbackReceived += (sender, args) =>
                {
                    var source = _deviceMap.First(m => m.Value.Equals(sender)).Key;

                    RumbleRequestReceived?.Invoke(source, new RumbleRequestEventArgs(args.LargeMotor, args.SmallMotor));
                };
            }
            else
            {
                Log.Warning("Rumble disabled by configuration");
            }

            try
            {
                Log.Information("Connecting ViGEm target {Target}", target);
                target.Connect();
                Log.Information("ViGEm target {Target} connected successfully", target);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to connect target {@Target}: {Exception}", target, ex);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public void DeviceRemoved(IDualShockDevice device)
        {
            _deviceMap[device].Disconnect();
            _deviceMap.Remove(device);
        }

        [HandleProcessCorruptedStateExceptions]
        public void InputReportReceived(IDualShockDevice device, IInputReport report)
        {
            switch (device.DeviceType)
            {
                case DualShockDeviceType.DualShock3:

                    var target = _deviceMap[device];
                    target.ResetReport();

                    var ds3Report = (DualShock3InputReport) report;

                    foreach (var axis in _XaxisMap) target.SetAxisValue(axis.Value, Scale(ds3Report[axis.Key], false));

                    foreach (var axis in _YaxisMap) target.SetAxisValue(axis.Value, Scale(ds3Report[axis.Key], true));

                    foreach (var axis in _triggerAxisMap) target.SetSliderValue(axis.Value, ds3Report[axis.Key]);

                    foreach (var button in _btnMap.Where(m => ds3Report.EngagedButtons.Contains(m.Key))
                        .Select(m => m.Value)) target.SetButtonState(button, true);

                    target.SubmitReport();

                    break;
            }
        }

        [ಠ_ಠ]
        private static short Scale(byte value, bool invert)
        {
            var intValue = value - 0x80;
            if (intValue == -128) intValue = -127;

            var wtfValue = intValue * 258.00787401574803149606299212599f; // what the fuck?

            return (short) (invert ? -wtfValue : wtfValue);
        }
    }
}