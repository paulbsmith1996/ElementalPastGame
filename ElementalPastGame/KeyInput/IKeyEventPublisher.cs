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
        public void PublishPressedKeys(List<Keys> keyCodes);
        public void AddIKeyEventSubscriber(IKeyEventSubscriber subscriber);
        public void RemoveIKeyEventSubscriber(IKeyEventSubscriber subscriber);
        public void MakeExclusiveIKeySubscriber(IKeyEventSubscriber subscriber);

    }

    public interface IKeyEventSubscriber
    {
        public void HandlePressedKeys(List<Keys> keyCodes);

    }
}
