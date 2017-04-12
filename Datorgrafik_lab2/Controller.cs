using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Datorgrafik_lab2
{
    public class Controller
    {
        private Dictionary<Keys, Vector3> bindings;

        private Vector3 dir;

        private float scaleMove;

        public Controller(float scaleMove)
        {
            bindings = new Dictionary<Keys, Vector3>();

            this.scaleMove = scaleMove;
        }

        public void AddBinding(Keys k, Vector3 direction)
        {
            if (!bindings.ContainsKey(k))
                bindings.Add(k, direction);
        }

        public Vector3 GetNextMove()
        {
            dir = Vector3.Zero;

            foreach (Keys k in Keyboard.GetState().GetPressedKeys())
            {
                if (bindings.ContainsKey(k))
                {
                    dir += scaleMove * bindings[k];
                }
            }

            return dir;
        }
    }
}