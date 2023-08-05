using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace SortCS.Kalman
{
    internal class KalmanBoxTracker
    {
        private KalmanFilter _filter;

        public KalmanBoxTracker(RectangleF box)
        {
            _filter = new KalmanFilter(7, 4)
            {
                StateTransitionMatrix = new Matrix(
                    new double[,]
                    {
                        { 1, 0, 0, 0, 1, 0, 0 },
                        { 0, 1, 0, 0, 0, 1, 0 },
                        { 0, 0, 1, 0, 0, 0, 1 },
                        { 0, 0, 0, 1, 0, 0, 0 },
                        { 0, 0, 0, 0, 1, 0, 0 },
                        { 0, 0, 0, 0, 0, 1, 0 },
                        { 0, 0, 0, 0, 0, 0, 1 }
                    }),
                MeasurementFunction = new Matrix(
                    new double[,]
                    {
                        { 1, 0, 0, 0, 0, 0, 0 },
                        { 0, 1, 0, 0, 0, 0, 0 },
                        { 0, 0, 1, 0, 0, 0, 0 },
                        { 0, 0, 0, 1, 0, 0, 0 }
                    }),
                UncertaintyCovariances = new Matrix(
                    new double[,]
                    {
                        { 10, 0, 0, 0, 0, 0, 0 },
                        { 0, 10, 0, 0, 0, 0, 0 },
                        { 0, 0, 10, 0, 0, 0, 0 },
                        { 0, 0, 0, 10, 0, 0, 0 },
                        { 0, 0, 0, 0, 10000, 0, 0 },
                        { 0, 0, 0, 0, 0, 10000, 0 },
                        { 0, 0, 0, 0, 0, 0, 10000 }
                    }),
                MeasurementUncertainty = new Matrix(new double[,]
                {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 0, 10, 0 },
                    { 0, 0, 0, 10 },
                }),
                ProcessUncertainty = new Matrix(
                    new double[,]
                    {
                        { 1, 0, 0, 0, 0, 0, 0 },
                        { 0, 1, 0, 0, 0, 0, 0 },
                        { 0, 0, 1, 0, 0, 0, 0 },
                        { 0, 0, 0, 1, 0, 0, 0 },
                        { 0, 0, 0, 0, .01, 0, 0 },
                        { 0, 0, 0, 0, 0, .01, 0 },
                        { 0, 0, 0, 0, 0, 0, .0001 }
                    }),
                CurrentState = ToMeasurement(box).Append(0, 0, 0)
            };
            _filter.stateTransitionMatrixTransposed = _filter.StateTransitionMatrix.Transposed;
            _filter.measurementFunctionTransposed = _filter.MeasurementFunction.Transposed;

            //for (int i = 0; i < _filter.MeasurementFunction.Rows; i++)
            //{
            //    string str = "";
            //    for (int j = 0; j < _filter.MeasurementFunction.Columns; j++)
            //    {
            //        var val = _filter.MeasurementFunction._values[i, j];
            //        str += $"{val},";
            //    }
            //    Debug.Log(str);
            //}


            //for (int i = 0; i < _filter.measurementFunctionTransposed.Rows; i++)
            //{
            //    string str = "";
            //    for (int j = 0; j < _filter.measurementFunctionTransposed.Columns; j++)
            //    {
            //        var val = _filter.measurementFunctionTransposed._values[i, j];
            //        str += $"{val},";
            //    }
            //    Debug.Log("[T]" + str);
            //}
        }

        public void Update(RectangleF box)
        {
            _filter.Update(ToMeasurement(box));
        }

        public RectangleF Predict()
        {
            var curstate = _filter.CurrentState;

            if (curstate[6] + curstate[2] <= 0)
            {
                curstate._values[6] = 0;
                //state[6] = 0;
                //_filter.CurrentState = _filter.CurrentState._values[6] = 0;
            }

            _filter.Predict();

            var prediction = ToBoundingBox(curstate);

            return prediction;
        }

        private static Vector ToMeasurement(RectangleF box)
        {
            var center = new PointF(box.Left + (box.Width / 2f), box.Top + (box.Height / 2f));
            return new Vector(center.X, center.Y, box.Width * (double)box.Height, box.Width / (double)box.Height);
        }

        private static RectangleF ToBoundingBox(Vector currentState)
        {
            var w = Math.Sqrt(currentState[2] * currentState[3]);
            var h = currentState[2] / w;

            return new RectangleF(
                (float)(currentState[0] - (w / 2)),
                (float)(currentState[1] - (h / 2)),
                (float)w,
                (float)h);
        }
    }
}