namespace MAUIAndroidReleaseTool.Models
{
    public class SelectItem
    {
        public SelectItem(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; set; }
        public string Value { get; set; }
    }
}
