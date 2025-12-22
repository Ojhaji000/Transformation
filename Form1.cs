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

        private static PointF rectangleOrigin = new PointF();//{ X = 0f, Y = 0f };
        private static PointF[] rectanglePoints = new PointF[4];

        private static PointF[] trianglePoints = new PointF[4]
        {
            new PointF(0f, 50f),
            new PointF(50f, -50f),
            new PointF(-50f, -50f),
            new PointF(0f, 50f)

        };
        private PointF translationAmountInPoints = new PointF() { X = 0f, Y = 0f }; // translation in screen pixels
        Transformation.CoordinateSystem coordSys = null;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            // Ensure form receives mouse wheel when hovered
            MouseEnter += (_, _) => Focus();
            MouseWheel += Form1_MouseWheel;
            Paint += CreateShapes;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Size cs = ClientSize;

            // Example
            PointF drawingBoardCenter = new PointF(
                cs.Width / 2f,
                cs.Height / 2f
            );

            coordSys = new Transformation.CoordinateSystem(new PointF(drawingBoardCenter.X, drawingBoardCenter.Y), cs.Width, cs.Height);

            rectangleOrigin = new PointF(-75f, -50f);
            rectanglePoints= new PointF[4] {
                rectangleOrigin,
                new PointF(rectangleOrigin.X + 150, rectangleOrigin.Y),
                new PointF(rectangleOrigin.X + 150, rectangleOrigin.Y + 100),
                new PointF(rectangleOrigin.X, rectangleOrigin.Y + 100),
            };

        }
        private void CreateShapes(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            coordSys.Adjustment(ref rectanglePoints);
            PointF[] rectangleClosedCurve = new PointF[5]
            {
                rectanglePoints[0],
                rectanglePoints[1],
                rectanglePoints[2],
                rectanglePoints[3],
                rectanglePoints[0]
            };
            g.DrawLines(new Pen(Color.DarkBlue, 2f), rectangleClosedCurve);
            g.FillPolygon(new SolidBrush(Color.FromArgb(180, Color.CornflowerBlue)), rectangleClosedCurve);

            PointF[] xAxis = { new PointF(-200, 0), new PointF(200, 0) };
            PointF[] yAxis = { new PointF(0, -200), new PointF(0, 200) };
            coordSys.Adjustment(ref xAxis);
            coordSys.Adjustment(ref yAxis);

            // draw axis cross for orientation reference
            using (Pen axisPen = new(Color.Red, 1f / _scale))
            {
                g.DrawLine(axisPen,xAxis[0], xAxis[1]); 
                g.DrawLine(axisPen,yAxis[0], yAxis[1]);
            }

            using (Font drawFont = new Font("Arial", 10, FontStyle.Bold))
            using (SolidBrush drawBrush = new SolidBrush(Color.Red))
            {
                g.DrawString("-x", drawFont, drawBrush, xAxis[0].X, xAxis[0].Y );
                g.DrawString("x", drawFont, drawBrush, xAxis[1].X, xAxis[1].Y);
                g.DrawString("y", drawFont, drawBrush,  yAxis[0].X, yAxis[0].Y);
                g.DrawString("-y", drawFont, drawBrush, yAxis[1].X, yAxis[1].Y);
            }

            coordSys.UndoAdjustment(ref rectanglePoints);
            coordSys.UndoAdjustment(ref xAxis);
            coordSys.UndoAdjustment(ref yAxis);

            DrawHud(e.Graphics);
        }

        private void Form1_MouseWheel(object? sender, MouseEventArgs e)
        {
            int notches = e.Delta / 120;
            if (notches == 0) return;

            // Determine which transformation to apply:
            // - Ctrl + Wheel => scale (zoom) centered at cursor
            // - Shift + Wheel => rotate around cursor
            // - Alt + Wheel => translate horizontally
            // - No modifier => translate vertically
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                // scaling
            }
            else if (ModifierKeys.HasFlag(Keys.Shift))
            {
                // rotation
                float rotationAngle = notches * RotationDegPerNotch;
                _rotation -= rotationAngle;
                double rotationAngleRad = (double)rotationAngle * (Math.PI / 180f);
                float[,] rotationMatrix = new float[2,2]
                    {{(float)Math.Cos(rotationAngleRad),-(float)Math.Sin(rotationAngleRad)},
                    {(float)Math.Sin(rotationAngleRad),(float)Math.Cos(rotationAngleRad)}
                };
                Rotate(rotationMatrix);
            }
            else if (ModifierKeys.HasFlag(Keys.Alt))
            {
                // vertical translation
                float translateNum = -notches * TranslationPxPerNotch; // negative because screen Y axis is inverted in WINFORMS
                float[,] translationMatrix = new float[3, 3]
                {
                    { 1, 0, 0 },
                    { 0, 1, translateNum },
                    { 0, 0, 1 }
                };
                Translate(translationMatrix);
            }
            else
            {
                // horizontal translation
                float translateNum = notches * TranslationPxPerNotch;
                float[,] translationMatrix = new float[3, 3]
                {
                    { 1, 0, translateNum },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                };
                Translate(translationMatrix);
            }
            translationAmountInPoints = new PointF(
                rectanglePoints[0].X - rectangleOrigin.X,
                rectangleOrigin.Y - rectanglePoints[0].Y // reverse becasue screen y axis is inverted in WINFORMS
                );
            Invalidate();
            /*
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
            */
        }

        private void DrawHud(Graphics g)
        {
            // Reset transform to draw UI overlay in screen coords
            g.ResetTransform();
            string info = $"Translation: {translationAmountInPoints.X:F0}, {translationAmountInPoints.Y:F0}, Rotation: {_rotation:F1}°\n" + // Scale: {_scale:F2}  
                          "Wheel: • Alt=Pan X • None=Pan X • Shift=Rotate";//  • Ctrl=Zoom 
            using (Brush b = new SolidBrush(Color.FromArgb(220, Color.Black)))
            using (Font f = new("Segoe UI", 9f))
            {
                var size = g.MeasureString(info, f);
                RectangleF bg = new(8f, 8f, size.Width + 8f, size.Height + 8f);
                g.FillRectangle(b, bg);
                g.DrawString(info, f, Brushes.White, 12f, 12f);
            }
        }

        private void Translate(float[,] translationMatrix)
        {
            float[,] resultVector = new float[3,1];

            for (int i = 0; i < rectanglePoints.Length; i++)
            {
                float[,] pointVector = new float[3, 1]
                {
                    { rectanglePoints[i].X },
                    { rectanglePoints[i].Y },
                    { 1 }
                };
                Matrix.Multiply(translationMatrix, pointVector, out resultVector);
                rectanglePoints[i] = new PointF(resultVector[0, 0], resultVector[1, 0]);
            }
        }

        private void Rotate(float[,] rotationMatrix)
        {
            float[,] resultVector = new float[2, 1];

            for (int i = 0; i < rectanglePoints.Length; i++)
            {
                float[,] pointVector = new float[2, 1]{
                    { rectanglePoints[i].X },
                    { rectanglePoints[i].Y }
                };
                Matrix.Multiply(rotationMatrix, pointVector, out resultVector);
                rectanglePoints[i] = new PointF(resultVector[0, 0], resultVector[1, 0]);
            }
        }
    }
}
