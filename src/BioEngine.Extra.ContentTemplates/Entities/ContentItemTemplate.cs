using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Abstractions;
using BioEngine.Core.Entities;
using Newtonsoft.Json;

namespace BioEngine.Extra.ContentTemplates.Entities
{
    [Table("ContentItemTemplates")]
    public class ContentItemTemplate : BaseEntity
    {
        [Required] public string ContentType { get; set; }
        [Required] public Guid[] SectionIds { get; set; } = new Guid[0];
        [Required] public Guid[] TagIds { get; set; } = new Guid[0];
        [Required] public virtual int AuthorId { get; set; }

        [Column(TypeName = "jsonb")]
        [Required]
        public ContentItemTemplateData Data { get; set; }

        [NotMapped] public IUser Author { get; set; }

        public TContentItem GetItem<TContentItem, TContentData>() where TContentItem : ContentItem<TContentData>
            where TContentData : ITypedData, new()
        {
            var content = Activator.CreateInstance<TContentItem>();
            content.Blocks = new List<ContentBlock>();
            foreach (var contentBlock in Data.Blocks)
            {
                contentBlock.Id = Guid.NewGuid();
                contentBlock.ContentId = Guid.Empty;
                content.Blocks.Add(contentBlock);
            }

            content.Url = Data.Url;
            content.Title = Data.Title;
            content.TagIds = TagIds;
            content.SectionIds = SectionIds;
            content.Data = JsonConvert.DeserializeObject<TContentData>(Data.Data);
            return content;
        }

        public void SetItem<TContentItem, TContentData>(TContentItem item)
            where TContentItem : ContentItem<TContentData> where TContentData : ITypedData, new()
        {
            Data = new ContentItemTemplateData
            {
                Blocks = item.Blocks,
                Title = item.Title,
                Url = item.Url,
                Data = JsonConvert.SerializeObject(item.Data)
            };
            ContentType = item.GetType().FullName;
            TagIds = item.TagIds;
            SectionIds = item.SectionIds;
        }
    }

    public class ContentItemTemplateData
    {
        public List<ContentBlock> Blocks { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
    }
}
