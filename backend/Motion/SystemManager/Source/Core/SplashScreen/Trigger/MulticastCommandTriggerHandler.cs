namespace Gorba.Motion.SystemManager.Core.SplashScreen.Trigger
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;

    public class MulticastCommandTriggerHandler : TriggerHandlerBase
    {

        private SplashScreenHandler owner;

        private SplashScreenMessage splashScreenMessage;

        public override void Start(SplashScreenHandler splashScreenHandler)
        {
            this.owner = splashScreenHandler;
           // MessageDispatcher.Instance.Subscribe<SplashScreenMessage>(this.ShowSplashScreenHandler);

        }

        public override void Stop()
        {
            
        }
    }
}