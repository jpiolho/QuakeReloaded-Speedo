using QuakeReloaded.Interfaces;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.Sigscan.Definitions;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Speedo.Template;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection.PortableExecutable;

namespace Speedo
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private unsafe float* _x, _y, _z;
        
        private IntPtr _cvarSpeed, _cvarSpeedUpdate, _cvarSpeedX, _cvarSpeedY, _cvarSpeedStyle;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _modConfig = context.ModConfig;


            var mainModule = Process.GetCurrentProcess().MainModule!;

            if (!(_modLoader.GetController<IStartupScanner>()?.TryGetTarget(out var scanner) ?? false))
                throw new Exception("Failed to get scanner");

            if (!(_modLoader.GetController<IQuakeReloaded>()?.TryGetTarget(out var qreloaded) ?? false))
                throw new Exception("Could not get QuakeReloaded API. Are you sure QuakeReloaded is installed & loaded before this mod?");

            qreloaded.Events.RegisterOnPreInitialize(() =>
            {
                _cvarSpeed = qreloaded.Cvars.Register("scr_speed", "0", "Draws a velocity meter on the screen. 0 = Off, 1 = Normal, 2 = Horizontal speed only", CvarFlags.Integer | CvarFlags.Saved, 0, 1);
                _cvarSpeedStyle = qreloaded.Cvars.Register("scr_speed_style", "0", "0 = Normal, 1 = Minimalist", CvarFlags.Integer | CvarFlags.Saved, 0, 1);
                _cvarSpeedUpdate = qreloaded.Cvars.Register("scr_speed_update", "0.2", "How frequent it should update. Note that higher value will be more constant whereas smaller will include more noise", CvarFlags.Float | CvarFlags.Saved, 0, 10);
                _cvarSpeedX = qreloaded.Cvars.Register("scr_speed_x", "0.10", "X position of the velocity meter (0: Left edge, 1: Right edge)", CvarFlags.Float | CvarFlags.Saved, 0, 1);
                _cvarSpeedY = qreloaded.Cvars.Register("scr_speed_y", "0.70", "Y position of the velocity meter (0: Top edge, 1: Bottom edge)", CvarFlags.Float | CvarFlags.Saved, 0, 1);

            });


            Vector3 lastPos = Vector3.Zero;
            double lastSpeed = 0.0;
            double fastestSpeed = 0.0f;
            float previousUpdateTime = 0.0f;
            qreloaded.Events.RegisterOnRenderFrame(() =>
            {
                unsafe {

                    var scrSpeed = qreloaded.Cvars.GetFloatValue(_cvarSpeed);
                    if (scrSpeed > 0)
                    {
                        var pos = new Vector3(*_x, *_y, *_z);

                        // If horizontal speed only, then remove the Z
                        if (scrSpeed == 2)
                        {
                            pos.Z = 0;
                            lastPos.Z = 0;
                        }

                        double distance = (pos - lastPos).Length();

                        fastestSpeed = Math.Max(fastestSpeed, distance);

                        // Calculate speed
                        if (qreloaded.Game.MapTime < previousUpdateTime || qreloaded.Game.MapTime >= previousUpdateTime + qreloaded.Cvars.GetFloatValue(_cvarSpeedUpdate,0.1f))
                        {
                            if (fastestSpeed > 0.0)
                                lastSpeed = fastestSpeed / (qreloaded.Game.MapTime - previousUpdateTime);
                            else
                                lastSpeed = 0.0f;

                            fastestSpeed = 0.0f;
                            previousUpdateTime = qreloaded.Game.MapTime;
                            lastPos = pos;
                        }

                        // Draw the text
                        var style = (int)qreloaded.Cvars.GetFloatValue(_cvarSpeedStyle, 0);
                        string text;
                        switch(style)
                        {
                            default:
                            case 0: text = $"Vel: {lastSpeed.ToString("0.0", CultureInfo.InvariantCulture)}"; break;
                            case 1: text = lastSpeed.ToString("0.0", CultureInfo.InvariantCulture); break;
                        }

                        qreloaded.UI.DrawText(text, qreloaded.Cvars.GetFloatValue(_cvarSpeedX,0.5f), qreloaded.Cvars.GetFloatValue(_cvarSpeedY, 0.1f));
                    }
                }
            });

            // Search for camera XYZ
            scanner.AddMainModuleScan("4C 8D 3D ?? ?? ?? ?? 48 8D 2D ?? ?? ?? ??", (result) =>
            {
                unsafe
                {
                    int offset = *(int*)(mainModule.BaseAddress + result.Offset + 3);
                    _x = (float*)(mainModule.BaseAddress + result.Offset + offset + 7) - 1;
                    _y = _x + 6;
                    _z = _y + 6;
                }
            });
        }


        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}