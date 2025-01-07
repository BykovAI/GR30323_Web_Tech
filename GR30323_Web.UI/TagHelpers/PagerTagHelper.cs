﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace GR30323_Web.UI.TagHelpers
{
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        // номер текущей страницы 
        public int CurrentPage { get; set; }

        // общее количество страниц 
        public int TotalPages { get; set; }

        // имя категории объектов 
        public string? Category { get; set; }

        // признак страниц администратора 
        public bool Admin { get; set; } = false;

        // Номер предыдущей страницы 
        private int Prev => CurrentPage == 1 ? 1 : CurrentPage - 1;

        // Номер следующей страницы 
        private int Next => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "row");

            var nav = new TagBuilder("nav");
            nav.Attributes.Add("aria-label", "pagination");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            // Кнопка предыдущей страницы
            ul.InnerHtml.AppendHtml(CreateListItem(Category, Prev, "<span aria-hidden=\"true\">&laquo;</span>"));

            // Кнопки страниц
            for (var index = 1; index <= TotalPages; index++)
            {
                ul.InnerHtml.AppendHtml(CreateListItem(Category, index, string.Empty));
            }

            // Кнопка следующей страницы
            ul.InnerHtml.AppendHtml(CreateListItem(Category, Next, "<span aria-hidden=\"true\">&raquo;</span>"));
            nav.InnerHtml.AppendHtml(ul);

            output.Content.AppendHtml(nav);
        }

        /// <summary> 
        /// Разметка одной кнопки пейджера 
        /// </summary> 
        private TagBuilder CreateListItem(string? category, int pageNo, string? innerText)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (pageNo == CurrentPage && string.IsNullOrEmpty(innerText))
            {
                li.AddCssClass("active");
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            var routeData = new
            {
                pageno = pageNo,
                category = category
            };

            string url;

            // Для страниц администратора используется Razor Pages
            if (Admin)
            {
                url = _linkGenerator.GetPathByPage(_httpContextAccessor.HttpContext, page: "./Index", values: routeData);
            }
            else
            {
                url = _linkGenerator.GetPathByAction("Index", "Product", routeData);
            }

            a.Attributes.Add("href", url);

            var text = string.IsNullOrEmpty(innerText) ? pageNo.ToString() : innerText;
            a.InnerHtml.AppendHtml(text);

            li.InnerHtml.AppendHtml(a);

            return li;
        }
    }
}
