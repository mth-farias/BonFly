using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;
using Bonsai.Vision;

[Combinator]
[Description("Isto fas visualizadires ")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class MultiVisualizer
{
    [Description("Method to use for comparing pixels. Darkest maintains the darkest values for each pixel. Brightest maintains the brightest values for each pixel.")]
    public int RadiusSize { get; set; } 
    //public Scalar[] Collors { get; set; } 
    public IObservable<IplImage> Process(IObservable<Tuple<IplImage, ConnectedComponentCollection>> source)
    {
        return source.Select(value => 
        {
            //var output = value.Item1.Clone();
            var image = value.Item1;
            var output = new IplImage(image.Size,image.Depth, 3);
            CV.CvtColor(image, output,ColorConversion.Gray2Bgr);
            // CV.Circle(output, new Point(value.Item2), RadiusSize, Collors[0], 2);
            // CV.Circle(output, new Point(value.Item3), RadiusSize, Collors[1] , 2);
            // CV.Circle(output, new Point(value.Item4), RadiusSize, Collors[2] , 2);
            // CV.Circle(output, new Point(value.Item5), RadiusSize, Collors[3] , 2);
            // CV.Circle(output, new Point(value.Item6), RadiusSize, Collors[4] , 2);
            // CV.Circle(output, new Point(value.Item6), RadiusSize, Collors[4] , 2);
            
            var colorArray = new List<Scalar>()
            {
                Scalar.Rgb(255, 0, 0),
                Scalar.Rgb(0, 255, 0),
                Scalar.Rgb(0, 0, 255),
                Scalar.Rgb(255, 0, 127),
                Scalar.Rgb(255, 255, 0)
            };
            var enumCollor = colorArray.GetEnumerator();
            
            foreach (var item in value.Item2)
            {
                enumCollor.MoveNext();
                CV.Circle(output, new Point(item.Centroid), RadiusSize, enumCollor.Current, 2);
            }
            
            return output;

        });
    }
}
