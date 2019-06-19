using BioEngine.Core.DB;
using BioEngine.Core.Modules;
using BioEngine.Extra.ContentTemplates.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BioEngine.Extra.ContentTemplates
{
    public class ContentItemTemplatesModule : BaseBioEngineModule
    {
        public override void ConfigureEntities(IServiceCollection serviceCollection, BioEntitiesManager entitiesManager)
        {
            base.ConfigureEntities(serviceCollection, entitiesManager);
            entitiesManager.ConfigureDbContext(builder =>
            {
                builder.Entity<ContentItemTemplate>().Property(c => c.Data).HasConversion(
                    d => JsonConvert.SerializeObject(d,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            TypeNameHandling = TypeNameHandling.Auto
                        }),
                    j => JsonConvert.DeserializeObject<ContentItemTemplateData>(j,
                        new JsonSerializerSettings
                        {
                            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                            TypeNameHandling = TypeNameHandling.Auto
                        }));
            });
        }
    }
}
