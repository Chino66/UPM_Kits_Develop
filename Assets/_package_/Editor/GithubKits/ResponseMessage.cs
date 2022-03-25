namespace GithubKits
{
    public class ResponseMessage
    {
        public string Message;
        public string DocumentationUrl;

        public override string ToString()
        {
            return $"Message:{Message}, DocumentationUrl:{DocumentationUrl}";
        }
    }
}