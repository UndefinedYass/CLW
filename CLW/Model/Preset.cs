using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;

using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.WebSockets;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml;

using System.Security.Policy;
using System.Dynamic;
using System.Xml.Serialization;
using System.Windows.Documents;
using System.Windows.Input;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using System.Collections.ObjectModel;
using CLW.Model;
using System.Threading;
using CLW.Model.Enums;
using AngleSharp.Common;

namespace CLW.Model
{



    /// <summary>
    /// abbreviation for full name: CLW Preset Element
    /// base class for all CPE elements, i,e what makes up the preset (file/object/definition)
    /// </summary>
    public abstract class CPE 
    {

        public IEnumerable<CPE> GetAllDescendants()
        {
            IEnumerable<CPE> outp ;
            
            if (Children == null)
            {
                return new List<CPE>();
            }
            outp = Children.Aggregate<CPE,IEnumerable<CPE>>(Children, (acc, ch) => acc.Concat(ch.GetAllDescendants()));

            
            return outp;
        }
        public List<CPE> GetAllDescendantsOld()
        {
            List<CPE> outp = new List<CPE>();
            if (Children == null)
            {
                return outp;
            }
            foreach (var item in Children)
            {
                outp.AddRange(item.GetAllDescendants());
                outp.Add(item);
            }
            return outp;
        }


        /// <summary>
        /// recurse over ascendants to find the root, i,e the one with null Parent 
        /// implemented for no specific reason
        /// </summary>
        public CPE GetRout()
        {
            if (this.Parent == null) return this;
            else return Parent.GetRout();
        }

        public void spawnChildren()
        {

            foreach (var item in XElement.Nodes())
            {
                //doesnt include Item tag cz the ListParser class does child spawning by it's owwn, instead of calling this base ethode
                CPEElements type = Type(item);
                Type realType = Fucs.MapCPETypeEnumToType(type);
                if (realType == null)
                {
                    continue;
                }
                //Trace.WriteLineIf(false, realType.FullName, "spawnChildren: realType: ");

                CPE newChild = (CPE)Activator.CreateInstance(realType, (XElement)item, this);

                if (newChild == null)
                {
                    throw new Exception("child spawning resulted in null");
                    continue;
                }

                Children.Add(newChild);





            }
        }

        /// <summary>
        /// returns null if attribute doesnt exist
        /// </summary>
        public string getAttrib(string attribName)
        {
            return XElement.Attribute(attribName)?.Value;
        }

        /// <summary>
        /// returns bool indicating wether the attribute was found and outputed
        /// </summary>
        public bool TryGetAttrib(string attribName, out string maybeValue)
        {
            var xattr = XElement.Attribute(attribName);
            if (xattr != null) { maybeValue = xattr.Value; return true; }
            else { maybeValue = null; return false; }
        }

        /// <summary>
        /// causes all descendants to re apply(), resulting in a new data distrubution over the preset model, 
        /// this allows to re use the preset model to parse another string data, and usually only invoked by the root ListWatcher object 
        /// </summary>
        public virtual void Refresh(string injectInput = "")
        {
            Input = injectInput;
            if (string.IsNullOrEmpty(injectInput))
            {
                Input = Parent.Output;
            }

            apply();
            if(Children!=null)//todo: a more rbust approach would be using a cpe wide property: IsParentType meaning the class supports having children or not e;g targetProperty doesnt
            foreach (var item in Children)
            {
                item.Refresh();
            }
        }

      

        public static CPEElements Type(XElement xelem)
        {
            if (xelem.Name.Namespace == "html-parser") return CPEElements.HtmlParser;
            switch (xelem.Name.LocalName.ToLower())
            {
                case "wraper":
                    return CPEElements.Wraper;
                case "replacer":
                    return CPEElements.Replacer;
                case "matcher":
                    return CPEElements.Matcher;
                case "listparser":
                    return CPEElements.ListParser;
                case "item":
                    return CPEElements.Item;
                case "value":
                    return CPEElements.Value;
                case "group":
                    return CPEElements.Group;
                case "targetproperty":
                    return CPEElements.TargetProperty;
                case "tracer": return CPEElements.Tracer;
                case "attributevalue": return CPEElements.AttributeValue;
                case "innerhtml": return CPEElements.InnerHtml;
                case "htmltextnode": return CPEElements.HtmlTextNode;
                default:
                    return CPEElements.None;
            }

        }
        public static CPEElements Type(XNode xnode)
        {
            if (xnode.NodeType == XmlNodeType.Element)
            {
                return Type((XElement)xnode);
            }
            else
            {
                return CPEElements.None;
            }


        }
        public List<CPE> Children { get; set; }
        public XElement XElement { get; set; }

