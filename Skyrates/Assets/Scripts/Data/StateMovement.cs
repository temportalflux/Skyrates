
using System;
using UnityEngine;

namespace Skyrates.Data
{

    [Serializable]
    public class StateMovement
    {

        public float SpeedInitial = 10;

        // TODO: Calculate via ship ocmponents
        public float SpeedMin = 0;

        // TODO: Calculate via ship ocmponents
        public float SpeedMax = 60;

        public float CurrentSpeed;

    }

}