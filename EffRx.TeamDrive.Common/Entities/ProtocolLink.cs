namespace EffRx.TeamDrive.Common.Entities
{
    public class ProtocolLink
    {
        public string PathTranslated { get; }
        public string SpaceFromLink { get; }
        public string LinkWithoutProtocolPrefix { get; }
        public string LinkWithSpaceCharacters { get; }
        public string LinkFromCommandLineArgs { get; }

        public ProtocolLink(string[] args, string uriScheme)
        {
            LinkFromCommandLineArgs = args[0];
            LinkWithSpaceCharacters = args[0].Replace("%20", " ");
            LinkWithoutProtocolPrefix = GetLinkWithoutProtcolPrefix(args, uriScheme);
            SpaceFromLink = GetSpaceFromLink(args, uriScheme);


            PathTranslated = GetPathTranslated(args);

        }

        private string GetLinkWithoutProtcolPrefix(string[] args, string uriScheme)
        {
            return args[0].ToLower().Replace(uriScheme.ToLower() + ":", "").Replace("%20", " ");
        }

        private string GetPathTranslated(string[] args)
        {
            string str = args[0].Replace("%20", " ");
            
            return str;
        }

        private string GetSpaceFromLink(string[] args, string uriScheme)
        {
            string tempStr = args[0].ToLower().Replace(uriScheme.ToLower() + ":", "").Replace("%20", " ");
            return tempStr.Substring(0, tempStr.IndexOf(@"/"));
        }
    }
}
