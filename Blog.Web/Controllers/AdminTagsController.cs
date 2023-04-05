﻿using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Controllers;
// /AdminTags
public class AdminTagsController : Controller
{
    //constructor creation + injection
    //private readonly BlogDbContext _blogDbContext;
    private readonly ITagInterface tagInterface;

    public AdminTagsController(ITagInterface tagRepository)//BlogDbContext blogDbContext)
    {
    //    _blogDbContext = blogDbContext;
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    [ActionName("Add")]
    public async Task<IActionResult> Add(AddTagRequest addTagRequest)
    {
        //read input elements (map AddTagRequest to Tag domain model
        var tag = new Tag
        {
            Name = addTagRequest.Name,
            DisplayName = addTagRequest.DisplayName,
        };

        //post in db
        //await _blogDbContext.Tags.AddAsync(tag);
        //await _blogDbContext.SaveChangesAsync();
        await tagInterface.AddAsync(tag);
        return RedirectToAction("List");
    }

    [HttpGet]
    [ActionName("List")]
    public async Task<IActionResult> List()
    {
        var tags = await tagInterface.GetAllAsync();

        return View(tags);
    }

    [HttpGet]
    [ActionName("Edit")]
    public async Task<IActionResult> Edit(Guid id)
    {
        //var tag = await _blogDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);

        var tag = await tagInterface.GetAsync(id);

        if (tag != null)
        {
            var editTagRequest = new EditTagRequest
            {
                Id = tag.Id,
                Name = tag.Name,
                DisplayName = tag.DisplayName
            };
            return View(editTagRequest);
        }
        return View(null);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
    {
        var tag = new Tag
        {
            Id = editTagRequest.Id,
            Name = editTagRequest.Name,
            DisplayName = editTagRequest.DisplayName,
        };
        //var existingTag = await _blogDbContext.Tags.FindAsync(tag.Id);

        //if (existingTag != null)
        //{
        //    existingTag.Name = tag.Name;
        //    existingTag.DisplayName = tag.DisplayName;

        //    await _blogDbContext.SaveChangesAsync();

        //    return RedirectToAction("List");
        //}

        var updatedTag = tagInterface.UpdateAsync(tag);

        if (updatedTag != null)
        {
            //show success
        }
        else
        {
            //show error
        }

        return RedirectToAction("Edit", new { id = editTagRequest.Id });
    }



    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
    {
        //var tag = await _blogDbContext.Tags.FindAsync(editTagRequest.Id);
        
        //if (tag != null)
        //{
        //    _blogDbContext.Tags.Remove(tag);
        //    await _blogDbContext.SaveChangesAsync();

        //    //show success notification
        //    return RedirectToAction("List");
        //}
        //show error notification
        var deletedTag = await tagInterface.DeleteAsync(editTagRequest.Id);

        if (deletedTag != null)
        {
            //success notification
            return RedirectToAction("List");
        }
        
           //fail notification    
        return RedirectToAction("Edit", new { id = editTagRequest.Id });
    }

}
