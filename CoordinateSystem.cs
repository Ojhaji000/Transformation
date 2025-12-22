namespace Transformation
{
    class CoordinateSystem
    {
        private PointF Origin;
        private float WindowWidth;
        private float WindowHeight;
        public CoordinateSystem(PointF origin, float windowWidth, float windowHeight)
        {
            Origin = origin;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }
        public void Adjustment(ref PointF[] points)
        {
            PointF[] resultant = new PointF[points.Length];
            // Implementation of coordinate transformation
            for(int i = 0; i < points.Length;i++)
            {
                resultant[i] = new PointF(points[i].X + (WindowWidth / 2), (points[i].Y) + (WindowHeight/2)); // Example transformation
                //point.Y = point.Y + 10; // Example transformation
            }
            points = resultant;
        }

        public void UndoAdjustment(ref PointF[] points)
        {
            PointF[] resultant = new PointF[points.Length];
            // Implementation of coordinate transformation
            for (int i = 0; i < points.Length; i++)
            {
                resultant[i] = new PointF(points[i].X - (WindowWidth / 2), (points[i].Y) - (WindowHeight / 2)); // Example transformation
                //point.Y = point.Y + 10; // Example transformation
            }
            points = resultant;
        }

    }
}