        public string Input { set; get; }
        public string Output { set; get; }
        public CPE Parent { set; get; }
        public virtual void apply()
        {
            Output = (Input);
        }

        internal virtual void closExp()
        {
            if(Children!=null)
            for (int i = 0; i < Children.Count; i++)
            {
               // Children[i] = null;
            }
        }
    }


    public class ListWatcherCPE : CPE
    {
        public string uri { get; set; }
        public string presetName { get; set; }

        public TimeSpan DefaultInterval { set; get; } = TimeSpan.FromMinutes(3);
        public string DefaultAction { set; get; }
        public string PopupWindowTitleFormatter { get; set; } = "$name : $c New Items";
        public string UnreadButtonCaptionFormatter { get; set; } = "$c News";

        private ListParserCPE listParserRef = null;
        public ListParserCPE ListParserRef
        {
            get
            {

                if (listParserRef != null)
                {
                    return listParserRef;
                }
                foreach (var item in this.GetAllDescendants())
                {
                    if (item.GetType() == typeof(ListParserCPE))
                    {
                        listParserRef = (ListParserCPE)item;
                        return listParserRef;
                    }
                }

                Debug.Assert(listParserRef != null, "Mi: ListWarcherTag object couldnt find the ListParser descendant ");
                return null;
            }
        }

        public ListWatcherCPE(XElement XELEM, string input)
        {

            string SupportedVersion = "2.0";
            Debug.WriteLineIf(false, "spawning ListWatcherTag");

            Children = new List<CPE>();
            XElement = XELEM;
            // assert verion compability
            string maybeVersion = "none";
            TryGetAttrib("ToolsVersion", out maybeVersion);
            if (maybeVersion != SupportedVersion)
            {
                throw new Exception($"Mi: cannot parse preset with ToolsVersion='{maybeVersion}', the supported versions are: {SupportedVersion} ");

            }


            // ## validating and parsing attributes ## //
            string maybeDefaultAction = getAttrib("defaultAction");
            if (maybeDefaultAction != null) DefaultAction = maybeDefaultAction;
            string maybeDefaultInterval;
            if (TryGetAttrib("defaultInterval", out maybeDefaultInterval))
            {
                var maybeDefaultInterval_ = Fucs.TimeSpanFromString(maybeDefaultInterval);
                Trace.Assert(maybeDefaultInterval_ != null, $"Mi defaultInterval attribute value '{maybeDefaultInterval}' is not a valid timeSpan value");
                if (maybeDefaultInterval_.HasValue)
                    DefaultInterval = maybeDefaultInterval_.Value;
            }
            string maybePPWT;
            if (TryGetAttrib("popupWindowTitle", out maybePPWT))
                PopupWindowTitleFormatter = maybePPWT;

            string maybeUBC;
            if (TryGetAttrib("unreadButtonCaption", out maybeUBC))
                UnreadButtonCaptionFormatter = maybeUBC;

            uri = XElement.Attribute("uri")?.Value;
            Trace.Assert(uri != null, XMLLW.MESSAGES.missingProp("ListWatcher", "uri"));
            presetName = XElement.Attribute("name")?.Value;
            Trace.Assert(uri != null, XMLLW.MESSAGES.missingProp("ListWatcher", "name"));

            Input = input;
            //Trace.WriteLineIf(false, input, "ListWatcherTag: Input: ");
            apply();
            //Trace.WriteLineIf(false, "applyied", "ListWatcherTag: ");

            base.spawnChildren();
            //Trace.WriteLineIf(false, "spawning ended", "ListWatcherTag: ");

        }


        public override void Refresh(string injectInput = "")
        {
            base.Refresh(injectInput);
            
            //Trace.WriteLineIf(false, Input, "ListWatcherTag Refresh: Input: ");
        }

    }


    public class WraperCPE : CPE
    {
        string From { get; set; }
        string To { get; set; }
        int Ignore { get; set; }

        public WraperCPE(XElement XELEM, CPE parent)
        {
            if (XELEM == null && parent == null)
            {
                return;
            }
            Debug.WriteLineIf(false, "spawning wraper");
            Children = new List<CPE>();
            XElement = XELEM;
            string from = (string)XElement.Attribute("from").Value;
            string to = (string)XElement.Attribute("to").Value;
            int ignor = int.Parse(XElement.Attribute("ignore").Value);

            From = from;
            To = to;
            Ignore = ignor;

            Parent = parent;
            Input = Parent.Output;
            apply();

        }



        public override void apply()
        {
            Output = WraperCPE.Wrap(Input, From, To, Ignore);
        }

