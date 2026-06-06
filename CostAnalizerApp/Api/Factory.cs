using System;
using System.Collections.Generic;
using System.Text;

namespace CostAnalizerApp.Api
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class Factory
    {

        /// <summary>
        /// Entry point for the application logic
        /// </summary>
        /// <returns>Return Main BL object</returns>
        public static CostAnalizerApplication GetApplication()
        {
            return new CostAnalizerApplication();
        }
    }
}
