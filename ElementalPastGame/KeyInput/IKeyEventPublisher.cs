using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.KeyInput
{
    public interface IKeyEventPublisher
    {
        public static IKeyEventPublisher getInstance() => throw new NotImplementedException();
        public void PublishKeysDown(List<Keys> keyCodes);
        public void PublishKeyPressed(char keyChar);
        public void AddIKeyEventSubscriber(IKeyEventSubscriber subscriber);
        public void RemoveIKeyEventSubscriber(IKeyEventSubscriber subscriber);
        public void MakeExclusiveIKeySubscriber(IKeyEventSubscriber subscriber);

    }

    public interface IKeyEventSubscriber
    {
        public void HandleKeysDown(List<Keys> keyCodes);

        public void HandleKeyPressed(char keyChar);

    }
}