        public static string Wrap(string input, string from, string to, int ignore)
        {
            Match m = Regex.Match(input, from + ".*?" + to);
            if (m.Success) return m.Value;
            else return "";
        }

    }

    public class ReplacerCPE : CPE
    {
        string Pattern { get; set; }
        string Replacement { get; set; }
        string UseConverter { get; set; }
        public string AppendAfter { get; set; }
        public string AppendBefore { get; set; }


        public ReplacerCPE(XElement XELEM, CPE parent)
        {

            Children = new List<CPE>();
            XElement = XELEM;


            AppendBefore = getAttrib("appendBefore");
            AppendAfter = getAttrib("appendAfter");

            var ReplacementAtt = XElement.Attribute("replacement");
            var PatternAtt = XElement.Attribute("pattern");
            var UseConverterAtt = XElement.Attribute("converter");
            Trace.Assert(ReplacementAtt != null, XMLLW.MESSAGES.missingProp("replacer", "pattern"));
            Trace.Assert(ReplacementAtt != null, XMLLW.MESSAGES.missingProp("replacer", "replacement"));
            Replacement = ReplacementAtt.Value;
            Pattern = PatternAtt.Value;
            UseConverter = UseConverterAtt == null ? "none" : UseConverterAtt.Value;

            Parent = parent;
            Input = Parent.Output;
            apply();
            base.spawnChildren();


        }

        public override void apply()
        {
            if (UseConverter.ToLower() == "xmldecoder")
            {
                Output = Fucs.decodeXml(Input);

            }
            else
            {
                Output = Regex.Replace(Input, Pattern, Replacement);

            }
            Output = AppendBefore + Output + AppendAfter;
        }



    }


    public class MatcherCPE : CPE, IMatcher
    {
        string Pattern { get; set; }
        string Options { get; set; }
        public string AppendAfter { get; set; }
        public string AppendBefore { get; set; }

        public Match MatcherOutput { set; get; }

        public bool Success { get { return MatcherOutput.Success; } }

        public GroupCollection Groups { get { return MatcherOutput.Groups; } }

        public string Value { get { return MatcherOutput.Value; } }

        internal override void closExp()
        {
            base.closExp();
            try
            {
                MatcherOutput = null;


            }
            catch
            {

            }
           

        }

        public MatcherCPE(XElement XELEM, CPE parent)
        {
            Debug.WriteLineIf(false, "spawning matcher");

            Children = new List<CPE>();
            XElement = XELEM;
            AppendBefore = getAttrib("appendBefore");
            AppendAfter = getAttrib("appendAfter");
            Pattern = XElement.Attribute("pattern")?.Value;
            Options = XElement.Attribute("options")?.Value;
            Debug.Assert(Pattern != null, XMLLW.MESSAGES.missingProp("matcher", "pattern"));

            Parent = parent;
            Input = Parent.Output;
            apply();
            base.spawnChildren();
        }

        public static RegexOptions parseOptions(string asStr)
        {
            switch (asStr)
            {
                case "Singleline": return RegexOptions.Singleline;
                default:
                    return RegexOptions.None;
            }
        }
        public override void apply()
        {
            MatcherOutput = Regex.Match(Input, Pattern, parseOptions(Options));
            Output = MatcherOutput.Success ? MatcherOutput.Value : "";
            Output = AppendBefore + Output + AppendAfter;
        }
    }


    public class ListParserCPE : CPE
    {




        private string ItemPattern { get; set; }
        private RegexOptions ItemPatternOptions { get; set; }
        private XElement ItemNode { get; set; }
        public List<Match> ChildrenFragments { get; private set; } = new List<Match>();

        public ICollection<DictBasedNewItemObject> GetParsedList()
        {
            var sw = Stopwatch.StartNew();
            var res = new Collection<DictBasedNewItemObject>(ChildrenFragments. Select((matchChiled) => {
                //injetc the one and only one item with the text for each fragmentChild
                //var sw2 = Stopwatch.StartNew();
                ((ItemParserCPE)Children[0]).RefreshItem(  matchChiled);
                //Trace.WriteLine($"RefreshItem :{sw2.Elapsed}");
                return new DictBasedNewItemObject()
                { dict = ((ItemParserCPE)Children[0]).makeDictionaryObject() };
            }).ToList());
            var elapsed = sw.Elapsed;
            Trace.WriteLine($"GetParsedList :{elapsed}");
            return res;
        }


