
using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WorkplaceGroupAutomation.Models
{
        public partial class UserAccount
        {
            [JsonProperty("schemas")]
            public Schema[] Schemas { get; set; }

            [JsonProperty("totalResults")]
            public long TotalResults { get; set; }

            [JsonProperty("itemsPerPage")]
            public long ItemsPerPage { get; set; }

            [JsonProperty("startIndex")]
            public long StartIndex { get; set; }

            [JsonProperty("Resources")]
            public List<Resource> Resources { get; set; }
        }

        public partial class Resource
        {
            [JsonProperty("schemas")]
            public List<Schema> Schemas { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
            public string UserName { get; set; }

            [JsonProperty("name")]
            public Name Name { get; set; }

            [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
            public string Title { get; set; }

            [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
            public Locale? Locale { get; set; }

            [JsonProperty("active")]
            public bool Active { get; set; }

            [JsonProperty("addresses", NullValueHandling = NullValueHandling.Ignore)]
            public List<Address> Addresses { get; set; }

            [JsonProperty("urn:scim:schemas:extension:enterprise:1.0", NullValueHandling = NullValueHandling.Ignore)]
            public UrnScimSchemasExtensionEnterprise10 UrnScimSchemasExtensionEnterprise10 { get; set; }

            [JsonProperty("urn:scim:schemas:extension:facebook:accountstatusdetails:1.0")]
            public UrnScimSchemasExtensionFacebookAccountstatusdetails10 UrnScimSchemasExtensionFacebookAccountstatusdetails10 { get; set; }

            [JsonProperty("urn:scim:schemas:extension:facebook:auth_method:1.0")]
            public UrnScimSchemasExtensionFacebookAuthMethod10 UrnScimSchemasExtensionFacebookAuthMethod10 { get; set; }

            [JsonProperty("urn:scim:schemas:extension:facebook:starttermdates:1.0", NullValueHandling = NullValueHandling.Ignore)]
            public UrnScimSchemasExtensionFacebookStarttermdates10 UrnScimSchemasExtensionFacebookStarttermdates10 { get; set; }

            [JsonProperty("phoneNumbers", NullValueHandling = NullValueHandling.Ignore)]
            public List<PhoneNumber> PhoneNumbers { get; set; }

            [JsonProperty("externalId", NullValueHandling = NullValueHandling.Ignore)]
            public string ExternalId { get; set; }
        }

        public partial class Address
        {
            [JsonProperty("type")]
            public TypeEnum Type { get; set; }

            [JsonProperty("formatted")]
            public string Formatted { get; set; }

            [JsonProperty("primary")]
            public bool Primary { get; set; }
        }

        public partial class Name
        {
            [JsonProperty("formatted")]
            public string Formatted { get; set; }
            [JsonProperty("familyname")]
            public string FamilyName { get; set; }
            [JsonProperty("givenname")]
            public string GivenName { get; set; }
        }

        public partial class PhoneNumber
        {
            [JsonProperty("primary")]
            public bool Primary { get; set; }

            [JsonProperty("type")]
            public TypeEnum Type { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public partial class UrnScimSchemasExtensionEnterprise10
        {
            [JsonProperty("division", NullValueHandling = NullValueHandling.Ignore)]
            public string Division { get; set; }

            [JsonProperty("department", NullValueHandling = NullValueHandling.Ignore)]
            public string Department { get; set; }

            [JsonProperty("manager", NullValueHandling = NullValueHandling.Ignore)]
            public Manager Manager { get; set; }
        }

        public partial class Manager
        {
            [JsonProperty("managerId")]
            public long ManagerId { get; set; }
        }

        public partial class UrnScimSchemasExtensionFacebookAccountstatusdetails10
        {
            [JsonProperty("claimed")]
            public bool Claimed { get; set; }

            [JsonProperty("claimDate", NullValueHandling = NullValueHandling.Ignore)]
            public long? ClaimDate { get; set; }

            [JsonProperty("invited")]
            public bool Invited { get; set; }

            [JsonProperty("inviteDate")]
            public long InviteDate { get; set; }

            [JsonProperty("canDelete")]
            public bool CanDelete { get; set; }

            [JsonProperty("accessCode", NullValueHandling = NullValueHandling.Ignore)]
            public string AccessCode { get; set; }

            [JsonProperty("accessCodeExpirationDate", NullValueHandling = NullValueHandling.Ignore)]
            public long? AccessCodeExpirationDate { get; set; }

            [JsonProperty("claimLink", NullValueHandling = NullValueHandling.Ignore)]
            public Uri ClaimLink { get; set; }
        }

        public partial class UrnScimSchemasExtensionFacebookAuthMethod10
        {
            [JsonProperty("auth_method")]
            public AuthMethod AuthMethod { get; set; }
        }

        public partial class UrnScimSchemasExtensionFacebookStarttermdates10
        {
            [JsonProperty("startDate")]
            public long StartDate { get; set; }
        }

        public enum TypeEnum { Attributes, Work, Mobile };

        public enum Locale { EnGb, EnUs };

        public enum Schema { UrnScimSchemasCore10, UrnScimSchemasExtensionEnterprise10, UrnScimSchemasExtensionFacebookAccountstatusdetails10, UrnScimSchemasExtensionFacebookAuthMethod10, UrnScimSchemasExtensionFacebookStarttermdates10 };

        public enum AuthMethod { Password };

        public partial class UserAccount
        {
            public static UserAccount FromJson(string json) => JsonConvert.DeserializeObject<UserAccount>(json, Models.Converter.Settings);
        }

        public static class Serialize
        {
            public static string ToJson(this UserAccount self) => JsonConvert.SerializeObject(self, Models.Converter.Settings);
        }

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                TypeEnumConverter.Singleton,
                LocaleConverter.Singleton,
                SchemaConverter.Singleton,
                AuthMethodConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

        internal class TypeEnumConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "attributes":
                        return TypeEnum.Attributes;
                    case "work":
                        return TypeEnum.Work;
                    case "mobile":
                        return TypeEnum.Mobile;
            }
                throw new Exception("Cannot unmarshal type TypeEnum");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (TypeEnum)untypedValue;
                switch (value)
                {
                    case TypeEnum.Attributes:
                        serializer.Serialize(writer, "attributes");
                        return;
                    case TypeEnum.Work:
                        serializer.Serialize(writer, "work");
                        return;
                    case TypeEnum.Mobile:
                        serializer.Serialize(writer, "mobile");
                        return;
            }
                throw new Exception("Cannot marshal type TypeEnum");
            }

            public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
        }

        internal class LocaleConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(Locale) || t == typeof(Locale?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "en_GB":
                        return Locale.EnGb;
                    case "en_US":
                        return Locale.EnUs;
                }
                throw new Exception("Cannot unmarshal type Locale");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (Locale)untypedValue;
                switch (value)
                {
                    case Locale.EnGb:
                        serializer.Serialize(writer, "en_GB");
                        return;
                    case Locale.EnUs:
                        serializer.Serialize(writer, "en_US");
                        return;
                }
                throw new Exception("Cannot marshal type Locale");
            }

            public static readonly LocaleConverter Singleton = new LocaleConverter();
        }

        internal class SchemaConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(Schema) || t == typeof(Schema?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "urn:scim:schemas:core:1.0":
                        return Schema.UrnScimSchemasCore10;
                    case "urn:scim:schemas:extension:enterprise:1.0":
                        return Schema.UrnScimSchemasExtensionEnterprise10;
                    case "urn:scim:schemas:extension:facebook:accountstatusdetails:1.0":
                        return Schema.UrnScimSchemasExtensionFacebookAccountstatusdetails10;
                    case "urn:scim:schemas:extension:facebook:auth_method:1.0":
                        return Schema.UrnScimSchemasExtensionFacebookAuthMethod10;
                    case "urn:scim:schemas:extension:facebook:starttermdates:1.0":
                        return Schema.UrnScimSchemasExtensionFacebookStarttermdates10;
                }
                throw new Exception("Cannot unmarshal type Schema");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (Schema)untypedValue;
                switch (value)
                {
                    case Schema.UrnScimSchemasCore10:
                        serializer.Serialize(writer, "urn:scim:schemas:core:1.0");
                        return;
                    case Schema.UrnScimSchemasExtensionEnterprise10:
                        serializer.Serialize(writer, "urn:scim:schemas:extension:enterprise:1.0");
                        return;
                    case Schema.UrnScimSchemasExtensionFacebookAccountstatusdetails10:
                        serializer.Serialize(writer, "urn:scim:schemas:extension:facebook:accountstatusdetails:1.0");
                        return;
                    case Schema.UrnScimSchemasExtensionFacebookAuthMethod10:
                        serializer.Serialize(writer, "urn:scim:schemas:extension:facebook:auth_method:1.0");
                        return;
                    case Schema.UrnScimSchemasExtensionFacebookStarttermdates10:
                        serializer.Serialize(writer, "urn:scim:schemas:extension:facebook:starttermdates:1.0");
                        return;
                }
                throw new Exception("Cannot marshal type Schema");
            }

            public static readonly SchemaConverter Singleton = new SchemaConverter();
        }

        internal class AuthMethodConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(AuthMethod) || t == typeof(AuthMethod?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                if (value == "password")
                {
                    return AuthMethod.Password;
                }
                throw new Exception("Cannot unmarshal type AuthMethod");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (AuthMethod)untypedValue;
                if (value == AuthMethod.Password)
                {
                    serializer.Serialize(writer, "password");
                    return;
                }
                throw new Exception("Cannot marshal type AuthMethod");
            }

            public static readonly AuthMethodConverter Singleton = new AuthMethodConverter();
        }

}

