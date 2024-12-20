﻿using Grand.Infrastructure;
using Grand.Web.Common.Page;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Grand.Web.Common.TagHelpers;

[HtmlTargetElement("html", Attributes = ForAttributeName)]
public class HtmlTagHelper : TagHelper
{
    private const string ForAttributeName = "use-lang";

    private readonly IPageHeadBuilder _pageHeadBuilder;
    private readonly IWorkContext _workContext;

    public HtmlTagHelper(IWorkContext workContext, IPageHeadBuilder pageHeadBuilder)
    {
        _workContext = workContext;
        _pageHeadBuilder = pageHeadBuilder;
    }

    [HtmlAttributeName(ForAttributeName)] public bool UseLanguage { set; get; }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (UseLanguage)
        {
            output.Attributes.Add("lang", _workContext.WorkingLanguage.UniqueSeoCode);
            if (_workContext.WorkingLanguage.Rtl)
                output.Attributes.Add("dir", "rtl");
        }

        var classes = _pageHeadBuilder.GeneratePageCssClasses();
        if (string.IsNullOrEmpty(classes)) return Task.CompletedTask;
        if (output.Attributes.ContainsName("class"))
        {
            var attribute = output.Attributes["class"];
            output.Attributes.Remove(attribute);
            output.Attributes.Add("class", $"{attribute.Value} {classes}");
        }
        else
        {
            output.Attributes.Add("class", classes);
        }


        return Task.CompletedTask;
    }
}