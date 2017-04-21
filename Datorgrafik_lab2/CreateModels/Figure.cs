using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Datorgrafik_lab2.CreateModels
{
    public class Figure
    {
        public List<VertexPositionNormalTexture> vertices { get; protected set; }
        List<Cube> parts;
        Dictionary<String, Matrix> transforms;
        
        public Figure()
        {
            vertices = new List<VertexPositionNormalTexture>();
            parts = new List<Cube>();
            transforms = new Dictionary<string, Matrix>();

            buildFigure();
        }

        public void buildFigure()
        {
            Torso();
            Head();

            foreach(Cube part in parts)
            {
                vertices.AddRange(part.vertices);
            }
        }

        public int[] getIndices()
        {
            return Enumerable.Range(0, 360).ToArray();
        }

        private void Torso()
        {
            Cube torso = new Cube();
            torso.transform(Matrix.Identity);
            parts.Add(torso);
        }

        private void Head()
        {
            Cube head = new Cube();
            head.transform(Matrix.CreateTranslation(new Vector3(0f,2f/0.5f,0f)) 
                * Matrix.CreateScale(0.5f));
            parts.Add(head);
        }
    }
}
