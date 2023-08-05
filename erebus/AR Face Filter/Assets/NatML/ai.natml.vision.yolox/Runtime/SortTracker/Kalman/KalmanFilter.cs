using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SortCS.Kalman
{
    internal class KalmanFilter
    {
        private readonly int _stateSize;
        private readonly int _measurementSize;
        private readonly Matrix _identity;
        private readonly Matrix _processUncertainty;
        private readonly Matrix _stateTransitionMatrix;
        public Matrix stateTransitionMatrixTransposed;
        private readonly Matrix _measurementFunction;
        public Matrix measurementFunctionTransposed;
        private readonly Matrix _measurementUncertainty;
        private readonly double _alphaSq;

        private Vector _currentState;
        private Matrix _uncertaintyCovariances;

        public KalmanFilter(int stateSize, int measurementSize)
        {
            _stateSize = stateSize;
            _measurementSize = measurementSize;
            _identity = Matrix.Identity(stateSize);
            _alphaSq = 1.0d;

            StateTransitionMatrix = _identity; // F
            MeasurementFunction = new Matrix(_measurementSize, _stateSize); //  H
            UncertaintyCovariances = Matrix.Identity(_stateSize); // P
            MeasurementUncertainty = Matrix.Identity(_measurementSize); // R
            ProcessUncertainty = _identity; // Q
            CurrentState = new Vector(stateSize);
        }

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public Vector CurrentState
        {
            get => _currentState;
            set => _currentState = value.Length == _stateSize
                ? value
                : throw new ArgumentException($"Vector must be of size {_stateSize}.", nameof(value));
        }

        /// <summary>
        /// Gets the uncertainty covariances.
        /// </summary>
        public Matrix UncertaintyCovariances
        {
            get => _uncertaintyCovariances;
            init => _uncertaintyCovariances = value.Rows == _stateSize && value.Columns == _stateSize
                ? value
                : throw new ArgumentException($"Matrix must be of size {_stateSize}x{_stateSize}.", nameof(value));
        }

        /// <summary>
        /// Gets the process uncertainty.
        /// </summary>
        public Matrix ProcessUncertainty
        {
            get => _processUncertainty;
            init => _processUncertainty = value.Rows == _stateSize && value.Columns == _stateSize
                ? value
                : throw new ArgumentException($"Matrix must be of size {_stateSize}x{_stateSize}.", nameof(value));
        }

        public Matrix MeasurementUncertainty
        {
            get => _measurementUncertainty;
            init => _measurementUncertainty = value.Rows == _measurementSize && value.Columns == _measurementSize
                ? value
                : throw new ArgumentException($"Matrix must be of size {_measurementSize}x{_measurementSize}.", nameof(value));
        }

        /// <summary>
        /// Gets the state transition matrix.
        /// </summary>
        public Matrix StateTransitionMatrix
        {
            get => _stateTransitionMatrix;
            init => _stateTransitionMatrix = value.Rows == _stateSize && value.Columns == _stateSize
                ? value
                : throw new ArgumentException($"Matrix must be of size {_stateSize}x{_stateSize}.", nameof(value));
        }

        /// <summary>
        /// Gets the measurement function.
        /// </summary>
        public Matrix MeasurementFunction
        {
            get => _measurementFunction;
            init => _measurementFunction = value.Rows == _measurementSize && value.Columns == _stateSize
                ? value
                : throw new ArgumentException($"Matrix must be of size {_measurementSize}x{_stateSize}.", nameof(value));
        }

        public void Predict()
        {
            //Manually performing dot product
            var elem1 = _currentState[0] + _currentState[4];
            var elem2 = _currentState[1] + _currentState[5];
            var elem3 = _currentState[2] + _currentState[6];
            var elem4 = _currentState[3];
            var elem5 = _currentState[4];
            var elem6 = _currentState[5];
            var elem7 = _currentState[6];
            _currentState[0] = elem1;
            _currentState[1] = elem2;
            _currentState[2] = elem3;
            _currentState[3] = elem4;
            _currentState[4] = elem5;
            _currentState[5] = elem6;
            _currentState[6] = elem7;

            var uncertainty_arr = _uncertaintyCovariances._values;
            var intermed_arr = new double[7, 7]; //7x7 array initialized with zeros
            double val1, val2, val3, val4, val5, val6, val7;
            for (int i = 0; i < 7; i++)
            {
                if (i < 3)
                {
                    val1 = uncertainty_arr[4 + i, 0];
                    val2 = uncertainty_arr[4 + i, 1];
                    val3 = uncertainty_arr[4 + i, 2];
                    val4 = uncertainty_arr[4 + i, 3];
                    val5 = uncertainty_arr[4 + i, 4];
                    val6 = uncertainty_arr[4 + i, 5];
                    val7 = uncertainty_arr[4 + i, 6];
                }
                else
                {
                    val1 = 0;
                    val2 = 0;
                    val3 = 0;
                    val4 = 0;
                    val5 = 0;
                    val6 = 0;
                    val7 = 0;
                }
                intermed_arr[i, 0] = uncertainty_arr[i, 0] + val1;
                intermed_arr[i, 1] = uncertainty_arr[i, 1] + val2;
                intermed_arr[i, 2] = uncertainty_arr[i, 2] + val3;
                intermed_arr[i, 3] = uncertainty_arr[i, 3] + val4;
                intermed_arr[i, 4] = uncertainty_arr[i, 4] + val5;
                intermed_arr[i, 5] = uncertainty_arr[i, 5] + val6;
                intermed_arr[i, 6] = uncertainty_arr[i, 6] + val7;
            }

            var proc_uncertainty_arr = _processUncertainty._values;
            var final_arr = new double[7, 7]; //7x7 array initialized with zeros
            for (int i = 0; i < 7; i++)
            {
                final_arr[i, 0] = intermed_arr[i, 0] + intermed_arr[i, 4];
                final_arr[i, 1] = intermed_arr[i, 1] + intermed_arr[i, 5];
                final_arr[i, 2] = intermed_arr[i, 2] + intermed_arr[i, 6];
                final_arr[i, 3] = intermed_arr[i, 3];
                final_arr[i, 4] = intermed_arr[i, 4];
                final_arr[i, 5] = intermed_arr[i, 5];
                final_arr[i, 6] = intermed_arr[i, 6];

                final_arr[i, i] += proc_uncertainty_arr[i, i];
            }
            _uncertaintyCovariances = new Matrix(final_arr);

            //_currentState = _stateTransitionMatrix.Dot(_currentState);
            //_uncertaintyCovariances = (_stateTransitionMatrix * UncertaintyCovariances * stateTransitionMatrixTransposed) + _processUncertainty;
        }
        private Vector tmp_y = new Vector(new double[4] { 0, 0, 0, 0 });
        private double[,] pht_arr = new double[7, 4];//7x4 array initialized with zeros
        private double[,] intermed_res_arr = new double[7, 4]; //7x4 array initialized with zeros

        public void Update(Vector measurement)
        {
            //Manually performing dot product
            var measurement_val = measurement._values;
            var elem1 = measurement_val[0] - _currentState[0];
            var elem2 = measurement_val[1] - _currentState[1];
            var elem3 = measurement_val[2] - _currentState[2];
            var elem4 = measurement_val[3] - _currentState[3];
            tmp_y[0] = elem1;
            tmp_y[1] = elem2;
            tmp_y[2] = elem3;
            tmp_y[3] = elem4;

            var uncertainty_arr = _uncertaintyCovariances._values;
            for (int i = 0; i < 7; i++)
            {
                pht_arr[i, 0] = uncertainty_arr[i, 0];
                pht_arr[i, 1] = uncertainty_arr[i, 1];
                pht_arr[i, 2] = uncertainty_arr[i, 2];
                pht_arr[i, 3] = uncertainty_arr[i, 3];
            }
            var pht = new Matrix(pht_arr);

            var s_arr = new double[4, 4]; //4x4 array initialized with zeros
            var measure_uncertainty_arr = _measurementUncertainty._values;
            for (int i = 0; i < 4; i++)
            {
                s_arr[i, 0] = pht_arr[i, 0];
                s_arr[i, 1] = pht_arr[i, 1];
                s_arr[i, 2] = pht_arr[i, 2];
                s_arr[i, 3] = pht_arr[i, 3];

                s_arr[i, i] += measure_uncertainty_arr[i, i];
            }
            var S = new Matrix(s_arr);

            //var y = measurement - _measurementFunction.Dot(_currentState); //(4x7) x (7x1)
            //var pht = UncertaintyCovariances * measurementFunctionTransposed; //(7x7) x (7x4) = (7x4)
            //var S = (_measurementFunction * pht) + _measurementUncertainty; //(4x7) x (7x4) = (4x4)
            var SI = S.Inverted;
            var K = pht * SI; //(7x4)x(4x4) = (7x4)
            //_currentState += K.Dot(tmp_y); //(7x4) dot (4x1) = (7x1)
            _currentState = K.CurState_plus_KDot(_currentState, tmp_y); //(7x4) dot (4x1) = (7x1)

            //var I_KH = _identity - (K * _measurementFunction); // (7x4)x(4x7) = (7x7)

            var K_arr = K._values;
            var I_KH_arr = new double[7, 7]; //7x7 array initialized with zeros
            for (int i = 0; i < 7; i++)
            {
                I_KH_arr[i, 0] = -1 * K_arr[i, 0];
                I_KH_arr[i, 1] = -1 * K_arr[i, 1];
                I_KH_arr[i, 2] = -1 * K_arr[i, 2];
                I_KH_arr[i, 3] = -1 * K_arr[i, 3];

                I_KH_arr[i, i] += 1;
            }
            var I_KH_matrix = new Matrix(I_KH_arr);

            for (int i = 0; i < 7; i++)
            {
                intermed_res_arr[i, 0] = K_arr[i, 0];
                intermed_res_arr[i, 1] = K_arr[i, 1];
                intermed_res_arr[i, 2] = 10 * K_arr[i, 2];
                intermed_res_arr[i, 3] = 10 * K_arr[i, 3];
            }
            var intermed_res_matrix = new Matrix(intermed_res_arr);

            //(7x7)x(7x7)x(7x7) + (7x4)*(4x4)*(4x7) = (7x7)
            _uncertaintyCovariances = (I_KH_matrix * UncertaintyCovariances * I_KH_matrix.Transposed) + (intermed_res_matrix * K.Transposed);

            //_uncertaintyCovariances = (I_KH * UncertaintyCovariances * I_KH.Transposed) + (K * _measurementUncertainty * K.Transposed);
        }
    }
}