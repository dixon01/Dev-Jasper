using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gorba.Center.Common.ServiceModel.Membership;
using Newtonsoft.Json;

namespace WebApplication.Controllers
{
    public class TenantController : Controller
    {
        // GET: Tenant
        public async Task<ActionResult> Index()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:9191/api/");
                    var response = await httpClient.GetAsync("tenants");
                    var content = await response.Content.ReadAsStringAsync();
                    var tenants = JsonConvert.DeserializeObject<Tenant[]>(content);
                    return this.View(tenants);
                }
            }
            catch (Exception)
            {
                return this.View("Error");
            }
        }

        // GET: Tenant/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("http://localhost:9191/api/");
                    var response = await httpClient.GetAsync("tenants/" + id);
                    var content = await response.Content.ReadAsStringAsync();
                    var tenant = JsonConvert.DeserializeObject<Tenant>(content);
                    return this.View(tenant);
                }
            }
            catch (Exception)
            {
                return this.View("Error");
            }
        }

        // GET: Tenant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tenant/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tenant/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tenant/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tenant/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tenant/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