        public ListParserCPE(XElement XELEM, CPE parent)
        {
            Debug.WriteLineIf(false, "spawning ListParser");

            Children = new List<CPE>();
            XElement = XELEM;



            Parent = parent;
            Input = Parent.Output;
            apply();

            ItemNode = (XElement)XElement.FirstNode;
            Debug.Assert((ItemNode != null) && (Type(ItemNode) == CPEElements.Item), "ListParser must have an Item element as fist child");

            ItemPatternOptions = MatcherCPE.parseOptions(ItemNode.Attribute("options")?.Value);
            ItemPattern = ItemNode.Attribute("pattern")?.Value;
            Debug.Assert(ItemPattern != null, "mi: Item must have a pattern attribute");



            spawnItems();

            Children.Add(new ItemParserCPE((XElement) XELEM.FirstNode, Regex.Match("y","y") , this));
        }
        

        public override void Refresh(string injectedInput = "")
        {
            Input = injectedInput;
            if (string.IsNullOrEmpty(injectedInput))
            {
                Input = Parent.Output;
            }
            apply();
            spawnItems();
            //new code is uneccessary , the new spawned objects will have an up to date data no refreshing needed
            return;
            foreach (var item in Children)
            {
                item.Refresh();
            }
        }

        /// <summary>
        /// should be recalled when data changes
        /// </summary>
        private void spawnItems()
        {
            MatchCollection mc = Regex.Matches(Input, ItemPattern, ItemPatternOptions);
            //Children.Clear();
            ChildrenFragments.Clear();
            foreach (Match oneMatch in mc)
            {
                ChildrenFragments
                 .Add(oneMatch);


            }
        }


    }





    public class ItemParserCPE : CPE, IMatcher
    {
        Match RawMatch { get; set; }

        public bool Success { get { return RawMatch.Success; } }
        public GroupCollection Groups { get { return RawMatch.Groups; } }
        public string Value { get { return RawMatch.Value; } }

        public Match MatcherOutput { set; get; }
        private Collection<CPE>  _targetPropertyDescendentElements =null;
        public Collection<CPE> TargetPropertyDescendentElements { get {
                if (_targetPropertyDescendentElements == null)
                {
                    _targetPropertyDescendentElements = new Collection<CPE>(GetAllDescendants().Where((CPEElem)=>CPEElem.GetType()==typeof(TargetPropertyCPE)).ToList());
                    
                }
                return _targetPropertyDescendentElements;

            } }
        static int cc2=0;
      
        public Dictionary<string,object> makeDictionaryObject()
        {
            var sw = Stopwatch.StartNew();
            Dictionary<string, object> dict = new Dictionary<string, object>(9);

            //todo: this probably should run synch to avoide late mutations

        
            for (int i = 0; i < TargetPropertyDescendentElements.Count; i++)
            {
                TargetPropertyCPE tp = (TargetPropertyCPE)TargetPropertyDescendentElements[i];
                dict.Add(tp.PropertyName, tp.Output);
                if (tp.UseAs != SpecialProps.none)
                {
                    dict.Add(tp.UseAs.ToString(), tp.Output);
                }
            }
            /*foreach (var item in TargetPropertyDescendentElements)
            {
                TargetPropertyCPE tp = (TargetPropertyCPE)item;
                dict.Add(tp.PropertyName, tp.Output);
                if (tp.UseAs != SpecialProps.none)
                {
                    dict.Add(tp.UseAs.ToString(), tp.Output);
                }
            }*/
            var elepsed = sw.Elapsed;
            if (cc2++ == 20)
            {
                Trace.WriteLine($"dict creation sync: {elepsed}");
            }
            return dict;
        }



        public ItemParserCPE(XElement XELEM, Match rawMatch, ListParserCPE parent)
        {
            Debug.WriteLineIf(false, "spawning ItemParser");

            RawMatch = rawMatch;
            Children = new List<CPE>();
            XElement = XELEM;


            Parent = parent;
            Input = RawMatch.Value;
            apply();
            base.spawnChildren();
        }
        public override void apply()
        {
            Output = Value;
        }

        internal override void closExp()
        {
            base.closExp();
            _targetPropertyDescendentElements = null;
            MatcherOutput = null;
            RawMatch = null;


        }

        public override void Refresh(string injectInput = "")
        {
            Input = injectInput;
            base.Refresh(injectInput);
        }
       
        public void RefreshItem(Match matchobj)
        {
            //unsafe code for performance sake, Input asigning is beleived to be unecessary
            Output = matchobj.Value;
            RawMatch = matchobj;

            //apply();
            foreach (var item in Children)
            {
                item.Refresh();
            }
            
        }

    }


    // most messy shit i've ever written
    public class HtmlParserCPE : CPE
    {

       
        /// <summary>
        /// target, usually html, element name, e.g div, span
        /// </summary>
        public string ElementName { set; get; }

