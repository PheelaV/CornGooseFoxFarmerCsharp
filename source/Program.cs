using System;
using System.Collections.Generic;

internal class Program
{
    private const byte Corn = 0b_1000;
    private const byte Goose = 0b_0100;
    private const byte Fox = 0b_0010;
    private const byte Farmer = 0b_0001;

    // Which bits to switch using XOR
    private const byte FarmerMoves = Farmer;
    private const byte FarmerCornMoves = Farmer | Corn;
    private const byte FarmerGooseMoves = Farmer | Goose;
    private const byte FarmerFoxMoves = Farmer | Fox;

    private const byte InitialState = 0b0000;
    private const byte FinalState = 0b1111;

    private static void Main()
    {
        var stepCounter = 0;
        var possibleStepMasks = new []{ FarmerCornMoves, FarmerGooseMoves, FarmerFoxMoves, FarmerMoves };
        var pathsUnderConsideration = new List<Path>{new Path(InitialState) };
        var validPaths = new List<Path>();

        static bool StepIsValid(byte state, byte step) => step switch
        {
            FarmerMoves => true,
            FarmerFoxMoves => (state & Fox) >> 1 == (state & Farmer),
            FarmerGooseMoves => (state & Goose) >> 2 == (state & Farmer),
            FarmerCornMoves => (state & Corn) >> 3 == (state & Farmer),
            _ => throw new ArgumentException("Invalid value")
        };
        

        static bool StateIsValid(byte state) => state switch
            {
                // The world ends
                0b_0001 => false,
                0b_1110 => false,
                // Corn eats goose
                0b_0011 => false,
                0b_1100 => false,
                // Fox eats goose
                0b_0110 => false,
                0b_1001 => false,
                _ => true
        };

        while (pathsUnderConsideration.Count > 0)
        {
            foreach(var path in new List<Path>(pathsUnderConsideration))
            {
                var candidateSteps = new List<byte>();

                foreach(var possibleStepMask in possibleStepMasks)
                {
                    stepCounter++;

                    if(!StepIsValid(path.LastState, possibleStepMask))
                    {
                        continue;
                    }

                    var possibleStep = (byte)(path.LastState ^ possibleStepMask);                    

                    if(!StateIsValid(possibleStep))
                    {
                        continue;
                    }

                    if(!path.ContainsState(possibleStep))
                    {
                        candidateSteps.Add(possibleStep);
                    }
                }

                foreach(var candidateState in candidateSteps)
                {
                    if(FinalState == candidateState)
                    {
                        validPaths.Add(new Path(candidateState, path));
                    } 
                    else 
                    {
                        pathsUnderConsideration.Add(new Path(candidateState, path));
                    }
                }
                pathsUnderConsideration.Remove(path);
            }
        }

        Console.WriteLine($"Paths found: {validPaths.Count}");  
        Console.WriteLine($"Steps considered: {stepCounter}");
        Console.WriteLine();
        foreach(var validPath in validPaths)
        {
            Console.WriteLine(string.Join(',',validPath.GetStates()));
        }
    }

    private class Path
    {
        private Path Parent { get; }
        public byte LastState { get; }

        public Path(byte state) => LastState = state;
        public Path(byte state, Path parent)
        {
            LastState = state;
            Parent = parent;
        }

        public bool ContainsState(byte state) => LastState == state || (Parent?.ContainsState(state) ?? false);

        public List<byte> GetStates()
        {
            if (Parent is null)
            {
                return new List<byte> { LastState };
            }

            var states = Parent.GetStates();
            states.Add(LastState);
            return states;
        }
    }
}