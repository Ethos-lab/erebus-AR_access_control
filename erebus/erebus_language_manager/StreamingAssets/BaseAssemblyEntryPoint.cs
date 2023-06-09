using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using UnityEngine;
using System.Linq;
using DetectionResult;

namespace Erebus
{
    namespace AccessControl
    {
        //Entry point to the main code assembly
        public class BaseAssemblyEntryPoint
        {
            private Assembly baseAssembly = null;
            private Type baseAssemblyProgram = null;
            private object baseAssemblyInstance = null;
            public BaseAssemblyEntryPoint() { }
            public BaseAssemblyEntryPoint(Assembly baseAssembly, string baseAssemblyName)
            {
                this.baseAssembly = baseAssembly;
                baseAssemblyProgram = baseAssembly.GetType(baseAssemblyName);//"Erebus.LanguageManager.ErebusRemoteAssembly";
                var property = baseAssemblyProgram.GetProperty("Instance");
                this.baseAssemblyInstance = property.GetValue(null);
                //Debug.Log($"Base assembly : {baseAssembly}");
            }
            public string GetAppName()
            {
                var function = baseAssemblyProgram.GetMethod("GetAppName");
                var returnVal = function.Invoke(baseAssemblyInstance, null);
                return (string)returnVal;
            }
            public DateTimeWrapper GetCurrentTime()
            {
                var function = baseAssemblyProgram.GetMethod("GetCurrentTime");
                var returnVal = function.Invoke(baseAssemblyInstance, new object[] { false });
                return new DateTimeWrapper((DateTime)returnVal);
            }
            public Vector2 GetCurrentLocation()
            {
                var function = baseAssemblyProgram.GetMethod("GetCurrentLocation");
                var returnVal = function.Invoke(baseAssemblyInstance, null);
                return (Vector2)returnVal;
            }
            public object GetCurrentFaceId()
            {
                var function = baseAssemblyProgram.GetMethod("GetCurrentFaceId");
                var returnVal = function.Invoke(baseAssemblyInstance, null);
                return returnVal;
            }

            public List<string> GetTrustedAppNames()
            {
                var function = baseAssemblyProgram.GetMethod("GetTrustedAppNames");
                var returnVal = function.Invoke(baseAssemblyInstance, null);
                return (List<string>)returnVal;
            }
            public string GetValidHour(string searchTag)
            {
                var function = baseAssemblyProgram.GetMethod("GetValidHour");
                var returnVal = function.Invoke(baseAssemblyInstance, new object[] { searchTag });
                return (string)returnVal;
            }
            public string GetTrustedLocation(string searchTag)
            {
                var function = baseAssemblyProgram.GetMethod("GetTrustedLocation");
                var returnVal = function.Invoke(baseAssemblyInstance, new object[] { searchTag });
                return (string)returnVal;
            }
            public string GetTrustedFaceIds()
            {
                var function = baseAssemblyProgram.GetMethod("GetTrustedFaceIds");
                var returnVal = function.Invoke(baseAssemblyInstance, null);
                return (string)returnVal;
            }
            public IDetectionResult GetCurrentCameraFrame()
            {
                var function = baseAssemblyProgram.GetMethod("GetCurrentCameraFrame");
                var returnVal = function.Invoke(baseAssemblyInstance, null);
                return (IDetectionResult)returnVal;
            }
        }

        //Without this, it cannot compare
        //string formatted time vs DateTime formatted time
        //(TO-DO)
        //I would like to reformat the Date to : "HH:mm",
        //but " symbol and : symbol are not recognized in the transpiler (only numbers work)
        public struct DateTimeWrapper
        {
            private readonly DateTime actualDateTime;
            public DateTimeWrapper(DateTime dateTime)
            {
                this.actualDateTime = dateTime;
            }
            public static implicit operator DateTime(DateTimeWrapper t) => t.actualDateTime;
            public static explicit operator DateTimeWrapper(DateTime t) => new DateTimeWrapper(t);
            public override string ToString() => $"{actualDateTime}";

