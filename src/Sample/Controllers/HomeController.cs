using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Sample.Models;

namespace Sample.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IJobService _jobService;
        public HomeController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]        
        public async Task<IActionResult> Index(long? id)
        {
            var model = new SampleViewModel { History = await _jobService.GetAsync(null) };
            if (id.HasValue == false)
                return View(model);

            model.Current = await _jobService.FindAsync(id.Value);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Run([FromForm]RunViewModel model)
        {
            if (ModelState.IsValid == false)
                return RedirectToAction("Index");

            var id = await _jobService.ExclusiveCreateAsync(model.Name, new JObject());
            if (id.HasValue == false)
                return StatusCode((int)HttpStatusCode.Conflict);

            return RedirectToAction("Index",new { id });
        }

        [HttpPost]
        public IActionResult Cancel([FromForm]CancelViewModel model)
        {
            if (ModelState.IsValid == false)
                return RedirectToAction("Index");

            _jobService.Cancel(model.Id);

            return RedirectToAction("Index",new { id = model.Id });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
