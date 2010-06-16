/*
 * Created by Drew, 17/05/2010 11:15.
 */
using System;
using System.Xml;
using System.Collections.Generic;

namespace Drew.RoboCup
{
    /// <summary>
    /// A collection of extension methods for working with XML.
    /// </summary>
	internal static class XmlUtil {
	    public static string GetAttributeValue(this XmlNode node, string attributeName) {
	        if (node==null)
	            throw new ArgumentNullException("node");
	        if (attributeName==null)
	            throw new ArgumentNullException("attributeName");
	        var attribute = node.Attributes[attributeName];
	        if (attribute==null)
	            throw new Exception(string.Format("XML node with tag '{0}' does not contain the attribute '{1}'.", node.Name, attributeName));
	        return attribute.Value;
	    }
	    public static string GetAttributeValueOrDefault(this XmlNode node, string attributeName, string defaultValue) {
	        if (node==null)
	            throw new ArgumentNullException("node");
	        if (attributeName==null)
	            throw new ArgumentNullException("attributeName");
	        var attribute = node.Attributes[attributeName];
	        return attribute==null ? defaultValue : attribute.Value;
	    }
	}
}
