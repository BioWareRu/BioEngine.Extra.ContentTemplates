using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Extra.ContentTemplates.Entities;
using Newtonsoft.Json;

namespace BioEngine.Extra.ContentTemplates.Db
{
    public class ContentItemTemplatesRepository : BioRepository<ContentItemTemplate>
    {
        private readonly BioEntitiesManager _entitiesManager;

        public ContentItemTemplatesRepository(BioRepositoryContext<ContentItemTemplate> repositoryContext,
            BioEntitiesManager entitiesManager) : base(
            repositoryContext)
        {
            _entitiesManager = entitiesManager;
        }

        public Task<(ContentItemTemplate[] items, int itemsCount)> GetTemplatesAsync<TContent>()
            where TContent : ContentItem
        {
            return GetAllAsync(
                templates => templates.Where(t => t.ContentType == _entitiesManager.GetKey<TContent>()));
        }

        public async Task<ContentItemTemplate> CreateTemplateAsync<TContent, TContentData>(TContent content)
            where TContent : ContentItem<TContentData> where TContentData : ITypedData, new()
        {
            var template = new ContentItemTemplate
            {
                Title = content.Title,
                AuthorId = content.AuthorId,
                Data = new ContentItemTemplateData
                {
                    Blocks = content.Blocks,
                    Title = content.Title,
                    Url = content.Url,
                    Data = JsonConvert.SerializeObject(content.Data)
                },
                ContentType = _entitiesManager.GetKey(content),
                TagIds = content.TagIds,
                SectionIds = content.SectionIds,
            };

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

            var key = _entitiesManager.GetKey<TContent>();
            if (template.ContentType != key)
            {
                throw new Exception(
                    $"Can't create {key} from template for {template.ContentType}");
            }

            var content = Activator.CreateInstance<TContent>();
            content.Blocks = new List<ContentBlock>();
            foreach (var contentBlock in template.Data.Blocks)
            {
                contentBlock.Id = Guid.NewGuid();
                contentBlock.ContentId = Guid.Empty;
                content.Blocks.Add(contentBlock);
            }

            content.Url = template.Data.Url;
            content.Title = template.Data.Title;
            content.TagIds = template.TagIds;
            content.SectionIds = template.SectionIds;
            content.Data = JsonConvert.DeserializeObject<TContentData>(template.Data.Data);
            content.DateAdded = DateTimeOffset.UtcNow;
            content.DateUpdated = DateTimeOffset.UtcNow;

            return content;
        }
    }
}
