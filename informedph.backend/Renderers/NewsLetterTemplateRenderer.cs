using informedph.backend.View_Models;
using RazorLight;

namespace informedph.backend.Renderers
{
    public class NewsletterTemplateRenderer
    {
        private readonly IRazorLightEngine _engine;

        public NewsletterTemplateRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "Templates"
                    )
                )
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderNewsletterTemplateAsync(
            NewsletterViewModel model)
        {
            return await _engine.CompileRenderAsync(
                "NewsLetterTemplate.cshtml",
                model
            );
        }
    }
}
