using CollisionSample;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.Helpers
{
    public static class BoundingVolume
    {
        public static BoundingVolumeComponent GetBoundingBoxVolume(VertexPositionNormalTexture[] vertices, Matrix objWorld)
        {
            BoundingVolumeComponent bvCmp = new BoundingVolumeComponent();

            //Matrix withoutRotation = Matrix.CreateScale(objWorld.Scale) * Matrix.CreateTranslation(objWorld.Translation);
            VertexPositionNormalTexture[] copyVertices;/* = new VertexPositionNormalTexture[vertices.Length];*/
            copyVertices = vertices.ToArray();

            for (int i = 0; i < vertices.Length; i++)
                copyVertices[i].Position = Vector3.Transform(vertices[i].Position, objWorld);

            //List<VertexPositionNormalTexture> verts = vertices.ToList();
            //VertexPositionNormalTexture[] verts = vertices.ToArray();
            //Array.ForEach(verts, new Action<VertexPositionNormalTexture>(x =>   Vector3.Transform(x.Position, objWorld)));

                //verts.ForEach( x =>  x.Position = Vector3.Transform(x.Position, objWorld));


                //vertices.ToList().ForEach(x => x.Position = Vector3.Transform(x.Position, objWorld));

            bvCmp.bbox = BoundingBox.CreateFromPoints(copyVertices.Select(x => x.Position));

             return bvCmp;
        }

        public static void DrawBoundingVolume(GraphicsDevice gd, BoundingVolumeComponent boundingVolume, CameraComponent camera, Matrix objWorld)
        {
            DebugDraw debugDraw = new DebugDraw(gd);

            debugDraw.Begin(objWorld, camera.viewMatrix, camera.projectionMatrix);
            debugDraw.DrawWireBox(boundingVolume.bbox, Color.White);
            debugDraw.End();

            debugDraw.Dispose();
        }

        public static void DrawWireFrustum(GraphicsDevice gd, CameraComponent camera)
        {
            DebugDraw debugDraw = new DebugDraw(gd);

            debugDraw.Begin(Matrix.Identity, camera.viewMatrix, camera.projectionMatrix);
            debugDraw.DrawWireFrustum(camera.bFrustum, Color.Cyan);
            debugDraw.End();

            debugDraw.Dispose();
        } 
    }
}
