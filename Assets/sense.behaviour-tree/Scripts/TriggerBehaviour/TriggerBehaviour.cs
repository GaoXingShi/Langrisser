using UnityEngine;

namespace Sense.BehaviourTree
{
    public class TriggerBehaviour : MonoBehaviour
    {
        protected bool running;

        public virtual bool IsRunning
        {
             get { return running; }
        }

        public virtual void EnableTrigger()
        {
            running = true;
        }

        public virtual void DisableTrigger()
        {
            running = false;
        }
    }
}