        /// <summary>
        /// contains the raw inner string of the node
        /// </summary>
        public string InnerHtmlContent { get; set; }

        /// <summary>
        /// performance-crtical
        /// gets the first element that passes a set of attribute conditions and/or an index condition
        /// the indedx condition usage is as follow h:index="4", meaning the element must ba the 4th among it's siblings 
        /// </summary>
        static HtmlElement GetElement(HtmlElement InputElem, string targetElemName, IEnumerable<XAttribute> requiredAttributes)
        {
            var attribsAslist = requiredAttributes.ToList();
            var attribsASKeyPairs = attribsAslist.ConvertAll<KeyValuePair<string, string>>(new Converter<XAttribute, KeyValuePair<string, string>>((at) => new KeyValuePair<string, string>((at.Name.NamespaceName == "html-parser" ? "h:" : "") + at.Name.LocalName, at.Value)));
            
            return GetElement(InputElem, targetElemName, attribsASKeyPairs);
        }


        /// <summary>
        /// performance-critical
        /// returns the first child element that have all the required attribs with the requiredd values, 
        /// passing a null string as a required attib value will pass any value
        /// returns null if no match was found
        /// </summary>
        static HtmlElement GetElement(HtmlElement InputElem, string targetElemName, IEnumerable<KeyValuePair<string, string>> requiredAttributes)
        {
            IHtmlCollection<IElement> lst = InputElem.GetElementsByTagName(targetElemName);

            //List<IElement> lst = new List<IElement>(InputElem.GetElementsByTagName(targetElemName));
            //Debug.WriteLineIf(false, $"lst has {lst.Count()} elements");
            foreach (var item in lst)
            {
                bool passesAllConditions = true;
                foreach (var requiredAttrib in requiredAttributes)
                {
                    //### case of a special h:index=4 condition
                    if (requiredAttrib.Key == "h:index")
                    {
                        int requiredIndexValue = int.Parse(requiredAttrib.Value);
                        int actualIx = InputElem.GetElementsByTagName(item.TagName).Index(item);
                        if (actualIx != requiredIndexValue)
                        {
                            Debug.WriteLineIf(false, $"index of the element is actually {actualIx} , required ix is {requiredIndexValue} ");
                            passesAllConditions = false;
                            break;
                        }
                        continue;
                    }

                    //## case of a regular attribute=value condition
                    if (!item.HasAttribute(requiredAttrib.Key))// forgot that '!' and caused me a nasty bug
                    {
                        passesAllConditions = false;
                        break; // no need to keep cheking other requested attribs
                    }
                    if (requiredAttrib.Value != null)
                    {
                        if ((requiredAttrib.Value != "mi:any") && (item.GetAttribute(requiredAttrib.Key) != requiredAttrib.Value))
                        {
                            // break if element does not have the requird attrib at all
                            // break ig it have the required attib but with a different vale that the one required, 
                            // if the required value is null then this will not do the value checking
                            passesAllConditions = false;
                            break; // no need to keep cheking other requested attribs
                        }
                    }
                }
                if (passesAllConditions) { return item as HtmlElement; }
                // steps here when the item didsnt pass the tests, 
            }
            // steps here when none of the items if any passed the tests, 
            // returning null
            return null;

        }


        /// <summary>
        /// returns the index of an item whithin a parrent's child elements, or -1 if it doesnt exist among them 
        /// </summary>
        private static int GetIndexOf(IElement parentElem, IElement elem, bool sameLocalName = false)
        {
            //if (sameLocalName)

            /*  return parentElem.Elements(elem.Name.LocalName).ToList().IndexOf(elem);
          return parentElem.Elements().ToList().IndexOf(elem);*/
            return parentElem.GetElementsByTagName(elem.TagName).Index(elem);
        }

