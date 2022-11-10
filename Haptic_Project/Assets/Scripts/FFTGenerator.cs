using System.Collections.Generic;
using System;
using System.Numerics;

public class FFTGenerator
{
    public static Complex[] BitReverseArrayShuffle(Complex[] input)
    {
        int nBits = GetMinimumBits(input.Length);
        Complex[] output = new Complex[input.Length];
        for (int k = 0; k < input.Length; k++)
        {
            output[k] = input[ReverseBitOrder(k, nBits)];
        }

        return output;
    }

    public static double[] BitReverseArrayShuffle(double[] input)
    {
        int nBits = GetMinimumBits(input.Length);
        double[] output = new double[input.Length];
        for (int k = 0; k < input.Length; k++)
        {
            output[k] = input[ReverseBitOrder(k, nBits)];
        }

        return output;
    }

    public static Complex[] BitReverseArrayShuffleAndToComplex(double[] input)
    {
        int nBits = GetMinimumBits(input.Length - 1); //-1 to take into account 0-based indexing
        Complex[] output = new Complex[input.Length];
        int reversedIndex;
        for (int k = 0; k < input.Length; k++)
        {
            reversedIndex = ReverseBitOrder(k, nBits);
            output[k] = input[reversedIndex];
        }

        return output;
    }

    public static Complex[] BitReverseArrayShuffleAndToComplex(double[] input, int startIndex, int endIndex)
    {
        int nBits = GetMinimumBits(input.Length - 1); //-1 to take into account 0-based indexing
        int segmentLength = endIndex - startIndex + 1;
        Complex[] output = new Complex[segmentLength];
        int reversedIndex;
        for (int k = 0; k < segmentLength; k++)
        {
            reversedIndex = ReverseBitOrder(k, nBits);
            output[k] = input[reversedIndex + startIndex];
        }

        return output;
    }

    private static Complex ComputeTwiddleFactor(int r, int N)
    {
        Complex WrN;

        WrN = Complex.Exp(-Complex.ImaginaryOne * (2 * Math.PI * r / N));
        return WrN;
    }

    public static double[] ExtendToPowOf2AndZeroPad(double[] input)
    {
        //zero padding for the next 2^n length
        int newLength; //new list count which is power of two
        newLength = RoundUpToNextPowOf2(input.Length);
        double[] output = new double[newLength];

        input.CopyTo(output, 0); //copy input to the output

        return output;
    }

    public static int GetMinimumBits(int inputNumber)
    {
        //gets the minimum number of bits(nBits) that could be theoretically used to represent the inputNumber
        int nBits = 1;
        int i = 1;
        if (inputNumber < 0)
            throw new ArgumentException("Negative values not allowed");
        while (inputNumber > i)
        {
            i |= (i << 1);
            nBits++;
        }

        return nBits;
    }

    public static Complex[] FFT(double[] x, bool zeroPad = false)
    {
        //Uses Radix 2 decimation in time FFT algorithm
        //Although the input x is double it is treated as complex number by the algorithm and further optimisation can be made for real inputs
        //Reference: Oppenheimer A., Schaffer R. W. "Digital Signal Processing" 1975 Prentice Hall International

        //This function takes full length of the input x, so it may not be ideal for a standalone FFT application, but it is sufficient for a spectrum estimation using Welch method
        //To be consisent with DSP conventions:
        //x - input signal
        //X - output signal
        //N - FFT and singal lengths
        //n - input signal sample
        //k - FFT point


        if (x.Length < 0)
        {
            throw new IndexOutOfRangeException("signal length negative");
        }


        if (zeroPad == true)
        {
            //extend the input to the power of two length and zero pad
            x = ExtendToPowOf2AndZeroPad(x);
        }


        if (IsNumberAPowerOf2(x.Length)) //check if input signal length is a power of 2
        {
            /*do nothing*/
        }
        else
            throw new ArgumentException(
                "Provide FFT length as a power of 2, or set zeroPad to true to pad the signal with zeros and extend the length to the next power of 2");

        int N = x.Length;
        int b = GetMinimumBits(N) - 1;

        Complex[] X;

        X = BitReverseArrayShuffleAndToComplex(
            x); //Shuffle the FFT input using bit-reverse technique and automatically convert to Complex number
        Complex[] WrN = new Complex[N >> 1];

        for (int R = 0; R < N / 2; R++)
        {
            WrN[R] = ComputeTwiddleFactor(R, N);
        }

        int p;
        int q;
        int r = 0;

        int d = 1;

        //tests lists to check if the programme iterates over ps and qs correctly
        List<int> ListOfPs = new List<int>();
        List<int> ListOfQs = new List<int>();

        //temporary complex variables
        Complex nextXp;
        Complex nextXq;
        for (int v = 1, u = 1; u < N; u <<= 1, v++) //v=log_2(N) operations
        {
            p = 0;
            q = u;
            for (int n = 0; n < N / 2; n++)
            {
                //compute a single FFT butterfly
                nextXp = X[p] + WrN[r] * X[q];
                nextXq = X[p] - WrN[r] * X[q];
                X[p] = nextXp;
                X[q] = nextXq;

                //Only for Debugging
                //ListOfPs.Add(p);
                //ListOfQs.Add(q);


                //Iterate p,q and r appropriately
                if ((n + 1) % u == 0)
                {
                    p = d * (u << 1);
                    q = p + u;
                    d++;
                }
                else
                {
                    p++;
                    q++;
                }

                if (r < ((N >> 1) - (1 << b - v)))
                {
                    r += (1 << b - v);
                }
                else
                {
                    r = 0;
                }
            }


            d = 1; //reset 
        }


        return X;
    }

