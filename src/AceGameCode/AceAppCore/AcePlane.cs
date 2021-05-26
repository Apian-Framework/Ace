using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Apian;
using UnityEngine;

namespace AceGameCode
{

    public interface IAcePlane : IApianCoreData
    {
        string PlaneId {get;}
        string PlayerId {get;}
        string Name {get;}
        string CtrlType {get;}
        Vector2 Position {get;}
        PlaneOrientation Orientation {get;}
        //string OrientationCommitHash {get;}
    }
    public class AcePlane : IAcePlane
    {
        public string PlaneId {get; protected set;}
        public string PlayerId {get; protected set;}
        public string Name {get; protected set;}
        public string CtrlType {get; protected set;}
        public Vector2 Position {get; protected set;}
        public PlaneOrientation Orientation {get; protected set;}

        public AcePlane(string _id, string _peerId, string _name, string ctrl, Vector2 initialPos, PlaneOrientation initialOrient)
        {
            PlaneId = _id;
            PlayerId = _peerId;
            Name = _name;
            CtrlType = ctrl;
            Position = initialPos;
            Orientation = initialOrient;
        }

        // Custom compact json
        // TODO: set up params to make more compact.
        public static AcePlane FromApianJson(string jsonData)
        {
            object[] data = JsonConvert.DeserializeObject<object[]>(jsonData);
            return new AcePlane(
                (string)data[0],
                (string)data[1],
                (string)data[2],
                (string)data[3],
                new Vector2((int)(long)data[4], (int)(long)data[5]),
                (PlaneOrientation)(long)data[6]
            );
        }

        public string ApianSerialized(object args=null)
        {
            return  JsonConvert.SerializeObject(new object[]{
                PlaneId,
                PlayerId,
                Name,
                CtrlType,
                (int)Position.x,
                (int)Position.y,
                Orientation,
            });
        }

    }
}