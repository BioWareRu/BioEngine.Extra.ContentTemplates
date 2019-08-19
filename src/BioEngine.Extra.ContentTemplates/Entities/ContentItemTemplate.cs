using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Core.Abstractions;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;

namespace BioEngine.Extra.ContentTemplates.Entities
{
    [Table("ContentItemTemplates")]
    [Entity("contentitemtemplate")]
    public class ContentItemTemplate : BaseEntity
    {
        [Required] public string Title { get; set; }
        [Required] public string ContentType { get; set; }
        [Required] public Guid[] SectionIds { get; set; } = new Guid[0];
        [Required] public Guid[] TagIds { get; set; } = new Guid[0];
        [Required] public virtual string AuthorId { get; set; }

        [Column(TypeName = "jsonb")]
        [Required]
        public ContentItemTemplateData Data { get; set; }

        [NotMapped] public IUser Author { get; set; }
    }

    public class ContentItemTemplateData
    {
        public List<ContentBlock> Blocks { get; set; } = new List<ContentBlock>();
        public string Url { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
    }
}
