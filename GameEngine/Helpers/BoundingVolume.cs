using CollisionSample;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace GameEngine.Helpers
{
    public static class BoundingVolume
    {
        public static BoundingVolumeComponent GetBoundingBoxVolume(VertexPositionNormalTexture[] vertices)
        {
            BoundingVolumeComponent bvCmp = new BoundingVolumeComponent();
            bvCmp.bbox = BoundingBox.CreateFromPoints(vertices.Select(x => x.Position));

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
