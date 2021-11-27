using System;

namespace MidnightCommander.Components
{
    public interface IComponent
    {
        public bool IsActive { get; set; }

        public void HandleKey(ConsoleKeyInfo info);

        public void Draw();
    }
}
