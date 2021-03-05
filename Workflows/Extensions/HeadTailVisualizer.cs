using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;
using Bonsai.Vision;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class HeadTailVisualizer
{
    public IObservable<IplImage> Process(IObservable<Tuple<IplImage, Point2f, Point2f>> source)
    {
        return source.Select(value => 
        {
            var image = value.Item1;
            var output = new IplImage(image.Size, image.Depth, 3);
            CV.CvtColor(image, output, ColorConversion.Gray2Bgr);
            
            var head = value.Item2;
            var tail = value.Item3;

            CV.Circle(output, new Point(head), 3, Scalar.Rgb(204, 0, 0), -1);
            CV.Circle(output, new Point(tail), 3, Scalar.Rgb(0, 0, 204), -1);
            
            return output;
        
        });
    }
}
