using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Linq;

namespace cam_aforge1
{
    class Shape
    {
        public List<Primitive> shapeList = new List<Primitive>();
        Graphics canvas;

        public Shape() { }

        public void Add(Primitive item)
        {
            shapeList.Add(item);
        }

        public void Draw(Graphics g)
        {
            if (this.shapeList.Count > 0)
            {
                int rootX = shapeList[0].x1;
                int rootY = shapeList[0].y1;

                shapeList[0].Draw(g);
                foreach (Primitive item in shapeList.Skip(1))
                {
                    item.x1 = rootX + item.x1;
                    item.y1 = rootY + item.y1;
                    item.Draw(g);
                }
            }
        }
    }
}