            //If the input is integer. It needs to be padded with preceding zeros
            public static bool operator >(int date1, DateTimeWrapper date2)
            {
                var date1Str = date1.ToString("0000");
                (var fDate1, var fDate2) = MatchFormat(date1Str, date2);
                return DateTime.Compare(fDate1, fDate2) > 0;
            }
            public static bool operator <(int date1, DateTimeWrapper date2)
            {
                var date1Str = date1.ToString("0000");
                (var fDate1, var fDate2) = MatchFormat(date1Str, date2);
                return DateTime.Compare(fDate1, fDate2) < 0;
            }
            public static bool operator >=(int date1, DateTimeWrapper date2)
            {
                var date1Str = date1.ToString("0000");
                (var fDate1, var fDate2) = MatchFormat(date1Str, date2);
                return DateTime.Compare(fDate1, fDate2) >= 0;
            }
            public static bool operator <=(int date1, DateTimeWrapper date2)
            {
                var date1Str = date1.ToString("0000");
                (var fDate1, var fDate2) = MatchFormat(date1Str, date2);
                return DateTime.Compare(fDate1, fDate2) <= 0;
            }
            public static bool operator >(DateTimeWrapper date1, int date2)
            {
                var date2Str = date2.ToString("0000");
                (var fDate2, var fDate1) = MatchFormat(date2Str, date1);
                return DateTime.Compare(fDate2, fDate1) < 0;
            }
            public static bool operator <(DateTimeWrapper date1, int date2)
            {
                var date2Str = date2.ToString("0000");
                (var fDate2, var fDate1) = MatchFormat(date2Str, date1);
                return DateTime.Compare(fDate2, fDate1) > 0;
            }
            public static bool operator >=(DateTimeWrapper date1, int date2)
            {
                var date2Str = date2.ToString("0000");
                (var fDate2, var fDate1) = MatchFormat(date2Str, date1);
                return DateTime.Compare(fDate2, fDate1) <= 0;
            }
            public static bool operator <=(DateTimeWrapper date1, int date2)
            {
                var date2Str = date2.ToString("0000");
                (var fDate2, var fDate1) = MatchFormat(date2Str, date1);
                return DateTime.Compare(fDate2, fDate1) >= 0;
            }

