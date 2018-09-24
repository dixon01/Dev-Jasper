// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The base view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.Common.MVVM.ViewModelHelpers
{

    #region

    

    #endregion

   /// <summary>The base view model.</summary>
   public class BaseViewModel : NotificationObject
   {
      public bool IsInDesignMode
      {
         get
         {
             return System.Reflection.Assembly.GetExecutingAssembly().Location.Contains("VisualStudio") ;
                    //&& System.ComponentModel.DesignTimeVisibleAttribute. GetIsInDesignMode(new DependencyObject()); 
         }
      }     
   }
}