using System;
// ReSharper disable UnusedMember.Global

namespace BoosterPumpLibrary.Settings
{
    /// <summary>
    /// { Undefined | Input | Output | InputAndOutput }
    /// </summary>
    [Flags]
    public enum Direction
    {
        Undefined = 0,

        Input = 1,

        Output = 2,

        InputAndOutput = 3
    }
}
