using Datorgrafik_lab2.CreateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datorgrafik_lab2.InstanceContainers
{
    public class InstanceBodyParts
    {
        Dictionary<string, Cube> bodyParts;

        public InstanceBodyParts()
        {
            bodyParts = new Dictionary<string, Cube>();
        }

        public void AddBodyPart(string key, Cube bodyPart)
        {
            if (!bodyParts.ContainsKey(key))
                bodyParts.Add(key, bodyPart);
        }

        public Cube GetBodyPart(string key)
        {
            if (bodyParts.ContainsKey(key))
                return bodyParts[key];

            return null;
        }
    }
}