    public static Complex[] FFT(double[] x, int startIndex, int endIndex)
    {
        //Uses Radix 2 decimation in time FFT algorithm
        //Although the input x is double it is treated as complex number by the algorithm and further optimisation can be made for real inputs
        //Reference: Oppenheimer A., Schaffer R. W. "Digital Signal Processing" 1975 Prentice Hall International

        //This function takes full length of the input x, so it may not be ideal for a standalone FFT application, but it is sufficient for a spectrum estimation using Welch method
        //To be consisent with DSP conventions:
        //x - input signal
        //X - output signal
        //N - FFT and singal lengths
        //n - input signal sample
        //k - FFT point

        int N = endIndex - startIndex + 1;
        if (N < 0)
        {
            throw new IndexOutOfRangeException("signal length negative, check input signal indices");
        }


        if (IsNumberAPowerOf2(N)) //check if input signal length is a power of 2
        {
            /*do nothing*/
        }
        else
            throw new ArgumentException(
                "Provide FFT length as a power of 2, or set zeroPad to true to pad the signal with zeros and extend the length to the next power of 2");


        int b = GetMinimumBits(N) - 1;

        Complex[] X;

        X = BitReverseArrayShuffleAndToComplex(x, startIndex,
            endIndex); //Shuffle the FFT input using bit-reverse technique and automatically convert to Complex number
        Complex[] WrN = new Complex[N >> 1];

        for (int R = 0; R < N / 2; R++)
        {
            WrN[R] = ComputeTwiddleFactor(R, N);
        }

        int p;
        int q;
        int r = 0;

        int d = 1;

        //tests lists to check if the programme iterates over ps and qs correctly
        List<int> ListOfPs = new List<int>();
        List<int> ListOfQs = new List<int>();

        //temporary complex variables
        Complex nextXp;
        Complex nextXq;
        for (int v = 1, u = 1; u < N; u <<= 1, v++) //v=log_2(N) operations
        {
            p = 0;
            q = u;
            for (int n = 0; n < N / 2; n++)
            {
                //compute a single FFT butterfly
                nextXp = X[p] + WrN[r] * X[q];
                nextXq = X[p] - WrN[r] * X[q];
                X[p] = nextXp;
                X[q] = nextXq;

                //Only for Debugging
                //ListOfPs.Add(p);
                //ListOfQs.Add(q);


                //Iterate p,q and r appropriately
                if ((n + 1) % u == 0)
                {
                    p = d * (u << 1);
                    q = p + u;
                    d++;
                }
                else
                {
                    p++;
                    q++;
                }

                if (r < ((N >> 1) - (1 << b - v)))
                {
                    r += (1 << b - v);
                }
                else
                {
                    r = 0;
                }
            }


            d = 1; //reset 
        }


        return X;
    }

    public static Complex[] FFT(double[] x, double[] window, int startIndex, int endIndex)
    {
        //Uses Radix 2 decimation in time FFT algorithm
        //Although the input x is double it is treated as complex number by the algorithm and further optimisation can be made for real inputs
        //Reference: Oppenheimer A., Schaffer R. W. "Digital Signal Processing" 1975 Prentice Hall International

        //This function takes full length of the input x, so it may not be ideal for a standalone FFT application, but it is sufficient for a spectrum estimation using Welch method
        //To be consisent with DSP conventions:
        //x - input signal
        //X - output signal
        //N - FFT and singal lengths
        //n - input signal sample
        //k - FFT point

        int N = endIndex - startIndex + 1;
        if (N < 0)
        {
            throw new IndexOutOfRangeException("signal length negative, check input signal indices");
        }


        if (IsNumberAPowerOf2(N)) //check if input signal length is a power of 2
        {
            /*do nothing*/
        }
        else
            throw new ArgumentException(
                "Provide FFT length as a power of 2, or set zeroPad to true to pad the signal with zeros and extend the length to the next power of 2");


        int b = GetMinimumBits(N) - 1;
        double[] windowedInput = new double[N];
        for (int i = 0; i < N; i++)
        {
            windowedInput[i] = x[startIndex + i] * window[i];
        }

        Complex[] X;

        X = BitReverseArrayShuffleAndToComplex(
            windowedInput); //Shuffle the FFT input using bit-reverse technique and automatically convert to Complex number
        Complex[] WrN = new Complex[N >> 1];

        for (int R = 0; R < N / 2; R++)
        {
            WrN[R] = ComputeTwiddleFactor(R, N);
        }

        int p;
        int q;
        int r = 0;

        int d = 1;

        //tests lists to check if the programme iterates over ps and qs correctly
        List<int> ListOfPs = new List<int>();
        List<int> ListOfQs = new List<int>();

        //temporary complex variables
        Complex nextXp;
        Complex nextXq;
        for (int v = 1, u = 1; u < N; u <<= 1, v++) //v=log_2(N) operations
        {
            p = 0;
            q = u;
            for (int n = 0; n < N / 2; n++)
            {
                //compute a single FFT butterfly
                nextXp = X[p] + WrN[r] * X[q];
                nextXq = X[p] - WrN[r] * X[q];
                X[p] = nextXp;
                X[q] = nextXq;

                //Only for Debugging
                //ListOfPs.Add(p);
                //ListOfQs.Add(q);


                //Iterate p,q and r appropriately
                if ((n + 1) % u == 0)
                {
                    p = d * (u << 1);
                    q = p + u;
                    d++;
                }
                else
                {
                    p++;
                    q++;
                }

                if (r < ((N >> 1) - (1 << b - v)))
                {
                    r += (1 << b - v);
                }
                else
                {
                    r = 0;
                }
            }


            d = 1; //reset 
        }


        return X;
    }

