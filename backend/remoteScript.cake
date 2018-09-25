// This script can be used to generate release notes out of a VSTS build.

#tool "nuget:?package=Mono.TextTransform"
#addin "Cake.Json"
#addin "Cake.Http"
#addin "Cake.FileHelpers"

var target = Argument("target", "Default");
var subscriptionId = Argument("subscriptionId", "2cc5132c-3f76-459f-8065-a3e844a18b20");
var username = Argument("username", "lef");
var password = Argument("password", "jcujesdjvgdmn7ncfmwj5dubjtloizent2ivhcntjgpk3rhkm6ra");
var language = Argument("language", "en");
var queriesPath = Argument("queriesPath", "Shared Queries/Products/Center/BackgroundSystem");
var productName = Argument("productName", "BackgroundSystem");
var productVersion = Argument("productVersion", "1.0");
var workingDirectory = Argument("workingDirectory", "work");
var outputPath = Argument("outputPath", string.Empty);
var aggregatedReleaseNotesTemplatePath = Argument("aggregatedReleaseNotesTemplatePath", "./templates/AggregatedReleaseNotes.md.tpl");

var releaseNotesTemplatePath = Argument("releaseNotesTemplate", "./templates/ReleaseNotes." + language + ".md.tpl");
var featureTemplatePath = Argument("featureTemplate", "./templates/ReleaseNotesFeature.md.tpl");
var pbiTemplatePath = Argument("pbiTemplate", "./templates/ReleaseNotesPBI.md.tpl");
var bugTemplatePath = Argument("bugTemplate", "./templates/ReleaseNotesBug.md.tpl");

Information("ProductName: " + productName);

if(string.IsNullOrEmpty(workingDirectory))
{
    workingDirectory = "./work";
}

var workingDirectoryPath = Directory(workingDirectory);
EnsureDirectoryExists(workingDirectoryPath);

var releaseNotesTemplateFile = File(releaseNotesTemplatePath);
var releaseNotesTemplate = FileReadText(releaseNotesTemplateFile.Path);

var featureTemplateFile = File(featureTemplatePath);
var featureTemplate = FileReadText(featureTemplateFile.Path);

var pbiTemplateFile = File(pbiTemplatePath);
var pbiTemplate = FileReadText(pbiTemplateFile.Path);

var bugTemplateFile = File(bugTemplatePath);
var bugTemplate = FileReadText(bugTemplateFile.Path);

using Newtonsoft.Json;

internal class Result
{
    [JsonProperty("workItems")]
    public WorkItem[] Items { get; set; }
}

internal class Query
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("children")]
    public Query[] Children { get; set; }

    [JsonProperty("_links")]
    public Links Links { get; set; }
}

internal class Links
{
    [JsonProperty("wiql")]
    public Wiql Wiql { get; set; }
}

internal class Wiql
{
    [JsonProperty("href")]
    public Uri Href { get; set; }
}

internal class WorkItem
{
    [JsonProperty("url")]
    public string Url { get; set; }
}

internal class Item
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("fields")]
    public Fields Fields { get; set; }

    [JsonProperty("relations")]
    public Relation[] Relations { get; set; }
}

internal class Relation
{
    [JsonProperty("rel")]
    public string Type { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

internal class Fields
{
    [JsonProperty("LTGScrum.ReleaseNotes")]
    public string ReleaseNotes { get; set; }
    
    [JsonProperty("LTGScrum.ReleaseNotesGerman")]
    public string ReleaseNotesGerman { get; set; }

    [JsonProperty("System.Title")]
    public string Title { get; set; }

    [JsonProperty("System.State")]
    public string State { get; set; }

    [JsonProperty("LTGScrum.TitleGerman")]
    public string TitleGerman { get; set; }

    [JsonProperty("System.WorkItemType")]
    public string WorkItemType { get; set; }
}

void AppendItem(
    StringBuilder stringBuilder,
    Item item, string language,
    string featureTemplate,
    string pbiTemplate,
    string bugTemplate)
{
    string title, releaseNotes;
    switch (language.ToString())
    {
        case "en":
            if (string.IsNullOrEmpty(item.Fields.ReleaseNotes))
            {
                return;
            }

            title = item.Fields.Title;
            releaseNotes = item.Fields.ReleaseNotes;
            break;
        case "de":
            if (string.IsNullOrEmpty(item.Fields.TitleGerman)
                || string.IsNullOrEmpty(item.Fields.ReleaseNotesGerman))
            {
                return;
            }

            title = item.Fields.TitleGerman;
            releaseNotes = item.Fields.ReleaseNotesGerman;
            break;
        default:
            throw new ArgumentOutOfRangeException("language");
    }

    string template;
    switch (item.Fields.WorkItemType)
    {
        case "Bug":
            template = bugTemplate;
            break;
        case "Product Backlog Item":
            template = pbiTemplate;
            break;
        case "Feature":
            template = featureTemplate;
            break;
        default:
            throw new ArgumentOutOfRangeException("item", "The item has an unsupported type (" + item.Fields.WorkItemType+ ")");
    }

    var rendered = TransformText(template)
        .WithToken("id", item.Id)
        .WithToken("title", title)
        .WithToken("releaseNotes", releaseNotes)
        .ToString();
    Information(rendered);
    stringBuilder.AppendLine();
    stringBuilder.AppendLine();

    stringBuilder.Append(rendered);
}

Task("AggregateReleaseNotes")
    .Does(() =>
    {
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = workingDirectoryPath.Path.FullPath + "/AggregatedReleaseNotes.md";
        }

        var outputFile = File(outputPath);
        var aggregatedPath = File(aggregatedReleaseNotesTemplatePath);

        var files = GetFiles(workingDirectoryPath.Path + "/*.md");
        var stringBuilder = new StringBuilder();
        foreach (var file in files)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.Append(FileReadText(file));
        }

        CopyFile(aggregatedPath, outputFile);
        FileAppendText(outputFile, stringBuilder.ToString());
    });

