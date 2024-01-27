using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    public class NeuralNetwork
    {
        private Matrix weightsIH;
        private Matrix weightsHO;
        private Matrix biasIH;
        private Matrix biasHO;

        private int inputNodesNumber;
        private int hiddenNodesNumber;
        private int outputNodesNumber;
        private float learningRate;

        public NeuralNetwork(int inputNodesNumber, int hiddenNodesNumber, int outputNodesNumber, float learningRate, float startWeightFromRange, float startWeightToRange)
        {
            this.inputNodesNumber = inputNodesNumber;
            this.hiddenNodesNumber = hiddenNodesNumber;
            this.outputNodesNumber = outputNodesNumber;
            this.learningRate = learningRate;

            // Create matrix of weights between input and hidden layer
            weightsIH = new Matrix(hiddenNodesNumber, inputNodesNumber);
            // Create matrix of weights between hidden and output layer
            weightsHO = new Matrix(outputNodesNumber, hiddenNodesNumber);
            // Randomize weights
            weightsIH.Randomize(startWeightFromRange, startWeightToRange);
            weightsHO.Randomize(startWeightFromRange, startWeightToRange);

            // Create matrix of bias weights between input and hidden layer
            biasIH = new Matrix(hiddenNodesNumber, 1);
            // Create matrix of bias weights between hidden and output layer
            biasHO = new Matrix(outputNodesNumber, 1);

            // Randomize biases
            biasIH.Randomize(startWeightFromRange, startWeightToRange);
            biasHO.Randomize(startWeightFromRange, startWeightToRange);
        }

        public void Train(List<float> inputArray, List<float> targetArray)
        {
            // Guess the value

            // Get the outputs

            // Fill input matrix
            Matrix inputs = Matrix.FromArray(inputArray);
            // Multiplying matrices of weights and layer = W * I
            Matrix hiddens = Matrix.Multiply(weightsIH, inputs);
            // Adding biases = W * I + B
            hiddens = Matrix.Add(hiddens, biasIH);
            // Apply activation function = Sigmoid(W * I + B)
            hiddens.Map(Sigmoid);

            // Generating output's output

            // Multiplying matrices of weights and layer = W * I
            Matrix outputs = Matrix.Multiply(weightsHO, hiddens);
            // Adding biases = W * I + B
            outputs = Matrix.Add(outputs, biasHO);
            // Apply activation function = Sigmoid(W * I + B)
            outputs.Map(Sigmoid);

            // Analyze result and train neural network

            // Prepare matrix representations of arrays
            Matrix matrixTargets = Matrix.FromArray(targetArray);

            // Calculate the error, adjust weights and biases

            // Example of formula is given below
            // Weight formula = learningRate * Error o (Sigmoid(LayerResultElement) * (1 - Sigmoid(LayerResultElement)) * PreviousLayerInput 
            // Bias formula = learningRate * Error o PreviousLayerInput

            // o - hadamard multiplication (element wise)
            // * - matrix multiplication

            // Those formulas are based on gradient descent
            // y = m * x + b
            // delta m = lr * error * x  => Weight formula
            // delta b = lr * error      => Bias formula
            // , where
            // error = Error * (Sigmoid(LayerResult) * (1 - Sigmoid(LayerResult))
            // x = PreviousLayerInput
            // lr = our given value

            // Calculate hidden - output layer

            // OutputErrors = TargetArray - Outputs
            Matrix outputErrors = Matrix.Subtract(matrixTargets, outputs);

            // Calculate gradient

            // Outputs - (1 - Outputs)
            Matrix outputGradients = Matrix.Map(outputs, DSigmoid);

            // OutputErrors o Outputs
            outputGradients = Matrix.ElementWiseMultiplication(outputErrors, outputGradients);

            // LearningRate * Outputs
            outputGradients = Matrix.Multiply(outputGradients, learningRate);

            // Calculate deltas

            // Get transposed hidden layer results
            Matrix hiddens_T = Matrix.Transpose(hiddens);

            // Calculate new values for delta by multiplying gradient with hidden
            Matrix weights_HO_Deltas = Matrix.Multiply(outputGradients, hiddens_T);

            // Adjust output weights
            weightsHO = Matrix.Add(weightsHO, weights_HO_Deltas);

            // Adjust biases
            biasHO = Matrix.Add(biasHO, outputGradients);

            // Calculate input - hidden layer

            // In order to get hidden layer error, we need to get transposed matrix of weights
            Matrix weightsHO_T = Matrix.Transpose(weightsHO);

            // HiddenErrors = WeightsHO_T * OutputErrors
            Matrix hiddenErrors = Matrix.Multiply(weightsHO_T, outputErrors);

            // Calculate gradient

            // Outputs - (1 - Outputs)
            Matrix hiddenGradients = Matrix.Map(hiddens, DSigmoid);

            // OutputErrors * Outputs
            hiddenGradients = Matrix.ElementWiseMultiplication(hiddenErrors, hiddenGradients);

            // LearningRate * Outputs
            hiddenGradients = Matrix.Multiply(hiddenGradients, learningRate);

            // Calculate deltas

            // Get transposed hidden layer results
            Matrix inputs_T = Matrix.Transpose(inputs);

            // Calculate new values for delta by multiplying gradient with hidden
            Matrix weights_IH_Deltas = Matrix.Multiply(hiddenGradients, inputs_T);

            // Adjust output weights
            weightsIH = Matrix.Add(weightsIH, weights_IH_Deltas);

            // Adjust biases
            biasIH = Matrix.Add(biasIH, hiddenGradients);
        }

        public List<float> Predict(List<float> inputArray)
        {
            // Generating hidden's outputs

            // Fill input matrix
            Matrix input = Matrix.FromArray(inputArray);
            // Multiplying matrices of weights and layer = W * I
            Matrix hidden = Matrix.Multiply(weightsIH, input);
            // Adding biases = W * I + B
            hidden = Matrix.Add(hidden, biasIH);
            // Apply activation function = Sigmoid(W * I + B)
            hidden.Map(Sigmoid);

            // Generating output's output

            // Multiplying matrices of weights and layer = W * I
            Matrix output = Matrix.Multiply(weightsHO, hidden);
            // Adding biases = W * I + B
            output = Matrix.Add(output, biasHO);
            // Apply activation function = Sigmoid(W * I + B)
            output.Map(Sigmoid);

            return output.ToArray();
        }

        public void SetLearningRate(float newLearningRate)
        {
            learningRate = newLearningRate;
        }

        private float Sigmoid(float x)
        {
            return 1f / (1f + (float)Math.Exp(-x));
        }

        // We assume, that derivative has already calculated y of Sigmoid(x)
        private float DSigmoid(float y)
        {
            return y * (1f - y);
        }
    }
}