    public static Complex[] FFT(List<double> x, double[] window, int startIndex, int endIndex)
    {
        //Uses Radix 2 decimation in time FFT algorithm
        //Although the input x is double it is treated as complex number by the algorithm and further optimisation can be made for real inputs
        //Reference: Oppenheimer A., Schaffer R. W. "Digital Signal Processing" 1975 Prentice Hall International

        //This function takes full length of the input x, so it may not be ideal for a standalone FFT application, but it is sufficient for a spectrum estimation using Welch method
        //To be consisent with DSP conventions:
        //x - input signal
        //X - output signal
        //N - FFT and singal lengths
        //n - input signal sample
        //k - FFT point

        int N = endIndex - startIndex + 1;
        if (N < 0)
        {
            throw new IndexOutOfRangeException("signal length negative, check input signal indices");
        }


        if (IsNumberAPowerOf2(N)) //check if input signal length is a power of 2
        {
            /*do nothing*/
        }
        else
            throw new ArgumentException(
                "Provide FFT length as a power of 2, or set zeroPad to true to pad the signal with zeros and extend the length to the next power of 2");


        int b = GetMinimumBits(N) - 1;
        double[] windowedInput = new double[N];
        for (int i = 0; i < N; i++)
        {
            windowedInput[i] = x[startIndex + i] * window[i];
        }

        Complex[] X;

        X = BitReverseArrayShuffleAndToComplex(
            windowedInput); //Shuffle the FFT input using bit-reverse technique and automatically convert to Complex number
        Complex[] WrN = new Complex[N >> 1];

        for (int R = 0; R < N / 2; R++)
        {
            WrN[R] = ComputeTwiddleFactor(R, N);
        }

        int p;
        int q;
        int r = 0;

        int d = 1;

        //tests lists to check if the programme iterates over ps and qs correctly
        List<int> ListOfPs = new List<int>();
        List<int> ListOfQs = new List<int>();

        //temporary complex variables
        Complex nextXp;
        Complex nextXq;
        for (int v = 1, u = 1; u < N; u <<= 1, v++) //v=log_2(N) operations
        {
            p = 0;
            q = u;
            for (int n = 0; n < N / 2; n++)
            {
                //compute a single FFT butterfly
                nextXp = X[p] + WrN[r] * X[q];
                nextXq = X[p] - WrN[r] * X[q];
                X[p] = nextXp;
                X[q] = nextXq;

                //Only for Debugging
                //ListOfPs.Add(p);
                //ListOfQs.Add(q);


                //Iterate p,q and r appropriately
                if ((n + 1) % u == 0)
                {
                    p = d * (u << 1);
                    q = p + u;
                    d++;
                }
                else
                {
                    p++;
                    q++;
                }

                if (r < ((N >> 1) - (1 << b - v)))
                {
                    r += (1 << b - v);
                }
                else
                {
                    r = 0;
                }
            }


            d = 1; //reset 
        }


        return X;
    }

    public static bool IsNumberAPowerOf2(int N)
    {
        return (N != 0 && ((N & N - 1) == 0));
    }

    public static int ReverseBitOrder(int input, int nBits)
    {
        //Reverse bit order for an nBits bit long number, any bit number from 1 to 32 is supported

        int output = 0;
        int n = nBits; //while loop iterator
        while (n > 0)
        {
            if ((input & (1 << (nBits - n))) > 0)
            {
                output |= (1 << (n - 1));
            }

            n--;
        }

        return output;
    }

    public static int RoundUpToNextPowOf2(int oldLength)
    {
        int newLength = 2; //new list count which is power of two
        while (newLength < oldLength)
        {
            newLength *= 2;
        }

        return newLength;
    }
}