// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetNext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetNext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Core.Activities
{
    using System.Activities;

    using Google.Transit.Realtime;

    public sealed class GetNext : CodeActivity<FeedMessage>
    {
        protected override FeedMessage Execute(CodeActivityContext context)
        {
            return SimulationManager.Current.GetNext();
        }
    }
}