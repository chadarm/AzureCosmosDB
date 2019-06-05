using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;


public class Entity
{
    public class FloatEntity : Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string Index { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public string Owner { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        // Define the Id and Value
        public FloatEntity(int index, float floatValue, string owner)
        {
            this.Index = index.ToString();
            this.Owner = owner;
            this.Value = floatValue.ToString();
        }
    }
}
