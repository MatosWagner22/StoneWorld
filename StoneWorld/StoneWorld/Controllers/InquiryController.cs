using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneWorld_DataAccess;
using StoneWorld_DataAccess.Repository.IRepository;
using StoneWorld_Models;
using StoneWorld_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneWorld.Controllers
{

    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;

        public InquiryController(IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo)
        {
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
