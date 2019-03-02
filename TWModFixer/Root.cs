namespace TWModFixer
{
    public class Mod
    {
        public string uuid { get; set; }
        public int order { get; set; }
        public bool active { get; set; }
        public string game { get; set; }
        public string packfile { get; set; }
        public string name { get; set; }
        public string _short { get; set; }
        public string category { get; set; }
        public bool owned { get; set; }
    }
}
