﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nimator.Settings
{
    public class NimatorSettings
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            Formatting = Formatting.Indented,
            Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() },
        };

        internal NimatorSettings()
        {
            this.Notifiers = new NotifierSettings[] { new ConsoleSettings() };
        }

        public NotifierSettings[] Notifiers { get; set; }

        public LayerSettings[] Layers { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, jsonSettings);
        }

        public static NimatorSettings FromJson(string json)
        {
            return JsonConvert.DeserializeObject<NimatorSettings>(json, jsonSettings);
        }

        public static NimatorSettings GetExample()
        {
            return new NimatorSettings
            {
                Notifiers = new NotifierSettings[] 
                { 
                    new ConsoleSettings(),
                    new OpsGenieSettings(),
                    new SlackSettings(),
                },
                Layers = new LayerSettings[]
                {
                    new LayerSettings
                    {
                        Name = "Layer 1",
                        Checks = new ICheckSettings[]
                        {
                            new NoopCheckSettings(),
                            new NoopCheckSettings(),
                        },
                    },
                    new LayerSettings
                    {
                        Name = "Layer 2",
                        Checks = new ICheckSettings[]
                        {
                            new NoopCheckSettings(),
                            new NoopCheckSettings(),
                        },
                    }
                },
            };
        }
    }
}