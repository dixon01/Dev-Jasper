namespace Core
{
    using System;
    using System.Globalization;

    using Google.Transit.Realtime;

    public abstract class SimulationManager
    {
        static SimulationManager()
        {
            Reset();
        }

        public static SimulationManager Current { get; private set; }

        public static void Reset()
        {
            Set(DefaultSimulationManager.Instance);
        }

        public static void Set(SimulationManager instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        public abstract void Initialize(FeedMessage message);

        public abstract FeedMessage GetNext();

        internal sealed class DefaultSimulationManager : SimulationManager
        {
            private static readonly Lazy<DefaultSimulationManager> LazyInstance =
                new Lazy<DefaultSimulationManager>(CreateInstance);

            private int id;

            private FeedMessage message;

            internal DefaultSimulationManager()
            {
            }

            public static DefaultSimulationManager Instance
            {
                get
                {
                    return LazyInstance.Value;
                }
            }

            public override void Initialize(FeedMessage message)
            {
                this.message = message;
                this.GetNext();
            }

            public override FeedMessage GetNext()
            {
                for (var i = 0; i < message.entity.Count; i++)
                {
                    message.entity[i].id = this.GetId(i);
                }

                this.id++;
                return this.message;
            }

            private static DefaultSimulationManager CreateInstance()
            {
                return new DefaultSimulationManager();
            }

            private string GetId(int i)
            {
                return ((this.message.entity.Count * this.id) + i + 1).ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}