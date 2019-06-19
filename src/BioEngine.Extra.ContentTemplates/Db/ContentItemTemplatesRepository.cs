using System;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Extra.ContentTemplates.Entities;

namespace BioEngine.Extra.ContentTemplates.Db
{
    public class ContentItemTemplatesRepository : BioRepository<ContentItemTemplate>
    {
        public ContentItemTemplatesRepository(BioRepositoryContext<ContentItemTemplate> repositoryContext) : base(
            repositoryContext)
        {
        }

        public Task<(ContentItemTemplate[] items, int itemsCount)> GetTemplatesAsync<TContent>()
            where TContent : ContentItem
        {
            return GetAllAsync(
                templates => templates.Where(t => t.ContentType == typeof(TContent).FullName));
        }

        public async Task<ContentItemTemplate> CreateTemplateAsync<TContent, TContentData>(TContent content)
            where TContent : ContentItem<TContentData> where TContentData : ITypedData, new()
        {
            var template = new ContentItemTemplate {Title = content.Title, AuthorId = content.AuthorId,};
            template.SetItem<TContent, TContentData>(content);

            var result = await AddAsync(template);
            if (!result.IsSuccess)
            {
                throw new Exception(result.ErrorsString);
            }

            return template;
        }

        public async Task<TContent> CreateFromTemplateAsync<TContent, TContentData>(Guid templateId)
            where TContent : ContentItem<TContentData> where TContentData : ITypedData, new()
        {
            var template = await GetByIdAsync(templateId);
            if (template == null)
            {
                throw new ArgumentException(nameof(template));
            }

            if (template.ContentType != typeof(TContent).FullName)
            {
                throw new Exception(
                    $"Can't create {typeof(TContent).FullName} from template for {template.ContentType}");
            }

            var content = template.GetItem<TContent, TContentData>();
            content.DateAdded = DateTimeOffset.UtcNow;
            content.DateUpdated = DateTimeOffset.UtcNow;

            return content;
        }
    }
}
