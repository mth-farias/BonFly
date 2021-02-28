using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Vision;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]


public class BinaryRegionsTracking
{
    public struct DistanceConnectedComponentCollection
    {
        public double Distance;
        public ConnectedComponentCollection ConnCompCollection; 
    }

    private static double distance (OpenCV.Net.Point2f a,OpenCV.Net.Point2f b)
    {
        return Math.Sqrt(Math.Pow(b.X-a.X,2) + Math.Pow(b.Y-a.Y,2));
    }
    

	private DistanceConnectedComponentCollection permute(ConnectedComponentCollection current, 
								int l, int r, ConnectedComponentCollection previous)
	{
		if (l == r)
        {
            double totalDistance = 0;
            var newPermutationCopy = new ConnectedComponentCollection(current.ImageSize);
            for (int i = 0; i < current.Count; i++)
            {
                totalDistance += distance(current[i].Centroid,previous[i].Centroid);
                newPermutationCopy.Add(current[i]);
            }
            return new DistanceConnectedComponentCollection() {Distance = totalDistance, ConnCompCollection = newPermutationCopy};
        }
		else
		{ 
            var bestCandidateTillNow = new DistanceConnectedComponentCollection() {Distance = Double.MaxValue, ConnCompCollection = null};
			for (int i = l; i <= r; i++)
			{ 
				swap(current, l, i);
				
                var currentCandidate = permute(current, l + 1, r,previous);
                if (currentCandidate.Distance < bestCandidateTillNow.Distance)
                {
                   bestCandidateTillNow = currentCandidate;
                }
				swap(current, l, i);
			}
            return bestCandidateTillNow;
		} 
	}

	public static ConnectedComponentCollection swap(ConnectedComponentCollection a, 
							int i, int j)
	{ 
		ConnectedComponent temp;  
		temp = a[i] ; 
		a[i] = a[j]; 
		a[j] = temp;
		return a;
	}

// This code is contributed by mits 

    public IObservable<ConnectedComponentCollection> Process(IObservable<ConnectedComponentCollection> source)
    {
        ConnectedComponentCollection previous = null;
        return source.Select(value => 
        { 

            if (previous==null)
            {
                previous = new ConnectedComponentCollection(value,value.ImageSize);
                return previous; //bestPermute.permuted;
            }
            
            var valueCopy = new ConnectedComponentCollection(value.ImageSize);
            for (int i = 0; i < value.Count; i++)
            {
                valueCopy.Add(value[i]);
            }
            var bestPermute = permute(valueCopy,0,valueCopy.Count-1,previous);
            previous = bestPermute.ConnCompCollection;

            return bestPermute.ConnCompCollection;

        });
    }
}

            // var distanceTable = new Double[5,5];
            // for (int i = 0; i < current.Length; i++)
            // {
            //     for (int j = 0; j < previous.Length; i++) //(int j = 0 + i;
            //     {
            //         distanceTable[i,j] = distance (value[i].Centroid, previous[j].Centroid);
            //     }
            // }
