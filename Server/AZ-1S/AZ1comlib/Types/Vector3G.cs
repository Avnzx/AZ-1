// Vector3 "galactic" edition:tm:

using System;
using Godot;

public struct Vector3G {
        public int XC = 0;
        public int YC = 0;
        public int ZC = 0;
        public realt X = 0;
        public realt Y = 0;
        public realt Z = 0;



        public Vector3G() {}


        /// <param name="_XC">the new x chunk.</param>
        /// <param name="_YC">the new y chunk.</param>
        /// <param name="_ZC">the new z chunk.</param>
        /// <param name="_X">the new x coord inside the chunk.</param>
        /// <param name="_Y">the new y coord inside the chunk.</param>
        /// <param name="_Z">the new z coord inside the chunk.</param>
        public Vector3G(int _XC, int _YC, int _ZC, realt _X, realt _Y, realt _Z) {
            XC = _XC;   YC = _YC;   ZC = _ZC;
            X = _X;     Y = _Y;     Z = _Z;
        }

        public static explicit operator Godot.Vector3(Vector3G vg) {
            return new Vector3(
                vg.XC*FrontierConstants.chunkSize + vg.X,
                vg.YC*FrontierConstants.chunkSize + vg.Y,
                vg.ZC*FrontierConstants.chunkSize + vg.Z);
        }

        public static explicit operator (Godot.Vector3I, Godot.Vector3) (Vector3G vg) {
            return (new Vector3I(vg.XC, vg.YC, vg.ZC), new Vector3(vg.X, vg.Y, vg.Z));
        }

        public static implicit operator (string, Godot.Vector3) (Vector3G vg) {
            var st = new string("");
            st = vg.XC.ToString() + "_" + vg.YC.ToString()  + "_" +  vg.ZC.ToString();
            return (st, new Vector3(vg.X, vg.Y, vg.Z));
        }

}