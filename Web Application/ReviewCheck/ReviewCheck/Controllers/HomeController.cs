﻿using Newtonsoft.Json.Linq;
using ReviewCheck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ReviewCheck.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Review(List<Review> reviews)
        {
            return View(reviews);
        }

        public ActionResult Analysis(List<Review> reviews)
        {
            return View(reviews);
        }

        [HttpPost]
        public ActionResult Analysis(FormCollection _collection)
        {

            List<Review> listReview = new List<Review>();
            if (_collection.AllKeys.Any(x=>x == "text"))
            {
                ViewData.Add(new KeyValuePair<string, object>("Columns", new List<string> { "Review", "Would recommend?", "Review Time" }));
                DatabaseContext context = new DatabaseContext();
                context.Reviews.Add(new Review()
                {
                    ID = context.Reviews.Count() + 1,
                    Text = _collection["text"],
                    IsPositive = (_collection["sentiment"] == "Yes!"),
                    SubmissionDate = DateTime.UtcNow,
                    URL = _collection["url"]
                });
                context.SaveChanges();
                listReview = context.Reviews.Local.Where(x=>x.URL == _collection["url"]).ToList();
                if (listReview.Count > 0)
                {
                    ViewData.Add(new KeyValuePair<string, object>("TotalPublishedReviews", listReview.Count));
                    ViewData.Add(new KeyValuePair<string, object>("TotalPositiveReviews", listReview.Count(x => x.IsPositive)));
                    ViewData.Add(new KeyValuePair<string, object>("TotalNegativeReviews", listReview.Count(x => !x.IsPositive)));
                }
            }
            else
            {
                ViewData.Add(new KeyValuePair<string, object>("Columns", new List<string> { "Review", "Deceptive?" }));
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://axesso-axesso-amazon-data-service-v1.p.rapidapi.com/amz/amazon-lookup-product?url=" + _collection["url"]),
                    Headers =
                    {
                        { "x-rapidapi-key", "a8072e1762mshba2e28f0e894148p1fae3fjsn2cf8e1e111b0" },
                        { "x-rapidapi-host", "axesso-axesso-amazon-data-service-v1.p.rapidapi.com" },
                    },
                };
                using (var response = client.SendAsync(request).Result)
                {
                    response.EnsureSuccessStatusCode();
                    string responseData = response.Content.ReadAsStringAsync().Result;

                    dynamic d = JObject.Parse(responseData);
                    dynamic reviews = d.reviews;
                    foreach (var reviewItem in reviews)
                    {
                        string reviewText = reviewItem.text;
                        var content = new StringContent(reviewText);
                        var apiResponse = client.PostAsync("http://127.0.0.1:5000/", content).Result;
                        var reviewResult = apiResponse.Content.ReadAsStringAsync().Result;
                        Review r = new Review() { Text = reviewText, Result = reviewResult, SubmissionDate = DateTime.MinValue };
                        listReview.Add(r);
                    }
                }
                if (listReview.Count > 0) {
                    ViewData.Add(new KeyValuePair<string, object>("TotalAnalysedReviews", listReview.Count));
                    ViewData.Add(new KeyValuePair<string, object>("TotalDeceptiveReviews", listReview.Count(x => x.Result == "Deceptive")));
                    ViewData.Add(new KeyValuePair<string, object>("TotalRealReviews", listReview.Count(x => x.Result != "Deceptive")));
                }
            }
            
            return Analysis(listReview);
        }
    }
}
