using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace cam_aforge1
{
    class Primitive
    {
        public Color color;
        public int x1, y1;
        public bool isFill = false;

        public Primitive(Color color, int X1, int Y1)
        {
            this.color = color;
            this.x1 = X1;
            this.y1 = Y1;
        }

        public virtual void Draw(Graphics g) { }
        

    }

    /// <summary>
    /// Used to make a Line
    /// </summary>
    class Line : Primitive
    {
        public int thickness;
        public int deltaX, deltaY;

        /// <summary>
        /// Initializes a Line Object
        /// </summary>
        /// <param name="lineColor">The color of the line. It is highly recommended
        /// to use the system color constants (ie `Color.Red`)</param>
        /// <param name="lineThickness">Thickness of the Line in Pixels</param>
        /// <param name="lineX1">The x-coordinate of the origin of the line</param>
        /// <param name="lineY1">The y-coordinate of the origin of the line</param>
        /// <param name="lineX2">The x-coordinate of the line destination</param>
        /// <param name="lineY2">The y-coordinate of the line destination</param>
        public Line(Color lineColor, int lineThickness,
            int lineX1, int lineY1, int lineX2, int lineY2)
            : base(lineColor, lineX1, lineY1)
        {
            thickness = lineThickness;
            deltaX = lineX2 - lineX1;
            deltaY = lineY2 - lineY1;
        }

        /// <summary>
        /// Draws a Line Object
        /// </summary>
        /// <param name="g">The graphics object to be drawn on. ALWAYS PASS ON g IN THIS PARAMETER</param>
        public override void Draw(Graphics g)
        {
            Pen linepen = new Pen(color, (float)thickness);
            g.DrawLine(linepen, x1, y1, x1 + deltaX, y1 + deltaY);
            linepen.Dispose();
        }
    }

    class Text : Primitive
    {
        public string text;
        public int size;

        /// <summary>
        /// Initializes a Text Object
        /// </summary>
        /// <param name="textContent">The string to be written</param>
        /// <param name="textColor">The color of the text. It is highly recommended
        /// to use the system color constants (ie `Color.Red`)</param>
        /// <param name="textX1">The x-coordinate of the origin of the text</param>
        /// <param name="textY1">The y-coordinate of the origin of the text</param>
        /// <param name="textSize">The font size of the text</param>
        public Text(String textContent, Color textColor,
            int textX1, int textY1, int textSize)
            : base(textColor, textX1, textY1)
        {
            text = textContent;
            size = textSize;
        }

        /// <summary>
        /// Draws a Text Object
        /// </summary>
        /// <param name="g">The graphics object to be drawn on. ALWAYS PASS ON g IN THIS PARAMETER</param>
        public override void Draw(Graphics g)
        {
            Font font = new Font(FontFamily.GenericSansSerif, size, FontStyle.Regular);
            Brush brush = new SolidBrush(color);
            g.DrawString(text, font, brush, x1, y1);

            font.Dispose();
            brush.Dispose();
        }
    }

    class Rectangle : Primitive
    {
        public int thickness;
        public int width, height;

        /// <summary>
        /// Initializes a Rectangle Object
        /// </summary>
        /// <param name="rectColor">The color of the Rectangle. It is highly recommended
        /// to use the system color constants (ie `Color.Red`)</param>
        /// <param name="rectThickness">Thickness of the Rectangle Sides in Pixels (Unused if Rectangle is Filled)</param>
        /// <param name="rectX1">The x-coordinate of the origin of the Rectangle</param>
        /// <param name="rectY1">The y-coordinate of the origin of the Rectangle</param>
        /// <param name="rectWidth">The Width of the Rectangle in Pixels</param>
        /// <param name="rectHeight">The Height of the Rectangle in Pixelsn</param>
        public Rectangle(Color rectColor, int rectThickness,
            int rectX1, int rectY1, int rectWidth, int rectHeight)
            : base(rectColor, rectX1, rectY1)
        {
            thickness = rectThickness;
            width = rectWidth;
            height = rectHeight;
        }

        /// <summary>
        /// Draws a Rectangle Object
        /// </summary>
        /// <param name="g">The graphics object to be drawn on. ALWAYS PASS ON g IN THIS PARAMETER</param>
        public override void Draw(Graphics g)
        {
            if (!isFill)
            {
                Pen rectpen = new Pen(color, (float)thickness);
                g.DrawRectangle(rectpen, x1, y1, width, height);
                rectpen.Dispose();
            }
            else
            {
                Brush rectbrush = new SolidBrush(color);
                g.FillRectangle(rectbrush, x1, y1, width, height);
                rectbrush.Dispose();
            }
        }
    }

    class Arc : Primitive
    {
        public int thickness;
        public int width, height;
        public int startangle, endangle;

        /// <summary>
        /// Initializes an Arc Object
        /// </summary>
        /// <param name="arcColor">The color of the Arc. It is highly recommended
        /// to use the system color constants (ie `Color.Red`)</param>
        /// <param name="arcThickness">Thickness of the Arc in Pixels</param>
        /// <param name="arcX1">The x-coordinate of the origin of the Arc</param>
        /// <param name="arcY1">The y-coordinate of the origin of the Arc</param>
        /// <param name="arcWidth">The Width of the imaginary rectangle enclosing the Arc in Pixels</param>
        /// <param name="arcHeight">The Height of the imaginary rectangle enclosing the Arc in Pixels</param>
        public Arc(Color arcColor, int arcThickness,
            int arcX1, int arcY1, int arcWidth, int arcHeight,
            int arcStartAngle, int arcEndAngle)
            : base(arcColor, arcX1, arcY1)
        {
            thickness = arcThickness;
            width = arcWidth;
            height = arcHeight;
            startangle = arcStartAngle;
            endangle = arcEndAngle;
        }

        /// <summary>
        /// Draws an Arc Object
        /// </summary>
        /// <param name="g">The graphics object to be drawn on. ALWAYS PASS ON g IN THIS PARAMETER</param>
        public override void Draw(Graphics g)
        {
            Pen arcpen = new Pen(color, (float)thickness);
            g.DrawArc(arcpen, x1 / 2, y1 / 2, width, height, startangle, endangle);
            arcpen.Dispose();
        }
    }

    class Circle : Primitive
    {
        public int thickness;
        public int size;

        /// <summary>
        /// Initializes a Circle Object
        /// </summary>
        /// <param name="cirColor">The color of the Circle. It is highly recommended
        /// to use the system color constants (ie `Color.Red`)</param>
        /// <param name="cirThickness">Thickness of the Circle outline in Pixels (Unused if Circle is Filled)</param>
        /// <param name="cirX1">The x-coordinate of the centre of the Circle</param>
        /// <param name="cirY1">The y-coordinate of the centre of the Circle</param>
        /// <param name="cirSize">The diameter of the Circle in Pixels</param>
        public Circle(Color cirColor, int cirThickness,
            int cirX1, int cirY1, int cirSize)
            : base(cirColor, cirX1, cirY1)
        {
            thickness = cirThickness;
            size = cirSize;
        }

        /// <summary>
        /// Draws a Circle Object
        /// </summary>
        /// <param name="g">The graphics object to be drawn on. ALWAYS PASS ON g IN THIS PARAMETER</param>
        public override void Draw(Graphics g)
        {
            if (!isFill)
            {
                Pen cirpen = new Pen(color, (float)thickness);
                g.DrawArc(cirpen, x1 - (size / 2), y1 - (size / 2), size, size, 0, 360);
                cirpen.Dispose();
            }
            else
            {
                Brush cirbrush = new SolidBrush(color);
                g.FillEllipse(cirbrush, x1 - (size / 2), y1 - (size / 2), size, size);
                cirbrush.Dispose();
            }
        }
    }

    class Square : Primitive
    {
        public int thickness;
        public int size;

        /// <summary>
        /// Initializes a Square Object
        /// </summary>
        /// <param name="sqrColor">The color of the Square. It is highly recommended
        /// to use the system color constants (ie `Color.Red`)</param>
        /// <param name="sqrThickness">Thickness of the Square outline in Pixels (Unused if Square is Filled)</param>
        /// <param name="sqrX1">The x-coordinate of the upper-left corner of the Square</param>
        /// <param name="sqrY1">The y-coordinate of the upper-left corner of the Square</param>
        /// <param name="sqrSize">The Radius of the Circle in Pixels</param>
        public Square(Color sqrColor, int sqrThickness,
            int sqrX1, int sqrY1, int sqrSize)
            : base(sqrColor, sqrX1, sqrY1)
        {
            thickness = sqrThickness;
            size = sqrSize;
        }

        /// <summary>
        /// Draws a Square Object
        /// </summary>
        /// <param name="g">The graphics object to be drawn on. ALWAYS PASS ON g IN THIS PARAMETER</param>
        public override void Draw(Graphics g)
        {
            if (!isFill)
            {
                Pen sqrpen = new Pen(color, (float)thickness);
                g.DrawRectangle(sqrpen, x1, y1, size, size);
                sqrpen.Dispose();
            }
            else
            {
                Brush sqrbrush = new SolidBrush(color);
                g.FillRectangle(sqrbrush, x1, y1, size, size);
                sqrbrush.Dispose();
            }
        }
    }
}
