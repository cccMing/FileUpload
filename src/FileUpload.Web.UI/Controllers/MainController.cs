﻿using FileUpload.Controllers.Filters;
using FileUpload.Models;
using FileUpload.Services;
using FileUpload.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    [Route("")]
    [Route("{urltoken:regex([[a-zA-Z0-9]]+)}")]
    [TypeFilter(typeof(ProfileFilter))]
    public class MainController : Controller
    {
        private readonly FileService fileService;
        private readonly UploadSettings configuration;
        private readonly Factory factory;

        public MainController(FileService fileService, UploadSettings configuration, Factory factory)
        {
            Ensure.NotNull(fileService, "fileService");
            Ensure.NotNull(configuration, "configuration");
            Ensure.NotNull(factory, "factory");
            this.fileService = fileService;
            this.configuration = configuration;
            this.factory = factory;
        }

        public string AppVersion 
            => "v" + typeof(UploadOptions).Assembly.GetName().Version.ToString(3);

        [Route("")]
        public IActionResult Index() 
            => View(new IndexViewModel(AppVersion, factory.CreateUpload(), factory.CreateBrowser()));

        [HttpGet]
        [Route("browse")]
        public IActionResult Browse(bool noLayout)
        {
            BrowseViewModel model = factory.CreateBrowser();
            if (model == null)
                return NotFound();

            if (noLayout)
                return PartialView("_Browser", model);
            else
                return View(model);
        }

        [HttpGet]
        [Route("upload")]
        public IActionResult Upload()
        {
            return View(factory.CreateUpload());
        }

        [HttpPost]
        [Route("upload")]
        public async Task<StatusCodeResult> Upload(IFormFile file)
        {
            Ensure.NotNull(file, "file");
            bool isSuccess = await fileService.SaveAsync(configuration, file.FileName, file.Length, file.OpenReadStream()).ConfigureAwait(false);
            if (isSuccess)
                return Ok();

            return NotValidUpload();
        }

        [Route("{fileName}.{extension}")]
        [HttpGet]
        public IActionResult Download(string fileName, string extension)
        {
            Ensure.NotNull(fileName, "fileName");

            var content = fileService.FindContent(configuration, fileName, extension);
            if (content == null)
                return NotFound();

            return File(content.Value.Content, content.Value.ContentType);
        }

        [Route("delete")]
        public IActionResult Delete([FromServices] UrlBuilder urlBuilder, string fileName)
        {
            if (HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                fileService.Delete(configuration, fileName);
                return Redirect(urlBuilder.Index());
            }
            return View("error");
        }

        [HttpGet("/error")]
        public IActionResult Error() => View();

        private StatusCodeResult NotValidUpload() => BadRequest();
    }
}