        public HtmlElement ParsedElement { get; set; }
        public bool Success { set; get; }
        public bool isParentAnXParser
        {
            get
            {
                return Parent.GetType() == typeof(HtmlParserCPE);
            }
        }
        public HtmlParserCPE(XElement XELEM, CPE parent)
        {
            Parent = parent;
            Input = Parent.Output;
            Debug.Assert(XELEM.Name.NamespaceName == "html-parser", "Mi: the passed xelement does not belong to the 'html-parser' namespace");
            XElement = XELEM;
            ElementName = XElement.Name.LocalName;
            Debug.WriteLineIf(false, "spawning XParserTag");
            Children = new List<CPE>();
            XElement = XELEM;
            apply();
            spawnChildren();
        }

       
        static IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        //performance-critical method
        public override void apply()
        {
            if (isParentAnXParser)
            {
                var parentAsXparser = (HtmlParserCPE)Parent;
                if (!parentAsXparser.Success)
                {
                    Debug.WriteLine($"XParserTag with elementName name:'{ElementName}' failed because it's XParserTag '{parentAsXparser.ElementName}' parent has failed");
                    Success = false;
                }
                else
                {
                    var parentsParsedElement = ((HtmlParserCPE)Parent).ParsedElement;
                    ParsedElement = GetElement(parentsParsedElement, this.ElementName, this.XElement.Attributes());
                    //string accumilaatedAttribes = XElement.Attributes().Aggregate("Attributes here: \n ", (prev, curr) => (prev + "\n" + curr.Name.NamespaceName + ":" + curr.Name.LocalName + " = " + curr.Value));
                    Success = ParsedElement != null;
                    if (Success == false)
                    {
                        Debug.WriteLine($"XParserTag with elementName name:'{ElementName}' failed because GetElement() didnt find any matching element, parent XParserTag is: '{parentAsXparser.ElementName}' \n it's outter is {parentAsXparser.ParsedElement.OuterHtml} ");
                    }
                }

            }
            else
            {
                try
                {
                    //case2: this is the root htmLParser
                    
                    //Parse the document from the content of a response to a virtual request
                    var document = context.OpenAsync(req => req.Content(Input))
                        . GetAwaiter().GetResult();
                    //Do something with document like the following
                    ParsedElement = (HtmlElement)document.All.FirstOrDefault((n) => (n.TagName.ToLower() == ElementName.ToLower()));
                    

                    /*
                    00:00:00.0001153
                    00:00:00.0002397
                    00:00:00.0000102
                    */

                    Success = true;
                    if (ParsedElement == null)
                    {
                        Success = false;
                        Debug.WriteLine($"Mi: XParserTag with name:'{ElementName}' failed because parsing it's input text returnd a null HtmlElement object");
                    }
                    else if (ParsedElement.TagName.ToLower() != ElementName.ToLower())
                    {
                        Success = false;
                        Debug.WriteLine($"Mi: XParserTag with name:'{ElementName}' failed because parsing it's input text returnd an HtmlElement with name '{ParsedElement.TagName}' when the name '{ElementName}' was excpected");
                    }

                }
                catch (Exception err)
                {
                    Success = false;
                    Debug.WriteLine($"Mi: XParserTag with name:'{ElementName}' failed to parse it's Input text : {err?.Message}");
                }
            }






            Output = Success ? ParsedElement.OuterHtml : "";
            InnerHtmlContent = Success ? ParsedElement.InnerHtml : "";



        }

        internal override void closExp()
        {
            base.closExp();
            if(ParsedElement!=null)
            ParsedElement = null;
            if (InnerHtmlContent != null)
                InnerHtmlContent = null;

        }

    }


    // can only appear as a XParserTag child
    // supplies children with the parent's InnertXMLSring 
    // if a 'Trim' attribut is present this performes a string.trim() before outputing text
    public class InnerHtmlCPE : CPE
    {
        public bool EnableTrimming { get; set; }

        public InnerHtmlCPE(XElement XELEM, CPE parent)
        {
            Debug.Assert(parent.GetType() == typeof(HtmlParserCPE), "Mi: InnerXML tag can only appear as a XParser child ");
            Children = new List<CPE>();
            XElement = XELEM;
            Parent = parent;
            EnableTrimming = XElement.Attribute("Trim") != null;
            var parentAsXParser = (HtmlParserCPE)parent;
            Input = parentAsXParser.InnerHtmlContent; // tofo: make this tag do the inner text exctracting operations instead of the XParser, for better debuging
            Debug.WriteLine($"spawning InnerXML, child of a '{parentAsXParser.ElementName}' XParser");
            apply();
            base.spawnChildren();
        }
        

        public override void apply()
        {
            Input = ((HtmlParserCPE)Parent).InnerHtmlContent;
            if(Input.Contains("Masters : R"))
            {
                Trace.WriteLine("here");
            }
            if (EnableTrimming)
                Output = Input.Trim();
            else
                Output = Input;
        }

    }



    // can only appear as a XParserTag child
    // supplies children with the parent's attribute value  
    // if a 'Trim' attribut is present this performes a string.trim() before outputing text
    public class AttributeValueCPE : CPE
    {
        public bool EnableTrimming { get; set; }
        public string AttributeName { get; internal set; }

