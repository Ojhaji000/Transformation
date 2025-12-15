using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

    namespace Transformation
{
    public partial class Form1 : Form
    {
        // Cumulative transform components
        private float _scale = 1f;                     // uniform scale
        private float _rotation = 0f;                  // degrees
        private PointF _translation = new(0f, 0f);     // translation in screen pixels

        // tweakable sensitivities
        private const float ScaleFactorPerNotch = 1.1f;
        private const float RotationDegPerNotch = 15f;
        private const float TranslationPxPerNotch = 20f;


        // ================= RECTANGLE ORIGIN INTERPRETATION =================
        // Rectangle origin is considered as the LEFT-MOST BOTTOM CORNER
        // (X, Y) represents the bottom-left reference point of the rectangle
        //
        //           Y+            
        //           |              WIDTH
        //           |                || 
        //           |                \/
        //           |           _______________
        //           |          |               |  
        //           |          |               |  <-- Height
        //           |          |_______________|  
        //           |             
        //           |          /\  
        //           |          ||   
        //           |          RECTANGLE ORIGIN
        // ---------------------------------------------------------> X+
        //           ^
        //          / \
        //           |
        //         Origin (0,0)
        //
        // RectangleF(0, 0, width, height)
        // -> Origin (0,0) = bottom-left corner
        // -> Width  extends toward +X direction
        // -> Height extends toward +Y direction
        // ================================================================

        private static PointF rectangleOrigin = new PointF() { X = 0f, Y = 0f };
        private static float rectangleWidth = 150f;
        private static float rectangleHeight = 100f;
        PointF[] rectanglePoints = new PointF[4]
            {
                rectangleOrigin,
                new PointF(rectangleOrigin.X +rectangleWidth, rectangleOrigin.Y),
                new PointF(rectangleOrigin.X +rectangleWidth, rectangleOrigin.Y+rectangleHeight),
                new PointF(rectangleOrigin.X, rectangleOrigin.Y+rectangleHeight),
            };


        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Paint+= CreateShapes;

            // Ensure form receives mouse wheel when hovered
            MouseEnter += (_, _) => Focus();
            MouseWheel += Form1_MouseWheel;
            //Paint += Form1_Paint;
        }

        private void CreateShapes(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF rect = new RectangleF(
                rectanglePoints[0].X,
                rectanglePoints[0].Y,
                Math.Abs(rectanglePoints[1].X - rectanglePoints[0].X), // width
                Math.Abs(rectanglePoints[2].Y - rectanglePoints[1].Y) // height
                );
            g.DrawRectangle(new Pen(Color.DarkBlue, 2f),rect);

            g.FillRectangle(new SolidBrush(Color.FromArgb(180, Color.CornflowerBlue)), rect);

            // draw axis cross for orientation reference
            using (Pen axisPen = new(Color.Red, 1f/_scale))
            {
                PointF rectangleCentre = new PointF(
                    rectangleOrigin.X + rectangleWidth / 2f,
                    rectangleOrigin.Y + rectangleHeight / 2f
                    );
                g.DrawLine(axisPen, 
                    // horizontal line left point
                    new PointF(rectangleCentre.X-200f,rectangleCentre.Y),
                    // horizontal line right point
                    new PointF(rectangleCentre.X + 200f, rectangleCentre.Y)
                    );
                g.DrawLine(axisPen,
                    // vertical line top point
                    new PointF(rectangleCentre.X, rectangleCentre.Y - 200f),
                    // vertical line bottom point
                    new PointF(rectangleCentre.X, rectangleCentre.Y + 200f)
                    );
            }
            DrawHud(e.Graphics);
            //float halfWidth = rectangleWidth / 2f;
            //float halfHeight = rectangleHeight / 2f;
            //rectanglePoints[0] = new PointF(rectangleCentre.X - halfWidth, rectangleCentre.Y - halfHeight); // Top-left
            //rectanglePoints[1] = new PointF(rectangleCentre.X + halfWidth, rectangleCentre.Y - halfHeight); // Top-right
            //rectanglePoints[2] = new PointF(rectangleCentre.X + halfWidth, rectangleCentre.Y + halfHeight); // Bottom-right
            //rectanglePoints[3] = new PointF(rectangleCentre.X - halfWidth, rectangleCentre.Y + halfHeight); // Bottom-left
        }



