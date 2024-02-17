using System;
using System.Collections.Generic;

namespace DisEn
{
    [Serializable]
    public class NeuralNetwork
    {
        private List<Matrix> weightsLayers;
        private List<Matrix> biasLayers;
        private List<int> nodesPerLayer;
        private double learningRate;

        public NeuralNetwork(List<int> nodesPerLayer, double learningRate, double startWeightFromRange, double startWeightToRange)
        {
            this.nodesPerLayer = nodesPerLayer;
            this.learningRate = learningRate;

            InitializeWeightsAndBiases(startWeightFromRange, startWeightToRange);
        }

        private void InitializeWeightsAndBiases(double startWeightFromRange, double startWeightToRange)
        {
            weightsLayers = new List<Matrix>();
            biasLayers = new List<Matrix>();

            for (int i = 1; i < nodesPerLayer.Count; i++)
            {
                Matrix weights = new Matrix(nodesPerLayer[i], nodesPerLayer[i - 1]);
                weights.Randomize(startWeightFromRange, startWeightToRange);
                weightsLayers.Add(weights);

                Matrix bias = new Matrix(nodesPerLayer[i], 1);
                bias.Randomize(startWeightFromRange, startWeightToRange);
                biasLayers.Add(bias);
            }
        }

        public void Train(List<double> inputArray, List<double> targetArray)
        {
            List<Matrix> layerOutputs = new List<Matrix>();
            Matrix inputs = Matrix.FromArray(inputArray);
            layerOutputs.Add(inputs);

            // Forward propagation
            for (int i = 0; i < weightsLayers.Count; i++)
            {
                Matrix hidden = Matrix.Multiply(weightsLayers[i], layerOutputs[i]);
                hidden = Matrix.Add(hidden, biasLayers[i]);
                hidden.Map(Sigmoid); // Activation
                layerOutputs.Add(hidden);
            }

            // Backpropagation
            Matrix errors = Matrix.Subtract(Matrix.FromArray(targetArray), layerOutputs[layerOutputs.Count - 1]);
            for (int i = weightsLayers.Count - 1; i >= 0; i--)
            {
                Matrix gradients = Matrix.Map(layerOutputs[i + 1], DSigmoid); // Activation derivative
                gradients = Matrix.ElementWiseMultiplication(errors, gradients);
                gradients = Matrix.Multiply(gradients, learningRate);

                Matrix inputs_T = Matrix.Transpose(layerOutputs[i]);
                Matrix weights_deltas = Matrix.Multiply(gradients, inputs_T);

                weightsLayers[i] = Matrix.Add(weightsLayers[i], weights_deltas);
                biasLayers[i] = Matrix.Add(biasLayers[i], gradients);

                Matrix weights_T = Matrix.Transpose(weightsLayers[i]);
                errors = Matrix.Multiply(weights_T, errors);
            }
        }

        public List<double> Predict(List<double> inputArray)
        {
            Matrix inputs = Matrix.FromArray(inputArray);
            Matrix layerOutput = inputs;

            for (int i = 0; i < weightsLayers.Count; i++)
            {
                layerOutput = Matrix.Multiply(weightsLayers[i], layerOutput);
                layerOutput = Matrix.Add(layerOutput, biasLayers[i]);
                layerOutput.Map(Sigmoid); // Activation
            }

            return layerOutput.ToArray();
        }

        public void SetLearningRate(double newLearningRate)
        {
            learningRate = newLearningRate;
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        private double DSigmoid(double y)
        {
            return y * (1.0 - y);
        }
    }
}