Task("ReleaseNotes")
    .Description("Getting related work items") 
    .Does(() =>
    {
        var queryName = "ReleaseNotes";
        var queriesAddress = 
                "https://ltg-dev.visualstudio.com/defaultcollection/LTG/_apis/wit/queries/" + queriesPath + "?$depth=1&api-version=2.2";

        var settings = new HttpSettings().UseBasicAuthorization(username, password);
        string responseBody2 = HttpGet(queriesAddress, settings);
        var queries = DeserializeJson<Query>(responseBody2);
        var query = queries.Children.FirstOrDefault(q => q.Name == productName + ".ReleaseNotes");
        Information("Getting query: " + query.Links.Wiql.Href);
        string responseBody = HttpGet(query.Links.Wiql.Href.ToString(), settings);
        var result = DeserializeJson<Result>(responseBody);
        var items = new Dictionary<int, Item>();
        foreach (var workItem in result.Items)
        {
            var body = HttpGet(workItem.Url + "?$expand=relations", settings);
            var item = DeserializeJson<Item>(body);
            Verbose(
                "Found item " + item.Id + " of type " + item.Fields.WorkItemType + " and in state "
                + item.Fields.State);
            Item parent = null;
            switch (item.Fields.WorkItemType)
            {
                case "Feature":
                    if (!items.ContainsKey(item.Id))
                    {
                        items.Add(item.Id, item);
                    }

                    break;
                case "Product Backlog Item":
                    foreach (var relation in item.Relations)
                    {
                        if (relation.Type == "System.LinkTypes.Hierarchy-Reverse")
                        {
                            var parentBody = HttpGet(relation.Url, settings);
                            parent = DeserializeJson<Item>(parentBody);
                            if (parent.Fields.State == "Done" && !string.IsNullOrEmpty(parent.Fields.ReleaseNotes))
                            {
                                if (!items.ContainsKey(parent.Id))
                                {
                                    items.Add(parent.Id, parent);
                                }

                                break;
                            }
                        }

                        if (!items.ContainsKey(item.Id))
                        {
                            items.Add(item.Id, item);
                        }
                    }

                    break;
                case "Bug":
                    Verbose("Adding Bug " + item.Id);
                    if (!items.ContainsKey(item.Id))
                    {
                        items.Add(item.Id, item);
                    }
                    
                    break;
                default:
                    break;
            }
        }

        var stringBuilder = new StringBuilder();
        if (items.Any())
        {
            foreach (var i in items.Values)
            {
                AppendItem(
                    stringBuilder,
                    i,
                    language,
                    featureTemplate,
                    pbiTemplate,
                    bugTemplate);
            }
        }
        else
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.Append("`No item available`");
        }

        var text = TransformText(releaseNotesTemplate)
            .WithToken("product", productName)
            .WithToken("version", productVersion)
            .WithToken("releaseNotes", stringBuilder.ToString())
            .ToString();
        var languageExtension = language == "en" ? string.Empty : "." + language;
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = workingDirectoryPath.Path.FullPath + "/ReleaseNotes" + languageExtension + ".md";
        }

        var releaseNotesFile = File(outputPath);
        FileWriteText(releaseNotesFile.Path, text);
        FileAppendText(releaseNotesFile.Path, stringBuilder.ToString());
    });

Task("Default")
    .IsDependentOn("ReleaseNotes");

RunTarget(target);