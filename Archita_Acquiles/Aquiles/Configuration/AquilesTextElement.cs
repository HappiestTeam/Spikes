﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Aquiles.Configuration
{
    /// <summary>
    /// ConfigurationElement to hold text values
    /// </summary>
    public class AquilesTextElement : ConfigurationElement
    {
        private string text;
        
        /// <summary>
        /// get or set the Text (innerText) value.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        /// <summary>
        /// HACK
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="serializeCollectionKey"></param>
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            this.text = reader.ReadElementContentAsString();
        }
    }
}
