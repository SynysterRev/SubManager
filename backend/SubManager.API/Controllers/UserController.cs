//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SubManager.Application.Interfaces;

//namespace SubManager.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class UserController : Controller
//    {
//        private readonly ISubscriptionService _subscriptionService;
//        private readonly ILogger<SubscriptionController> _logger;

//        public UserController(ISubscriptionService subscriptionService, ILogger<SubscriptionController> logger)
//        {
//            _subscriptionService = subscriptionService;
//            _logger = logger;
//        }
//        // GET: UserController
//        public ActionResult Index()
//        {
//            return View();
//        }

//        // GET: UserController/Details/5
//        public ActionResult Details(int id)
//        {
//            return View();
//        }

//        // GET: UserController/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: UserController/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(IFormCollection collection)
//        {
//            try
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: UserController/Edit/5
//        public ActionResult Edit(int id)
//        {
//            return View();
//        }

//        // POST: UserController/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(int id, IFormCollection collection)
//        {
//            try
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: UserController/Delete/5
//        public ActionResult Delete(int id)
//        {
//            return View();
//        }

//        // POST: UserController/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(int id, IFormCollection collection)
//        {
//            try
//            {
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }
//    }
//}
