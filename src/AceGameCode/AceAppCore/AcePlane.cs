using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Apian;
using UnityEngine;

namespace AceGameCode
{

    public interface IAcePlane : IApianCoreData
    {

    }
    public class AcePlane : IAcePlane
    {
        public AcePlane()
        {

        }

        // Custom compact json
        // TODO: set up params to make more compact.
        public static AcePlane FromApianJson(string jsonData)
        {
            object[] data = JsonConvert.DeserializeObject<object[]>(jsonData);
            return new AcePlane();
        }

        public string ApianSerialized(object args=null)
        {
            return  JsonConvert.SerializeObject(new object[]{ });
        }

    }
}