        public AttributeValueCPE(XElement XELEM, CPE parent)
        {
            Children = new List<CPE>();
            XElement = XELEM;
            Parent = parent;
            Trace.Assert(parent.GetType() == typeof(HtmlParserCPE), "Mi: AttributeValue tag can only appear as a XParser child ");
            var parentAsXParser = (HtmlParserCPE)parent;
            Input = parent.Output;
            Debug.WriteLineIf(false, $"spawning AttributeValue, child of a '{parentAsXParser.ElementName}' XParser, target attribute is ");
            Trace.Assert((AttributeName = getAttrib("AttributeName")) != null, XMLLW.MESSAGES.missingProp("AttributeValue", "AttributeName"));
            EnableTrimming = XElement.Attribute("Trim") != null;
            apply();
            base.spawnChildren();
        }

        public override void apply()
        {
            var parentAsXParser = (HtmlParserCPE)Parent;
            string outString;

            if (parentAsXParser.Success)
            {
                bool existsAttrb = parentAsXParser.ParsedElement.HasAttribute(AttributeName);
                string attrValueMaybe = existsAttrb ? parentAsXParser.ParsedElement.GetAttribute(AttributeName) : null;
                if (attrValueMaybe == null)
                {
                    outString = "";
                    Debug.WriteLine($"Mi: AttributeValue tag couldnt find the requested attribute '{AttributeName}'");

                }
                else
                {
                    outString = attrValueMaybe;

                }
            }
            else /// (!parentAsXParser.Success)
            {
                outString = "";
                Debug.WriteLine($"Mi: AttributeValue tag returned empty string because its XParser '{parentAsXParser.ElementName}' parent has failed ");

            }
            if (EnableTrimming)
                Output = outString.Trim();
            else
                Output = outString;
        }

    }



    public class TargetPropertyCPE : CPE
    {
        public string PropertyName { get; internal set; }
        public string AppendAfter { get; set; }
        public string AppendBefore { get; set; }
        public SpecialProps UseAs { get; set; } = SpecialProps.none;



        public TargetPropertyCPE(XElement XELEM, CPE parent)
        {

            Debug.WriteLineIf(false, "spawning targetProperty");
            Children = null;
            XElement = XELEM;

            PropertyName = XELEM.Attribute("property")?.Value;
            Trace.Assert(PropertyName != null, XMLLW.MESSAGES.missingProp("TargetProperty", "property"));

            AppendBefore = getAttrib("appendBefore");
            AppendAfter = getAttrib("appendAfter");
            string UseAsStr_ = getAttrib("UseAs");
            if (UseAsStr_ != null)
            {
                UseAsStr_ = UseAsStr_.ToLower();
                switch (UseAsStr_)
                {
                    case "title": UseAs = SpecialProps.Title; break;
                    case "link": UseAs = SpecialProps.Link; break;
                    case "subtitle": UseAs = SpecialProps.SubTitle; break;
                    default: UseAs = SpecialProps.none; break;
                }
            }



            Parent = parent;

            Input = Parent.Output;
            apply();
        }


        public override void apply()
        {

            Output = AppendBefore + Input + AppendAfter;
        }

    }



    public class TracerCPE : CPE
    {
        public string Message { get; internal set; }

        public override void Refresh(string injectInput = "")
        {
            Input = Parent.Output;
            Trace.WriteLine("Message=" + Message, "tracer refresh");
            Trace.WriteLine("Inpute=" + Input, "tracer refresh");


        }

        public TracerCPE(XElement XELEM, CPE parent)
        {

            Children = null;

            XElement = XELEM;
            Message = XELEM.Attribute("message").Value;
            Parent = parent;
            Input = Parent.Output;
            Trace.WriteLine("Message=" + Message, "tracer");
            Trace.WriteLine("Inpute=" + Input, "tracer");

            apply();
        }


    }



    public class ValueCPE : CPE
    {
        public string PropertyName { get; internal set; }

        public ValueCPE(XElement XELEM, CPE parent)
        {
            Children = new List<CPE>();

            Debug.WriteLineIf(false, "spawning valueTag");
            //Trace.WriteLineIf(true, "spawning a valueTag");

            Debug.WriteLine(parent.GetType());

            Debug.Assert((parent.GetType() == typeof(MatcherCPE)) || (parent.GetType() == typeof(ItemParserCPE)), "Mi: a Value tag's parent must be either a Matcher or an Item");

            XElement = XELEM;
            Parent = parent;
            Input = ((IMatcher)Parent).Value;
            apply();
            base.spawnChildren();

        }

    }



    public class GroupCPE : CPE
    {

        public int Index { get; internal set; }

