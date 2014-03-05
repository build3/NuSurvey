﻿

using System;
using System.Linq;
using System.Web.Mvc;
using NuSurvey.Web.Controllers.Filters;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace NuSurvey.Web.Controllers
{
    /// <summary>
    /// Controller for the Educator class
    /// </summary>
    [Authorize]
    public class EducatorController : ApplicationController
    {

        //
        // GET: /Educator/
        [User]
        public ActionResult Index()
        {          
            return View();
        }


    }

}
