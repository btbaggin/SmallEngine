using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine
{
    public static class Extensions
    {
        internal static void AddOrdered(this List<UI.UIElement> pList, UI.UIElement pElement)
        {
            var i = pList.BinarySearch(pElement);
            if (i < 0) i = ~i;
            pList.Insert(i, pElement);
        }

        #region Random
        public static float NextFloat(this Random r)
        {
            return (float)r.NextDouble();
        }

        public static float Range(this Random r, float min, float max)
        {
            return min + r.NextFloat() * (max - min);
        }

        public static double Range(this Random r, double min, double max)
        {
            return min + r.NextFloat() * (max - min);
        }
        #endregion

        #region IGameObject
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

        public static IGameObject Nearest(this IEnumerable<IGameObject> pGameObjects, IGameObject pGameObject, string pTag)
        {
            return NearestWithinDistance(pGameObjects, pGameObject, float.MaxValue, pTag);
        }

        public static IEnumerable<IGameObject> WithTag(this IEnumerable<IGameObject> pGameObjects, string pTag)
        {
            foreach(var go in pGameObjects)
            {
                if (pTag == go.Tag) yield return go;
            }
        }
        #endregion
    }
}
