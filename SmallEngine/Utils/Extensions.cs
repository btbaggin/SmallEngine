using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public static class Extensions
    {
        public static float NextFloat(this Random r)
        {
            return (float)r.NextDouble();
        }

        public static float Range(this Random r, float min, float max)
        {
            return min + r.NextFloat() * (max - min);
        }

        public static IEnumerable<IGameObject> WithinDistance(this IEnumerable<IGameObject> pGameObjects, IGameObject pGameObject, float pDistance, string pTag)
        {
            return pGameObjects.Where(pGo => pGo.Tag == pTag && 
                                             pGo != pGameObject && 
                                             Vector2.Distance(pGo.Position, pGameObject.Position) <= pDistance);
        }

        public static IGameObject NearestWithinDistance(this IEnumerable<IGameObject> pGameObjects, IGameObject pGameObject, float pDistance, string pTag)
        {
            float bestDistance = pDistance;
            IGameObject closest = null;
            foreach(var go in pGameObjects.Where(pGo => pGo.Tag == pTag && pGo != pGameObject))
            {
                var d = Vector2.Distance(go.Position, pGameObject.Position);
                if (d < bestDistance)
                {
                    closest = go;
                    bestDistance = d; 
                }
            }
            return closest;
        }
    }
}
