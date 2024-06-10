using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SharedPlayerSettings
{
   #region - Player -

    [Serializable]
    public class PlayerSettingsModel
    {
        [Header("View Settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;

        public bool ViewXInverted;
        public bool ViewYInverted;

        [Header("Movement")]
        public float WalkingForwardSpeed;
        public float WalkingStrafeSpeed;
        public float WalkingBackwardSpeed;
    }

   #endregion
}
