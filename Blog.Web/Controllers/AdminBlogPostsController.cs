﻿using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;// (selectListItem)

namespace Blog.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminBlogPostsController : Controller
{
    private readonly ITagRepository tagRepository;
    private readonly IBlogPostRepository blogPostRepository;

    public AdminBlogPostsController(ITagRepository tagRepository,IBlogPostRepository blogPostRepository)
    {
        this.tagRepository = tagRepository;
        this.blogPostRepository = blogPostRepository;
    }


    [HttpGet]
   public async Task<IActionResult> Add()
    {
        //get tags from repository
        var tags = await tagRepository.GetAllAsync();

        var model = new AddBlogPostRequest
        {
            Tags = tags.Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id.ToString() })
        };
            
        return View(model);

    }

    [HttpPost]
    [ActionName("Add")]
    public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
    {
        //mapear o view model para o domain model
        var blogPost = new BlogPost
        {
            Heading = addBlogPostRequest.Heading,
            PageTitle = addBlogPostRequest.PageTitle,
            Content = addBlogPostRequest.Content,
            ShortDescription = addBlogPostRequest.ShortDescription,
            FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
            UrlHandle = addBlogPostRequest.UrlHandle,
            PublishedDate = addBlogPostRequest.PublishedDate,
            Author = addBlogPostRequest.Author,
            Visible = addBlogPostRequest.Visible,
        };

        //mapear tags das tags selecionadas
        var selectedTags = new List<Tag>();
        foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
        {
            var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
            var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

            if (existingTag != null)
            {
                selectedTags.Add(existingTag);
            }
        }
        //mapear tags para o domain model
        blogPost.Tags = selectedTags;

        await blogPostRepository.AddAsync(blogPost);

        return RedirectToAction("List");
    }

    [HttpGet]
    [ActionName("List")]
    public async Task<IActionResult> List()
    {
        //chamar o repositorio pra pegar os dados do bd
        var blogPosts = await blogPostRepository.GetAllAsync();

        return View(blogPosts);
    }

    [HttpGet]
    [ActionName("Edit")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var blogPost = await blogPostRepository.GetAsync(id);
        var tagsDomainModel = await tagRepository.GetAllAsync();

        if (blogPost != null)
        {
        //map domain model into view model
        var model = new EditBlogPostRequest
            {
                Id = blogPost.Id,
                Heading = blogPost.Heading,
                Author = blogPost.Author,
                PageTitle = blogPost.PageTitle,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                ShortDescription = blogPost.ShortDescription,
                PublishedDate = blogPost.PublishedDate,
                Visible = blogPost.Visible,
                Tags = tagsDomainModel.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray(),
            };
            return View(model);
        }

        return View(null);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
    {
        //map viewmodel back to domainmodel
        var bpdm = new BlogPost
        {
            Id = editBlogPostRequest.Id,
            Heading = editBlogPostRequest.Heading,
            PageTitle = editBlogPostRequest.PageTitle,
            Content = editBlogPostRequest.Content,
            Author = editBlogPostRequest.Author,
            ShortDescription = editBlogPostRequest.ShortDescription,
            FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
            PublishedDate = editBlogPostRequest.PublishedDate,
            UrlHandle = editBlogPostRequest.UrlHandle,
            Visible = editBlogPostRequest.Visible
        };

        //map tags into domain model
        var selectedTags = new List<Tag>();

        foreach(var selectedTag in editBlogPostRequest.SelectedTags)
        {
            if (Guid.TryParse(selectedTag, out var tag))
            {
                var foundTag = await tagRepository.GetAsync(tag);

                if (foundTag != null)
                {
                    selectedTags.Add(foundTag);
                }
            }
        }

        bpdm.Tags = selectedTags;

        //submit info to repo to update
        var updatedBlog = await blogPostRepository.UpdateAsync(bpdm);

        if (updatedBlog != null)
        {
            return RedirectToAction("List");
        }
        return RedirectToAction("Edit");

    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
    {
        var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

        if (deletedBlogPost != null)
        {
            return RedirectToAction("List");
        }
        return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
    }
}
