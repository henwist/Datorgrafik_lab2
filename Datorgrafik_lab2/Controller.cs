using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Datorgrafik_lab2
{
    public class Controller
    {
        private Dictionary<Keys, Vector2> bindings;

        Vector2 dir;

        public Controller()
        {
            bindings = new Dictionary<Keys, Vector2>();
        }

        public void AddBinding(Keys k, Vector2 direction)
        {
            if (!bindings.ContainsKey(k))
                bindings.Add(k, direction);
        }

        public Vector2 GetNextMove()
        {
            dir = Vector2.Zero;

            foreach (Keys k in Keyboard.GetState().GetPressedKeys())
            {
                if (bindings.ContainsKey(k))
                {
                    dir += bindings[k];
                }
            }

            return dir;
        }
    }
}