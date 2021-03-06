﻿using System.Collections.Generic;

namespace DynamicTranslator.Application.Yandex.Orchestration
{
    public class YandexDetectResponse
    {
        public string Code { get; set; }

        public string Lang { get; set; }

        public IList<string> Text { get; set; }
    }
}
