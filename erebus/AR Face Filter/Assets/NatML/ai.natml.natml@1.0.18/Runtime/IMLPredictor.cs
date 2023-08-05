/* 
*   NatML
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/
namespace NatML
{
    public struct Bbox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public Bbox(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
namespace NatML {

    using System;

    /// <summary>
    /// Lightweight primitive for making predictions with a model.
    /// Predictors transform raw model outputs to types that are easily usable by applications.
    /// </summary>
    public interface IMLPredictor<TOutput> : IDisposable {

        /// <summary>
        /// Make a prediction on one or more input features.
        /// </summary>
        /// <param name="inputs">Input features.</param>
        /// <returns>Prediction output.</returns>
        TOutput Predict (params MLFeature[] inputs);
    }
}

namespace NatML
{

    using System;

    /// <summary>
    /// Lightweight primitive for making predictions with a model.
    /// Predictors transform raw model outputs to types that are easily usable by applications.
    /// </summary>
    public interface IMLPredictor2<TOutput> : IDisposable
    {
        /// <summary>
        /// Make a prediction on one or more input features.
        /// </summary>
        /// <param name="inputs">Input features.</param>
        /// <returns>Prediction output.</returns>
        (TOutput, string[]) Predict2(params MLFeature[] inputs);

        ///// <summary>
        ///// Make a prediction on one or more input features.
        ///// </summary>
        ///// <param name="inputs">Input features.</param>
        ///// <returns>Prediction output.</returns>
        //Bbox[] Predict2(params MLFeature[] inputs);
    }
}