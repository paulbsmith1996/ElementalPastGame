using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElementalPastGame.KeyInput
{
    public class KeyEventPublisher : IKeyEventPublisher
    {
        internal List<IKeyEventSubscriber> subscribers = new();
        static KeyEventPublisher? _instance;

        public static KeyEventPublisher GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new KeyEventPublisher();
            return _instance;
        }

        public void AddIKeyEventSubscriber(IKeyEventSubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void RemoveIKeyEventSubscriber(IKeyEventSubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public void MakeExclusiveIKeySubscriber(IKeyEventSubscriber subscriber)
        {
            subscribers.Clear();
            subscribers.Add(subscriber);
        }

        public void PublishKeysDown(List<Keys> keyCodes)
        {
            foreach (IKeyEventSubscriber subscriber in this.subscribers)
            {
                subscriber.HandleKeysDown(keyCodes);
            }
        }

        public void PublishKeyPressed(char keyChar)
        {
            foreach (IKeyEventSubscriber subscriber in this.subscribers)
            {
                subscriber.HandleKeyPressed(keyChar);
            }
        }
    }
}
