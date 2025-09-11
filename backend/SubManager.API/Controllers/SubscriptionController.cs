using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubManager.Application.Interfaces;
using System.Threading.Tasks;

namespace SubManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ISubscriptionService subscriptionService, ILogger<SubscriptionController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }



        [HttpGet]
        public async Task<ActionResult> GetSubscriptions([FromQuery] int pageNumber)
        {
            var subscriptions = await _subscriptionService.GetAllSubscryptionsAsync(pageNumber);
            return Ok(subscriptions);
        }

        //[HttpGet("{id}")]
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: SubscriptionController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: SubscriptionController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SubscriptionController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: SubscriptionController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: SubscriptionController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: SubscriptionController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