            //Normal string input
            public static bool operator >(string date1, DateTimeWrapper date2)
            {
                (var fDate1, var fDate2) = MatchFormat(date1, date2);
                return DateTime.Compare(fDate1, fDate2) > 0;
            }
            public static bool operator <(string date1, DateTimeWrapper date2)
            {
                (var fDate1, var fDate2) = MatchFormat(date1, date2);
                return DateTime.Compare(fDate1, fDate2) < 0;
            }
            public static bool operator >=(string date1, DateTimeWrapper date2)
            {
                (var fDate1, var fDate2) = MatchFormat(date1, date2);
                return DateTime.Compare(fDate1, fDate2) >= 0;
            }
            public static bool operator <=(string date1, DateTimeWrapper date2)
            {
                (var fDate1, var fDate2) = MatchFormat(date1, date2);
                return DateTime.Compare(fDate1, fDate2) <= 0;
            }
            public static bool operator >(DateTimeWrapper date1, string date2)
            {
                (var fDate2, var fDate1) = MatchFormat(date2, date1);
                return DateTime.Compare(fDate2, fDate1) < 0;
            }
            public static bool operator <(DateTimeWrapper date1, string date2)
            {
                (var fDate2, var fDate1) = MatchFormat(date2, date1);
                return DateTime.Compare(fDate2, fDate1) > 0;
            }
            public static bool operator >=(DateTimeWrapper date1, string date2)
            {
                (var fDate2, var fDate1) = MatchFormat(date2, date1);
                return DateTime.Compare(fDate2, fDate1) <= 0;
            }
            public static bool operator <=(DateTimeWrapper date1, string date2)
            {
                (var fDate2, var fDate1) = MatchFormat(date2, date1);
                return DateTime.Compare(fDate2, fDate1) >= 0;
            }
            private static (DateTime, DateTime) MatchFormat(string date1, DateTimeWrapper date2)
            {
                var rawDateTime1 = string.Concat(date1.Split(':'));
                var formattedDateTime1 = DateTime.ParseExact(rawDateTime1, "HHmm", null);
                var formattedDateTime2 = date2;
                return (formattedDateTime1, formattedDateTime2);
            }
        }
        public static class FunctionExtension
        {
            public static bool Contains(this IDetectionResult detectionResult, string targetLabel)
            {
                if (detectionResult == null || detectionResult.Data == null || detectionResult.Data.Count <= 0)
                    return false;

                //Remove everything but the target label
                //Removing so that it updates the AR Manager's detection res data (Class type -> Linked)
                //Otherwise, no way to pass the value of targetLabel to the AR Manager
                detectionResult.Data.RemoveAll(item => item.Label.ToLower() != targetLabel.ToLower());

                return detectionResult.Data.Count > 0;
            }
            public static bool IncludedIn(this object inputFace, string emptyData)
            {
                //Because this won't even be executed if the app did not pass the Biometric check
                //At the start of the app
                return true;
            }
            public static bool Within(this DateTimeWrapper inputTime, string cmpTimeStr)
            {
                if (cmpTimeStr == null)
                    return false;
                var inputDateTime = (DateTime)inputTime;
                var cmpTimes = cmpTimeStr.Split(',');
                var startTime = DateTime.Parse(cmpTimes[0]);
                var endTime = DateTime.Parse(cmpTimes[1]);

                return (inputDateTime >= startTime) && (inputDateTime <= endTime);
            }
            public static bool Within(this Vector2 inputLocation, string cmpLocationStr)
            {
                if (cmpLocationStr == null)
                    return false;
                var cmpTimes = cmpLocationStr.Split(',');
                var rawLat = float.Parse(cmpTimes[0]);
                var rawLon = float.Parse(cmpTimes[1]);

                var cmpLocation = new Vector2(rawLat, rawLon);

                //GoogleMap/Geofence uses Haversine formula to calculate the dist btw two lat/long points
                double distance = CalcHaversineDistance(cmpLocation, inputLocation);
                //Only allows within the GPS radius (Default 1Km)
                //Debug.Log($"[GPS] {distance}");

                return (distance <= LocRadiusBound);
            }
            public static bool Contains(this object inputFace, string emptyData)
            {
                //Because this won't even be executed if the app did not pass the Biometric check
                //At the start of the app
                return true;
            }
            public static bool Contains(this DateTime inputTime, string cmpTimeStr)
            {
                if (cmpTimeStr == null)
                    return false;
                var inputDateTime = (DateTime)inputTime;
                var cmpTimes = cmpTimeStr.Split(',');
                var startTime = DateTime.Parse(cmpTimes[0]);
                var endTime = DateTime.Parse(cmpTimes[1]);

                return (inputDateTime >= startTime) && (inputDateTime <= endTime);
            }
            public static bool Contains(this Vector2 inputLocation, string cmpLocationStr)
            {
                if (cmpLocationStr == null)
                    return false;
                var cmpTimes = cmpLocationStr.Split(',');
                var rawLat = float.Parse(cmpTimes[0]);
                var rawLon = float.Parse(cmpTimes[1]);

                var cmpLocation = new Vector2(rawLat, rawLon);

                //GoogleMap/Geofence uses Haversine formula to calculate the dist btw two lat/long points
                double distance = CalcHaversineDistance(cmpLocation, inputLocation);
                //Only allows within the GPS radius (Default 1Km)
                //Debug.Log($"[GPS] {distance}");

                return (distance <= LocRadiusBound);
            }
            private const double LocRadiusBound = 1;//in Kilometer
            private const int R = 6371; // km (Earth radius)
                                        //Reference : https://stackoverflow.com/questions/17787235/creating-a-method-using-haversine-formula-android-v2
            private static double CalcHaversineDistance(Vector2 latLong1, Vector2 latLong2)
            {
                double lat1 = latLong1.x;
                double long1 = latLong1.y;
                double lat2 = latLong2.x;
                double long2 = latLong2.y;

                double dLat = ToRadian(lat2 - lat1);
                double dLon = ToRadian(long2 - long1);
                lat1 = ToRadian(lat1);
                lat2 = ToRadian(lat2);

                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return R * c;
            }
            private static double ToRadian(double deg)
            {
                return deg * (Math.PI / 180);
            }
        }
    }
}

