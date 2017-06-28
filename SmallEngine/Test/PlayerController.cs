using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEngine.Test
{
    class PlayerController : Component
    {
        public void MoveDown()
        {
            GameObject.Position = new Vector2(GameObject.Position.X, GameObject.Position.Y + 1);
        }

        public void MoveUp()
        {
            GameObject.Position = new Vector2(GameObject.Position.X, GameObject.Position.Y - 1);
        }

        public void MoveLeft()
        {
            GameObject.Position = new Vector2(GameObject.Position.X - 1, GameObject.Position.Y);
        }

        public void MoveRight()
        {
            GameObject.Position = new Vector2(GameObject.Position.X + 1, GameObject.Position.Y);
        }
    }
}
