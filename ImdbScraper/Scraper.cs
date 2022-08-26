using System.Text.RegularExpressions;
using System.Web;

namespace ImdbScraper
{
    public class Scraper
    {
        public static async Task GetDataFromImdbAsync(Data d)
        {
            if (string.IsNullOrWhiteSpace(d.Id))
            {
                throw new ArgumentNullException("Id should not be null or empty.");
            }

            HttpClient client = new();

            string result;
            try
            {
                result = await client.GetStringAsync(new Uri("http://www.imdb.com/title/" + d.Id + "/"));
            }
            catch (Exception)
            {
                return;
            }

            string html = HttpUtility.HtmlDecode(result);

            // download image
            string imageUrl = Regex.Match(html, "<img src=\"(http://ia.media-imdb.com/images/.*?.jpg)\"\n     style=\"max-width:214px; max-height:317px;\"").Groups[1].Value;

            d.Cover = !string.IsNullOrWhiteSpace(imageUrl) ? await client.GetByteArrayAsync(new Uri(imageUrl)) : null;

            d.Name = GetName(html);
            d.Year = GetYear(html);
            d.Rating = GetRating(html);
            d.Genres = GetGenres(html);
            d.Directors = GetDirectors(html);
            d.Stars = GetStars(html);
            d.Writers = GetWriters(html);
            d.Storyline = GetStoryline(html);
            d.Runtime = GetRuntime(html);
            d.Metascore = GetMetascore(html);
        }

        private static string GetName(string html)
        {
            try
            {
                string name = Regex.Match(html, "<h1 class=\"header\" itemprop=\"name\">(.*?)<span class=\"nobr\">.*?</h1>", RegexOptions.Singleline).Groups[1].Value;
                return name.Replace("\n", "").Trim();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetYear(string html)
        {
            try
            {
                string Year = Regex.Match(html, "<a href=\"/year/.*?/\">" + "(.*?)" + "</a>").Groups[1].Value;
                return Year.Length != 4 ? "" : Year;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetRating(string html)
        {
            try
            {
                string ScoreString = Regex.Match(html, "<div class=\"star-box-giga-star\">" + "(.*?)" + "</div>", RegexOptions.Singleline).Groups[1].Value;
                return ScoreString.Replace("\n", "");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static List<string>? GetGenres(string html)
        {
            try
            {
                string tempString = Regex.Match(html, "<h4 class=\"inline\">Genres:</h4>(.*?)</div>", RegexOptions.Singleline).Groups[1].Value;
                MatchCollection mc = Regex.Matches(tempString, "<a .*? href=\"/genre/(.*?)\"   itemprop=\"genre\"\n    >.*?</a>");
                return mc.Select(m => m.Groups[1].Value).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<string>? GetDirectors(string html)
        {
            try
            {
                string tempString = Regex.Match(html, "<h4 class=\"inline\">\n    Director?:\n  </h4>\n.*?</div>", RegexOptions.Singleline).Value;
                MatchCollection mc = Regex.Matches(tempString, "<a .*? onclick=.*? href=\"/name/.*?/\" .*? itemprop=\"director\".*?>(.*?)</a>", RegexOptions.Singleline);
                return mc.Select(m => m.Groups[1].Value).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<string>? GetStars(string html)
        {
            try
            {
                string tempString = Regex.Match(html, "<h4 class=\"inline\">Stars:</h4>\n(.*?)\n</div>", RegexOptions.Singleline).Groups[1].Value;
                MatchCollection mc = Regex.Matches(tempString, "<a .*? onclick=.*? href=\"/name/.*?/\" .*? itemprop=\"actors\".*?>(.*?)</a>", RegexOptions.Singleline);
                return mc.Select(m => m.Groups[1].Value).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static List<string>? GetWriters(string html)
        {
            try
            {
                string tempString = Regex.Match(html, "<h4 class=\"inline\">.*? Writers:.*? </h4>(.*?)</div>", RegexOptions.Singleline).Groups[1].Value;
                MatchCollection mc = Regex.Matches(tempString, "<a .*? onclick=.*? href=\"/name/.*?/\".*?>(.*?)</a>", RegexOptions.Singleline);
                return mc.Select(m => m.Groups[1].Value).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetStoryline(string html)
        {
            try
            {
                string Storyline = Regex.Match(html, "<h2>Storyline</h2>\n\n<p>(.*?)\n").Groups[1].Value;
                return Storyline.Length > 1000 ? "" : Storyline;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetRuntime(string html)
        {
            try
            {
                string runtimeString = Regex.Match(html, "<h4 class=\"inline\">Runtime:</h4>.*?<time itemprop=\"duration\" datetime=\".*?\">(.*?) min</time>.*?</div>", RegexOptions.Singleline).Groups[1].Value;
                return runtimeString.Length > 3 ? "" : runtimeString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetMetascore(string html)
        {
            try
            {
                string metascoreString = Regex.Match(html, "Metascore: \n<a .*?><a href=\"criticreviews\">(.*?)/100</a>", RegexOptions.Singleline).Groups[1].Value;
                return metascoreString.Length > 3 ? "" : metascoreString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<string> GetIdFromNameAsync(string name)
        {
            try
            {
                HttpClient client = new();
                string result = await client.GetStringAsync("http://www.imdb.com/find?s=all&q=" + SearchFormat(name));

                string html = HttpUtility.HtmlDecode(result);

                if (Regex.IsMatch(html, "<b>No Matches.</b>"))
                {
                    return "";
                }

                string id = Regex.Match(html, "href=\"/title/(.*?)/\"").Groups[1].Value;
                return id.Length == 9 ? id : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string SearchFormat(string SearchString)
        {
            return SearchString
                .Replace(" ", "+")
                .Replace("ü", "%FC")
                .Replace("ö", "%F6")
                .Replace("å", "%E5")
                .Replace("ä", "%E4")
                .Replace("ç", "%E7")
                .Replace("Ç", "%C7")
                .Replace("é", "%E9")
                .Replace("è", "%E8")
                .Replace("ñ", "%F1")
                .Replace("ï", "%EF")
                .Replace("í", "%ED")
                .Replace("ô", "%F4")
                .Replace("&", "%26")
                .Replace("'", "%27");
        }
    }
}
