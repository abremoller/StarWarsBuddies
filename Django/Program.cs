// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;

Console.WriteLine("Hello, World!");


var _httpClient = new HttpClient();
Dictionary<string, List<string>> _films_names = new ();

bool containsDetailField = false;
string url = "https://swapi.dev/api/people";


while (!containsDetailField)
{
    var retrievedString = _httpClient.GetStringAsync(url).Result;
    JObject json = JObject.Parse(retrievedString);
    
    if (json.ContainsKey("detail"))
    {
        containsDetailField = true;
        break;
    }

    url = json["next"]?.ToString();

    var resultsNode = json["results"];
    var name = resultsNode[0]["name"];
    var filmsNode = resultsNode[0]["films"];

    ProcessResults(name, filmsNode);

    List<Tuple<string, string, string>> buddies = GetBuddies();

}

List<Tuple<string, string, string>> GetBuddies()
{
    SortActorNames();

    List<Tuple<string, string, string>> buddies = new ();

    //Go through list of actors by name, switching the list round and making sure the same actors are in all movies - Might have been easier the other way around, in hindsight


    foreach (var v in _films_names)
    {
        foreach(var compare in _films_names)
        {
            if (v.Value.SequenceEqual(compare.Value))
            {
                if (v.Value.Count() > 1)
                {
                    throw new Exception("Not supposed to happen");
                }

                if (v.Key != compare.Key)
                    buddies.Add(new Tuple<string, string, string>(v.Key, compare.Key, v.Value[0]));
            }
        }
    }

    return buddies;
}

void SortActorNames()
{
    foreach (var v in _films_names)
    {
        v.Value.Sort();
    }
}

void ProcessResults(JToken? name, JToken? filmsNode)
{

    foreach(var film in filmsNode)
    {
        string filmUrl = film.ToString();
        string characterName = name.ToString(); 

        if (!_films_names.ContainsKey(filmUrl))
            _films_names.Add(filmUrl, new List<string>());

        if (!_films_names[filmUrl].Contains(characterName))
            _films_names[filmUrl].Add(characterName);
    }
}

Console.WriteLine("Done, press any key to exit");
Console.ReadKey();

    