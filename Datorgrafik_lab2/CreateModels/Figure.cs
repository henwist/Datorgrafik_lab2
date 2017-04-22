﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Datorgrafik_lab2.InstanceContainers;

namespace Datorgrafik_lab2.CreateModels
{
    public class Figure
    {
        public List<VertexPositionNormalTexture> vertices { get; protected set; }
        public int[] indices { get; protected set; }
        List<Cube> parts;
        Dictionary<String, Matrix> transforms;
        InstanceStack stack;
        
        private readonly int RIGHT_LEG_INDEX_START = 216;
        
        public Figure()
        {
            vertices = new List<VertexPositionNormalTexture>();
            parts = new List<Cube>();
            transforms = new Dictionary<string, Matrix>();
            stack = new InstanceStack();

            buildFigure();
            indices = Enumerable.Range(0, vertices.Count).ToArray();
        }

        public void buildFigure()
        {
            Torso();
            Head();
            UpperLeftArm();
            LowerLeftArm();
            UpperRightArm();
            //LowerRightArm();
            UpperLeftLeg();
            //LowerLeftLeg();
            UpperRightLeg();
            //LowerRigtLeg();

            foreach(Cube part in parts)
            {
                vertices.AddRange(part.vertices);
            }
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

        private void UpperLeftArm()
        {
            Cube upperLeftArm = new Cube();

            Matrix instance = Matrix.CreateTranslation(new Vector3(1f / .25f, 1f / .25f, 0f))
                * Matrix.CreateScale(.25f);
            transforms.Add("UpperLeftArm", instance);

            upperLeftArm.transform(instance);
            parts.Add(upperLeftArm);
        }

        private void LowerLeftArm()
        {
            Cube lowerLeftArm = new Cube();
            Matrix relPos;
            transforms.TryGetValue("UpperLeftArm", out relPos);

            Matrix instance = Matrix.CreateTranslation(new Vector3(0f , -2f / .25f, 0f)) *
                (relPos / Matrix.CreateFromAxisAngle(new Vector3(.25f, .25f, .25f), 0f))
                * Matrix.CreateScale(.25f);

            lowerLeftArm.transform(instance);
            parts.Add(lowerLeftArm);
        }

        public void UpperRightArm()
        {
            Cube upperRightArm = new Cube();

            Matrix instance = Matrix.CreateTranslation(new Vector3(-1f / .25f, 1f / .25f, 0f))
                * Matrix.CreateScale(.25f);
            transforms.Add("UpperRightArm", instance);

            upperRightArm.transform(instance);
            parts.Add(upperRightArm);
        }

        private void LowerRightArm()
        {

        }

        private void UpperLeftLeg()
        {
            Cube upperLeftLeg = new Cube();

            Matrix instance = Matrix.CreateTranslation(new Vector3(.5f / .25f, -2f / .25f, 0f))
                * Matrix.CreateScale(.25f);
            transforms.Add("UpperLeftLeg", instance);

            upperLeftLeg.transform(instance);
            parts.Add(upperLeftLeg);
        }

        private void LowerLeftLeg()
        {

        }

        private void UpperRightLeg()
        {
            Cube upperRightLeg = new Cube();

            Matrix instance = Matrix.CreateTranslation(new Vector3(-.5f / .25f, -2f / .25f, 0f))
                * Matrix.CreateScale(.25f);
            transforms.Add("UpperRightLeg", instance);

            upperRightLeg.transform(instance);
            parts.Add(upperRightLeg);
        }

        private void LowerRightLeg()
        {

        }

        public void RotateUpperRightLeg(float posX)
        {
            Vector3 translation = transforms["UpperRightLeg"].Translation;
            Matrix translateBackToPos = Matrix.CreateTranslation(translation);
            Matrix translateToOrigo = Matrix.CreateTranslation(-1*translation);
;
            Quaternion qrot = (transforms["UpperRightLeg"].Rotation
                              + Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(posX)));
            qrot.Normalize();

            Matrix rotate = Matrix.CreateFromQuaternion(qrot);

            VertexPositionNormalTexture vertex;

            foreach (int index in Enumerable.Range(RIGHT_LEG_INDEX_START, 36))
            {
                vertex = vertices.ElementAt(index);
                vertex.Position = Vector3.Transform(vertices[index].Position, translateToOrigo * rotate * translateBackToPos);
                vertices[index] = vertex;
            }

        }
    }
}
