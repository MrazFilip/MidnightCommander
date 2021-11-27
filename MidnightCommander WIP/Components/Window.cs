using System;
using System.Collections.Generic;

namespace MidnightCommander.Components
{
    public abstract class Window
    {
        // vyzkouší všechny HandleKey a Draw v IComponent
        protected List<IComponent> tools = new List<IComponent>();

        public virtual void HandleKey(ConsoleKeyInfo info)
        {
            foreach (IComponent item in tools)
            {
                item.HandleKey(info);
            }
        }

        public virtual void Draw()
        {
            foreach (IComponent item in tools)
            {
                item.Draw();
            }
        }
    }
}