        private void Form1_MouseWheel(object? sender, MouseEventArgs e)
        {
            int notches = e.Delta / 120;
            if (notches == 0) return;

            // Build matrix representing the current transform (scale, rotate, translate)
            Matrix before = CreateTransform();

            // Map the mouse location to 'object/world' coordinates so we can preserve the point under the cursor
            Matrix beforeInv = before.Clone();
            bool invertible = true;
            try { beforeInv.Invert(); }
            catch { invertible = false; }

            PointF worldUnderCursor;
            if (invertible)
            {
                var pts = new[] { e.Location };
                beforeInv.TransformPoints(pts);
                worldUnderCursor = pts[0];
            }
            else
            {
                // fallback: treat world point as same as screen
                worldUnderCursor = e.Location;
            }

            // Determine which transformation to apply:
            // - Ctrl + Wheel => scale (zoom) centered at cursor
            // - Shift + Wheel => rotate around cursor
            // - Alt + Wheel => translate horizontally
            // - No modifier => translate vertically
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                float scaleChange = (float)Math.Pow(ScaleFactorPerNotch, notches);
                _scale *= scaleChange;

                // Recompute transform and adjust translation so worldUnderCursor remains under cursor
                Matrix after = CreateTransform();
                var pts = new[] { worldUnderCursor };
                after.TransformPoints(pts);
                PointF newScreen = pts[0];

                _translation.X += e.Location.X - newScreen.X;
                _translation.Y += e.Location.Y - newScreen.Y;
            }
            else if (ModifierKeys.HasFlag(Keys.Shift))
            {
                _rotation += notches * RotationDegPerNotch;

                // Recompute transform and adjust translation so worldUnderCursor remains under cursor
                Matrix after = CreateTransform();
                var pts = new[] { worldUnderCursor };
                after.TransformPoints(pts);
                PointF newScreen = pts[0];

                _translation.X += e.Location.X - newScreen.X;
                _translation.Y += e.Location.Y - newScreen.Y;
            }
            else if (ModifierKeys.HasFlag(Keys.Alt))
            {
                _translation.X += notches * TranslationPxPerNotch;
            }
            else
            {
                _translation.Y += -notches * TranslationPxPerNotch; // typical wheel scroll moves content vertically

            }

            Invalidate();
        }

        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Apply the cumulative transform to the Graphics
            using (Matrix m = CreateTransform())
            {
                g.Transform = m;

                // Draw a sample shape in object coordinates (centered at origin)
                using (Pen p = new(Color.DarkBlue, 2f / _scale)) // keep visual pen width roughly invariant to scale
                using (Brush b = new SolidBrush(Color.FromArgb(180, Color.CornflowerBlue)))
                {
                    // Example: a rounded rectangle / polygon centered at origin
                    RectangleF rect = new RectangleF(-75f, -50f, 150f, 100f);
                    g.FillRectangle(b, rect);
                    g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);

                    // draw a small axis cross for orientation reference
                    using (Pen axis = new(Color.Red, 1f / _scale))
                    {
                        g.DrawLine(axis, -200f, 0f, 200f, 0f); // X axis
                        g.DrawLine(axis, 0f, -200f, 0f, 200f); // Y axis
                    }
                }
            }

            // Optional: draw HUD in screen space (untransformed) to show current values
            DrawHud(e.Graphics);
        }

        private void DrawHud(Graphics g)
        {
            // Reset transform to draw UI overlay in screen coords
            g.ResetTransform();
            string info = $"Scale: {_scale:F2}   Rotation: {_rotation:F1}°   Translation: {_translation.X:F0}, {_translation.Y:F0}\n" +
                          "Wheel: • Ctrl=Zoom • Shift=Rotate • Alt=Pan X • None=Pan Y";
            using (Brush b = new SolidBrush(Color.FromArgb(220, Color.Black)))
            using (Font f = new("Segoe UI", 9f))
            {
                var size = g.MeasureString(info, f);
                RectangleF bg = new(8f, 8f, size.Width + 8f, size.Height + 8f);
                g.FillRectangle(b, bg);
                g.DrawString(info, f, Brushes.White, 12f, 12f);
            }
        }

        // Create the transformation matrix from the stored components.
        // Order: translate -> rotate -> scale (this means scale is applied in object space after rotation).
        private Matrix CreateTransform()
        {
            Matrix m = new();
            // Apply translation in screen pixels
            m.Translate(_translation.X, _translation.Y, MatrixOrder.Append);
            // Apply rotation (degrees)
            m.Rotate(_rotation, MatrixOrder.Append);
            // Apply uniform scale
            m.Scale(_scale, _scale, MatrixOrder.Append);
            return m;
        }

        private void Translate()
        {

        }
    }
}
