using UnityEngine;

public static class Utilities {
    const int ViewDirectionsCount = 100;
    public static readonly Vector3[] directions;

    static Utilities() {
        directions = new Vector3[ViewDirectionsCount];
        var phi = (1 + Mathf.Sqrt (5)) / 2;
        var angleIncrement = Mathf.PI * 2 * phi;

        for (var i = 0; i < ViewDirectionsCount; i++) {
            var t = (float) i / ViewDirectionsCount;
            var inclination = Mathf.Acos(1 - 2 * t);
            var azimuth = angleIncrement * i;

            var x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            var y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            var z = Mathf.Cos(inclination);
            directions[i] = new Vector3(x, y, z);
        }
    }
}
