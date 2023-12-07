using System.Text.Json.Serialization;

namespace DromAutoTrader.Models
{
    public class ParsedContact
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class ContactsControlValue
    {
        public string? Email { get; set; }
        public bool IsEmailHidden { get; set; }
        public string? contactInfo { get; set; }
    }

    public class Contacts
    {
        public ContactsControlValue? ControlValue { get; set; }
        public ParsedContact[]? parsedContacts { get; set; }
    }
    public class Field
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("controlValue")]
        public object? ControlValue { get; set; }

        [JsonPropertyName("currentCurrency")]
        public string? CurrentCurrency { get; set; }

        [JsonPropertyName("parsedContacts")]
        public List<ParsedContact>? ParsedContacts { get; set; }
    }

    public class PayLoad
    {
        [JsonPropertyName("addingType")]
        public string? AddingType { get; set; }

        [JsonPropertyName("directoryId")]
        public int DirectoryId { get; set; }

        [JsonPropertyName("fields")]
        public Dictionary<string, object>? Fields { get; set; }

        [JsonPropertyName("images")]
        public Images? images { get; set; }

        [JsonIgnore]
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }


    public class DromResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("isPublished")]
        public bool isPublished { get; set; }

        [JsonPropertyName("isDraft")]
        public bool isDraft { get; set; }
    }



    public class Images
    {
        [JsonPropertyName("images")]
        public List<long>? images { get; set; }

        [JsonPropertyName("isShowCompanyLogo")]
        public bool isShowCompanyLogo { get; set; }

        [JsonPropertyName("masterImageId")]
        public long masterImageId { get; set; }


    }
}
