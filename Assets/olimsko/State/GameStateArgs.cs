using System;
using UnityEngine;

namespace olimsko
{
    public class GameStateArgs : EventArgs
    {
        public readonly string SlotId;
        /// <summary>
        /// Whether it's a quick save/load operation.
        /// </summary>
        public readonly bool Quick;

        public GameStateArgs(string slotId, bool quick)
        {
            SlotId = slotId;
            Quick = quick;
        }
    }
}