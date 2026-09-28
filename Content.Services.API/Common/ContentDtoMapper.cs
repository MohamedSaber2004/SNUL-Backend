using System.Linq.Expressions;
using SNUL.Shared.Common.DTOs.Content;
using DocumentEntity = SNUL.Shared.Domain.Models.Document;
using LandingPageEntity = SNUL.Shared.Domain.Models.LandingPage;
using HelpCategoryEntity = SNUL.Shared.Domain.Models.HelpCategory;
using HelpArticleEntity = SNUL.Shared.Domain.Models.HelpArticle;
using FAQEntity = SNUL.Shared.Domain.Models.FAQItem;
using SupportTicketEntity = SNUL.Shared.Domain.Models.SupportTicket;
using SupportContactEntity = SNUL.Shared.Domain.Models.SupportContact;
using HelpSiteStatEntity = SNUL.Shared.Domain.Models.HelpSiteStat;
using SiteLogoEntity = SNUL.Shared.Domain.Models.SiteLogo;

namespace Content.Services.API.Common
{
    internal static class ContentDtoMapper
    {
        public static Expression<Func<DocumentEntity, DocumentDto>> DocumentProjection => d => new DocumentDto
        {
            Id = d.Id,
            Title = d.Title,
            DocType = d.DocType,
            FileUrl = d.FileUrl,
            FileSizeKB = d.FileSizeKB,
            ProductId = d.ProductId,
            PublishedDate = d.PublishedDate,
            CreatedAt = d.CreatedAt
        };

        public static Expression<Func<LandingPageEntity, LandingPageDto>> LandingPageProjection => l => new LandingPageDto
        {
            Id = l.Id,
            Type = l.Type,
            Slug = l.Slug,
            HeroTitle = l.HeroTitle,
            HeroBody = l.HeroBody,
            ContentBlock = l.ContentBlock,
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt
        };

        public static Expression<Func<HelpCategoryEntity, HelpCategoryDto>> HelpCategoryProjection => c => new HelpCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            NameAr = c.NameAr,
            Icon = c.Icon,
            ArticleCount = c.Articles.Count(a => !a.IsDeleted),
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        public static Expression<Func<HelpArticleEntity, HelpArticleDto>> HelpArticleProjection => a => new HelpArticleDto
        {
            Id = a.Id,
            CategoryId = a.CategoryId,
            CategoryName = a.Category != null ? a.Category.Name : string.Empty,
            Title = a.Title,
            TitleAr = a.TitleAr,
            Body = a.Body,
            BodyAr = a.BodyAr,
            Slug = a.Slug,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt
        };

        public static Expression<Func<FAQEntity, FAQItemDto>> FAQProjection => f => new FAQItemDto
        {
            Id = f.Id,
            Question = f.Question,
            QuestionAr = f.QuestionAr,
            Answer = f.Answer,
            AnswerAr = f.AnswerAr,
            SortOrder = f.SortOrder,
            IsActive = f.IsActive,
            CreatedAt = f.CreatedAt
        };

        public static Expression<Func<SupportTicketEntity, SupportTicketDto>> SupportTicketProjection => t => new SupportTicketDto
        {
            Id = t.Id,
            UserId = t.UserId,
            Subject = t.Subject,
            Message = t.Message,
            Status = t.Status,
            Reply = t.Reply,
            CreatedAt = t.CreatedAt,
            RepliedAt = t.RepliedAt
        };

        public static Expression<Func<SupportContactEntity, SupportContactDto>> SupportContactProjection => c => new SupportContactDto
        {
            Id = c.Id,
            SupportEmail = c.SupportEmail,
            PhoneNumber = c.PhoneNumber,
            WhatsAppNumber = c.WhatsAppNumber,
            WorkingHours = c.WorkingHours,
            UpdatedAt = c.UpdatedAt ?? c.CreatedAt
        };

        public static Expression<Func<HelpSiteStatEntity, HelpSiteStatDto>> HelpSiteStatProjection => s => new HelpSiteStatDto
        {
            Id = s.Id,
            StatKey = s.StatKey,
            Value = s.Value,
            Label = s.Label,
            LabelAr = s.LabelAr,
            SortOrder = s.SortOrder,
            IsVisible = s.IsVisible
        };

        public static Expression<Func<SiteLogoEntity, SiteLogoDto>> SiteLogoProjection => s => new SiteLogoDto
        {
            Id = s.Id,
            LogoUrl = s.LogoUrl,
            AltText = s.AltText,
            UpdatedAt = s.UpdatedAt ?? s.CreatedAt
        };
    }
}
