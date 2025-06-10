namespace SIGAD.BlazorApp.Pages
{
    public class PromotionStepDetailModel
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public List<PromotionItem> Items { get; set; }
    }
    public class PromotionItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } // text, file, button, textarea
    }
}
