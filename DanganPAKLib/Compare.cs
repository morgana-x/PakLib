using System.Text.RegularExpressions;

namespace DanganPAKLib
{
    public class CustomComparer : IComparer<string>
    {
        Regex r = new Regex(@"(\[.*?\])");
        public int Compare(string x, string y)
        {
            x = Path.GetFileNameWithoutExtension(x);
            y = Path.GetFileNameWithoutExtension(y);
            
            if (x.Contains("[") && x.Contains("]"))
                x = r.Match(x).Value.Replace("[","").Replace("]","");
            if (y.Contains("[") && y.Contains("]"))
                y = r.Match(y).Value.Replace("[", "").Replace("]", "");
            return int.Parse(x).CompareTo(int.Parse(y));
        }
    }
}
