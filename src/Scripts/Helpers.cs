﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers : MonoBehaviour {

    /* Finds a point on Plane p closest to Vector3 point */
    public static Vector3 ClosestPointOnPlane(Plane p, Vector3 point)
    {
        float d = Vector3.Dot(p.normal, point) + p.distance;
        return point - p.normal * d;
    }

    public static Vector2 UTMToLatLon(double utmX, double utmY, string utmZone)
    {
        // from https://stackoverflow.com/questions/2689836/converting-utm-wsg84-coordinates-to-latitude-and-longitude
        bool isNorthHemisphere = utmZone.Last() >= 'N';

        var diflat = -0.00066286966871111111111111111111111111;
        var diflon = -0.0003868060578;

        var zone = int.Parse(utmZone.Remove(utmZone.Length - 1));
        var c_sa = 6378137.000000;
        var c_sb = 6356752.314245;
        var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
        var e2cuadrada = Math.Pow(e2, 2);
        var c = Math.Pow(c_sa, 2) / c_sb;
        var x = utmX - 500000;
        var y = isNorthHemisphere ? utmY : utmY - 10000000;

        var s = ((zone * 6.0) - 183.0);
        var lat = y / (c_sa * 0.9996);
        var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
        var a = x / v;
        var a1 = Math.Sin(2 * lat);
        var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
        var j2 = lat + (a1 / 2.0);
        var j4 = ((3 * j2) + a2) / 4.0;
        var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
        var alfa = (3.0 / 4.0) * e2cuadrada;
        var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
        var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
        var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
        var b = (y - bm) / v;
        var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
        var eps = a * (1 - (epsi / 3.0));
        var nab = (b * (1 - epsi)) + lat;
        var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
        var delt = Math.Atan(senoheps / (Math.Cos(nab)));
        var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

        double longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
        double latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;

        return new Vector2((float)longitude, (float)latitude);
    } 

    public static Vector2 LonLatToUTM(double lon, double lat)
    {
        // from https://stackoverflow.com/questions/176137/java-convert-lat-lon-to-utm
        double easting;
        double northing;
        int zone;

        zone = (int)Math.Floor(lon / 6 + 31);

        easting = 0.5 * Math.Log((1 + Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)) / (1 - Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180))) * 0.9996 * 6399593.62 / Math.Pow((1 + Math.Pow(0.0820944379, 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)), 0.5) * (1 + Math.Pow(0.0820944379, 2) / 2 * Math.Pow((0.5 * Math.Log((1 + Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)) / (1 - Math.Cos(lat * Math.PI / 180) * Math.Sin(lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)))), 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2) / 3) + 500000;
        easting = Math.Round(easting * 100) * 0.01;
        northing = (Math.Atan(Math.Tan(lat * Math.PI / 180) / Math.Cos((lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180))) - lat * Math.PI / 180) * 0.9996 * 6399593.625 / Math.Sqrt(1 + 0.006739496742 * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) * (1 + 0.006739496742 / 2 * Math.Pow(0.5 * Math.Log((1 + Math.Cos(lat * Math.PI / 180) * Math.Sin((lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180))) / (1 - Math.Cos(lat * Math.PI / 180) * Math.Sin((lon * Math.PI / 180 - (6 * zone - 183) * Math.PI / 180)))), 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) + 0.9996 * 6399593.625 * (lat * Math.PI / 180 - 0.005054622556 * (lat * Math.PI / 180 + Math.Sin(2 * lat * Math.PI / 180) / 2) + 4.258201531e-05 * (3 * (lat * Math.PI / 180 + Math.Sin(2 * lat * Math.PI / 180) / 2) + Math.Sin(2 * lat * Math.PI / 180) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) / 4 - 1.674057895e-07 * (5 * (3 * (lat * Math.PI / 180 + Math.Sin(2 * lat * Math.PI / 180) / 2) + Math.Sin(2 * lat * Math.PI / 180) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) / 4 + Math.Sin(2 * lat * Math.PI / 180) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2) * Math.Pow(Math.Cos(lat * Math.PI / 180), 2)) / 3);
		if (lat < 0) {
			northing = northing + 10000000;
		}
        northing = Math.Round(northing * 100) * 0.01;

        return new Vector2((float)easting, (float)northing);
    }

    /* Transforms spherical to Cartesian coordinates 
     Preconditions: theta and phi must be in radians
     FOR RIGHT HANDED COORDINATE SYSTEM (y is up)
         */
    public static Vector3 SphericalToCartesian(float theta, float phi, float radius)
    {
        float y = radius * -Mathf.Sin(phi);
        float rCosElevation = radius * Mathf.Cos(phi);
        float x = rCosElevation * -Mathf.Cos(theta);
        float z = rCosElevation * -Mathf.Sin(theta);

        return new Vector3(x, y, z);
    }

    public static Vector3 SphericalToCartesian(Vector3 sphericalCoords)
    {
        return SphericalToCartesian(sphericalCoords.x, sphericalCoords.y, sphericalCoords.z);
    }

    public static Vector3 CartesianToSpherical(float x, float y, float z)
    {
        float hypotinuseXZ = Mathf.Sqrt(Mathf.Abs(x) * Mathf.Abs(x) + Mathf.Abs(z) * Mathf.Abs(z));
        float radius = Mathf.Sqrt(Mathf.Abs(hypotinuseXZ) * Mathf.Abs(hypotinuseXZ) + Mathf.Abs(y) * Mathf.Abs(y));
        float phi = Mathf.Atan2(y, hypotinuseXZ);
        float theta = Mathf.Atan2(z, x);

        return new Vector3(theta, phi, radius);
    }

    public static Vector3 CartesianToSpherical(Vector3 cartesianCoords)
    {
        return CartesianToSpherical(cartesianCoords.x, cartesianCoords.y, cartesianCoords.z);
    }

    public static Vector2 GetTrendAndPlungeFromNormal(Vector3 dir)
    {
        Vector3 spherical = CartesianToSpherical(dir);
        float theta = spherical.x;
        float phi = spherical.y;
        // float radius = spherical.z;

        float trend;
        float plunge;

        if (phi > 0)
        {
            trend = (3 * Mathf.PI / 2 - theta) * 180 / Mathf.PI;
            plunge = phi * 180 / Mathf.PI;
        }
        else
        {
            trend = (Mathf.PI / 2 - theta) * 180 / Mathf.PI;
            plunge = -phi * 180 / Mathf.PI;
        }

        if (trend > 360)
        {
            trend = trend - 360;
        }

        return new Vector2(trend, plunge);
    }

	public static Vector3 GetDirCosFromTrendAndPlunge(Vector2 trendPlunge) {
		float trend = trendPlunge.x;
		float plunge = trendPlunge.y;
		float radius = 1;

		float theta, phi;

		theta = (90 - trend) * Mathf.PI / 180;
		phi = -plunge * Mathf.PI / 180;

		return SphericalToCartesian (new Vector3 (theta, phi, radius));
	}

    public static Vector2 GetStrikeAndDipFromNormal(Vector3 dir)
    {
        Vector2 trendAndPlunge = GetTrendAndPlungeFromNormal(dir);
        float trend = trendAndPlunge.x;
        float plunge = trendAndPlunge.y;
        
        float strike;
        float dip;
        
        strike = trend + 90;
        if (strike > 360)
        {
            strike = strike - 360;
        } 

        dip = 90 - plunge;

        return new Vector2(strike, dip);
    }

    private static double svd_pythag(double a, double b)
    {
        double tolerance = 0.00000001;

        a = Math.Abs(a);
        b = Math.Abs(b);

        if (a > b)
        {
            return a * Math.Sqrt(1.0 + (b * b / a / a));
        }
        else if (b < tolerance)
        {
            return a;
        }

        return b * Math.Sqrt(1.0 + (a * a / b / b));
    }

    public static double[,] SVD_V(double[,] A)
    {
        // from http://www.numericjs.com/lib/numeric-1.2.6.js
        //Compute the thin SVD from G. H. Golub and C. Reinsch, Numer. Math. 14, 403-420 (1970)
        double temp;
        double prec = 2.220446049250313e-16; ; //Math.pow(2,-52) // assumes double prec
        double tolerance = (1e-64) / prec;
        double itmax = 50;
        double c = 0;
        int i = 0;
        int j = 0;
        int k = 0;
        int l = 0;

        double[,] u = new double[A.GetLength(0),A.GetLength(1)];
        for (i = 0; i < A.GetLength(0); i++)
        {
            for (j = 0; j < A.GetLength(1); j++)
            {
                u[i, j] = A[i, j];
            }
            
        }

        i = 0;
        j = 0;

        int m = u.GetLength(0);
        int n = u.GetLength(1);

        if (m < n) {
            throw new Exception("SVDError: Need more rows than columns");
        }

        double[] e = new double[n];
        double[] q = new double[n];
        double[,] v = new double[n,n];


        //Householder's reduction to bidiagonal form
        double f = 0.0;
        double g = 0.0;
        double h = 0.0;
        double x = 0.0;
        double y = 0.0;
        double z = 0.0;
        double s = 0.0;

        for (i = 0; i < n; i++) {
            e[i] = g;
            s = 0.0;
            l = i + 1;
            for (j = i; j < m; j++) {
                s += (u[j,i] * u[j,i]);
            }
            if (s <= tolerance){
                g = 0.0;
            }
            else
            {
                f = u[i, i];
                g = Math.Sqrt(s);
                if (f >= 0.0) {
                    g = -g;
                };
                h = f * g - s;
                u[i,i] = f - g;

                for (j = l; j < n; j++)
                {
                    s = 0.0;

                    for (k = i; k < m; k++) {
                        s += u[k,i] * u[k,j];
                    }

                    f = s / h;

                    for (k = i; k < m; k++){
                        u[k,j] += f * u[k,i];
                    }
                }   
            }
            q[i] = g;
            s = 0.0;
            for (j = l; j < n; j++){
                s = s + u[i,j] * u[i,j];
            }
            if (s <= tolerance) {
                g = 0.0;
            }
            else {
                f = u[i,i + 1];
                g = Math.Sqrt(s);
                if (f >= 0.0) {
                    g = -g;
                }
                h = f * g - s;
                u[i,i + 1] = f - g;
                for (j = l; j < n; j++) {
                    e[j] = u[i,j] / h;
                }
        
                for (j = l; j < m; j++) {
                    s = 0.0;
                    for (k = l; k < n; k++)
                    {
                        s += (u[j,k] * u[i,k]);
                    }
                    for (k = l; k < n; k++){
                        u[j,k] += s * e[k];
                    }
                }
            }

            y = Math.Abs(q[i]) + Math.Abs(e[i]);
            if (y > x) {
                x = y;
            }
        }

        // accumulation of right hand gtransformations
        for (i = n - 1; i != -1; i += -1) {
            if (g != 0.0) {
                h = g * u[i,i + 1];

                for (j = l; j < n; j++) {
                    v[j,i] = u[i,j] / h;
                }
                for (j = l; j < n; j++) {
                    s = 0.0;
                    for (k = l; k < n; k++) { 
                        s += u[i,k] * v[k,j];
                    }
                    for (k = l; k < n; k++) {
                        v[k,j] += (s * v[k,i]);
                    }
                }
            }

            for (j = l; j < n; j++)
            {
                v[i,j] = 0;
                v[j,i] = 0;
            }
            v[i,i] = 1;
            g = e[i];

            l = i;
        
        }

        // accumulation of left hand transformations
        for (i = n - 1; i != -1; i += -1) {
            l = i + 1;
            g = q[i];
        
            for (j = l; j < n; j++) {
                u[i,j] = 0;
            }
            if (g != 0.0)
            {
                h = u[i,i] * g;

                for (j = l; j < n; j++)
                {
                    s = 0.0;
                    for (k = l; k < m; k++) {
                        s += u[k,i] * u[k,j];
                    }
                    f = s / h;
                    for (k = i; k < m; k++) {
                        u[k,j] += f * u[k,i];
                    }
                }

                for (j = i; j < m; j++) {
                    u[j,i] = u[j,i] / g;
                }
            }
            else {
                for (j = i; j < m; j++) {
                    u[j,i] = 0;
                }
            }
            u[i,i] += 1;
        }

        // diagonalization of the bidiagonal form
        prec = prec * x;
 
        for (k = n - 1; k != -1; k += -1) {
            for (var iteration = 0; iteration < itmax; iteration++) {   // test f splitting
                bool test_convergence = false;
                for (l = k; l != -1; l += -1) {
                    if (Math.Abs(e[l]) <= prec) {
                        test_convergence = true;
                        break;
                    }
                    if (Math.Abs(q[l - 1]) <= prec) {
                        break;
                    }
                }
                if (!test_convergence)
                {   // cancellation of e[l] if l>0
                    c = 0.0;
                    s = 1.0;
                    int l1 = l - 1;

                    for (i = l; i < k + 1; i++)
                    {
                        f = s * e[i];
                        e[i] = c * e[i];
                        if (Math.Abs(f) <= prec) {
                            break;
                        }
                        g = q[i];
                        h = svd_pythag(f, g);
                        q[i] = h;
                        c = g / h;
                        s = -f / h;

                        for (j = 0; j < m; j++) {
                            y = u[j,l1];
                            z = u[j,i];
                            u[j,l1] = y * c + (z * s);
                            u[j,i] = -y * s + (z * c);
                        }
                    }
                }
                // test f convergence
                z = q[k];
                if (l == k)
                {   //convergence
                    if (z < 0.0)
                    {   //q[k] is made non-negative
                        q[k] = -z;
                        for (j = 0; j < n; j++) {
                            v[j,k] = -v[j,k];
                        }
                    }
                    break;  //break out of iteration loop and move on to next k value
                }
                if (iteration >= itmax - 1) {
                    throw new Exception("SVDError: No convergence.");
                }
                // shift from bottom 2x2 minor
                x = q[l];
                y = q[k - 1];
                g = e[k - 1];
                h = e[k];
                f = ((y - z) * (y + z) + (g - h) * (g + h)) / (2.0 * h * y);
                g = svd_pythag(f, 1.0);
                if (f < 0.0)
                {
                    f = ((x - z) * (x + z) + h * (y / (f - g) - h)) / x;
                } else {
                    f = ((x - z) * (x + z) + h * (y / (f + g) - h)) / x;
                }
                    
                // next QR transformation
                c = 1.0;
                s = 1.0;
        
                for (i = l + 1; i < k + 1; i++) {
                    g = e[i];
                    y = q[i];
                    h = s * g;
                    g = c * g;
                    z = svd_pythag(f, h);
                    e[i - 1] = z;
                    c = f / z;
                    s = h / z;
                    f = x * c + g * s;
                    g = -x * s + g * c;
                    h = y * s;
                    y = y * c;

                    for (j = 0; j < n; j++) {
                        x = v[j,i - 1];
                        z = v[j,i];
                        v[j,i - 1] = x * c + z * s;
                        v[j,i] = -x * s + z * c;
                    }
                    z = svd_pythag(f, h);
                    q[i - 1] = z;
                    c = f / z;
                    s = h / z;
                    f = c * g + s * y;
                    x = -s * g + c * y;

                    for (j = 0; j < m; j++) {
                        y = u[j,i - 1];
                        z = u[j,i];
                        u[j,i - 1] = y * c + z * s;
                        u[j,i] = -y * s + z * c;
                    }
                }
                e[l] = 0.0;
                e[k] = f;
                q[k] = x;
        
            }
        }

        //vt= transpose(v)
        //return (u,q,vt)
        for (i = 0; i < q.Length; i++) {
            if (q[i] < prec) {
                q[i] = 0;
            }
        }
            
        //sort eigenvalues	
        for (i = 0; i < n; i++) {
            //writeln(q)
            for (j = i - 1; j >= 0; j--) {
                if (q[j] < q[i]) {
                    //  writeln(i,'-',j)
                    c = q[j];
                    q[j] = q[i];
                    q[i] = c;
              
                    for (k = 0; k < u.GetLength(0); k++) {
                        temp = u[k,i];
                        u[k,i] = u[k,j];
                        u[k,j] = temp;
                    }
                    for (k = 0; k < v.GetLength(0); k++) {
                        temp = v[k,i];
                        v[k,i] = v[k,j];
                        v[k,j] = temp;
                    }
                   //	   u.swapCols(i,j)
                   //	   v.swapCols(i,j)
                   i = j;
                }       
            }
        }

        return v;
   
    }

	/* Triangulates a regular mesh with counter-clockwise faces (forward facing) */
	public static List<Vector3> Triangulate(int width, int height) {

		List<Vector3> triangles = new List<Vector3> ();

		for (int i = 0; i < height - 1; i++) {
			for (int j = 0; j < width - 1; j++) {
				// Makes this kind of triangle:
				//   .....
				//   .  /
				//   ./
				triangles.Add (new Vector3 ((i + 1) * width + j, i * width + j + 1, i * width + j));
				// Makes this kind of triangle:
				//      /
				//    / |
				//  /....
				triangles.Add (new Vector3 ((i + 1) * width + j, (i + 1) * width + j + 1, i * width + j + 1));
			}
		}

		return triangles;
	}
}