        public GroupCPE(XElement XELEM, CPE parent)
        {
            Debug.WriteLineIf(true, "spawning GroupTag");
            Children = new List<CPE>();

            Trace.Assert((parent.GetType() == typeof(ItemParserCPE)) || (parent.GetType() == typeof(MatcherCPE)), "Mi: a Group tag's parent must be either a Matcher or an Item");

            XElement = XELEM;
            Index = int.Parse(XElement.Attribute("index").Value);
            Debug.WriteLineIf(false, "GroupTag's index is:" + Index.ToString());
            //<td><a href=\"2019-03-26-08-33-33_58f37f00f409b98b8eda58634d6dbeadeec0340f\">2019-03-26-08-33-33_..&gt;</a></td><td align=\"right\">2019-03-26 09:33  </td><td align=\"right\">713K</td><td>&nbsp;</td></tr>\r
            //<a href=\"((.*)(\\..{3,5}))\">
            Parent = parent;
            if (((IMatcher)Parent).Groups.Count > Index)
            {
                string ct = ((MatcherCPE)Parent).Input;
                Debug.WriteLine("mi: a Group tag is pointing to an out of range index");
                Debug.WriteLine(ct);

            }

            Input = ((IMatcher)Parent).Groups[Index].Value;
            Debug.WriteLine($"goup: input is {Input}");
            apply();
            base.spawnChildren();

        }

    }


    /// <summary>
    /// can only appear as an HTMLPARSER element child
    /// add on 27-sept-2021
    /// </summary>
    public class HtmlTextNodeCPE : CPE
    {
        public bool EnableTrimming { get; set; }

        public int RequiredIndex { get; set; }
        public bool IsIndexRequired { get; private set; }

        public HtmlTextNodeCPE(XElement XELEM, CPE parent)
        {
            Children = new List<CPE>();
            XElement = XELEM;
            Parent = parent;
            Trace.Assert(parent.GetType() == typeof(HtmlParserCPE), "Mi: HtmlTextNodeCPE tag can only appear as a HtmlParser child ");

            var parentAsXParser = (HtmlParserCPE)parent;
            Input = parent.Output;
            Debug.WriteLineIf(true, $"spawning HtmlTextNodeCPE, child of a '{parentAsXParser.ElementName}' XParser, target attribute is ");

            string maybeIndex = getAttrib("index");
            if (string.IsNullOrWhiteSpace(maybeIndex) == false)
            {
                RequiredIndex = int.Parse(maybeIndex);
                IsIndexRequired = true;
            }

            EnableTrimming = XElement.Attribute("Trim") != null;




            apply();
            base.spawnChildren();

        }

        public override void apply()
        {
            Debug.WriteLineIf(true, "applying texnode");
            var parentAsXParser = (HtmlParserCPE)Parent;
            string outString;

            if (parentAsXParser.Success)
            {
                bool exisChildTextNodes = parentAsXParser.ParsedElement.HasTextNodes();

                if (exisChildTextNodes == false)
                {
                    outString = "";
                    Debug.WriteLine($"Mi: HtmlTextNode CPE couldnt find any textNodes in the parrent html elemnt  '{parentAsXParser.ParsedElement.TagName}'");

                }
                else
                {
                    var allTextnodes = parentAsXParser.ParsedElement.Children.Where((elem) => elem.NodeType == NodeType.Text);

                    string textNodeContent;
                    if (IsIndexRequired)
                    {
                        //specified index mode
                        Debug.WriteLineIf(false, "IsIndexRequired is tre");
                        Debug.WriteLineIf(true, $"all text nodes count is: {allTextnodes.Count()}");
                        Debug.WriteLineIf(false, $"required ix is: {RequiredIndex}");
                        Debug.WriteLineIf(true, $"parent type is: {parentAsXParser.ParsedElement.NodeType}");

                        var tagetTextNode = allTextnodes?.FirstOrDefault();
                        Debug.WriteLineIf(false, $"tagetTextNodeis: {tagetTextNode?.NodeType}");

                        textNodeContent = tagetTextNode?.TextContent;
                        outString = textNodeContent;

                        Debug.WriteLineIf(false, "textNodeContent");


                    }
                    else
                    {
                        //auto mode not supported
                        textNodeContent = allTextnodes?.FirstOrDefault()?.TextContent;
                        outString = textNodeContent;

                    }


                }
            }
            else /// (!parentAsXParser.Success)
            {
                outString = "";
                Debug.WriteLine($"Mi: HtmlTextNode CPE returned empty string because its htmlParser parent '{parentAsXParser.ElementName}' has failed ");

            }
            if (EnableTrimming)
                Output = outString.Trim();
            else
                Output = outString;
        }

    }





}
