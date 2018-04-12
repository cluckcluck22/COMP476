using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

using Random = UnityEngine.Random;
using Coroutine = System.Collections.IEnumerator;
using BTCoroutine = System.Collections.Generic.IEnumerator<BTNodeResult>;

public sealed class BehaviorTree
{
    public BTNode rootNode { get; private set; }
    private MonoBehaviour parent;
    private UnityEngine.Coroutine coroutineHandle;

    public BehaviorTree(BTNode rootNode, MonoBehaviour parent)
    {
        this.rootNode = rootNode;
        this.parent = parent;
    }

    public BehaviorTree(UnityEngine.TextAsset xml, MonoBehaviour parent)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        settings.ConformanceLevel = ConformanceLevel.Fragment;
        settings.ValidationType = ValidationType.None;

        this.parent = parent;

        System.IO.StringReader stringReader = new System.IO.StringReader(xml.text);

        using (XmlReader reader = XmlReader.Create(stringReader, settings))
        {
            XDocument doc = XDocument.Load(reader);

            if (doc.Root.Name.ToString().ToLowerInvariant() != "behavior-tree")
                throw new XmlException("Behavior tree root node not found");

            if (doc.Root.Elements().Count() == 0)
                throw new XmlException("The behavior tree is empty!");

            if (doc.Root.Elements().Count() > 1)
                throw new XmlException("The behavior tree must have only one node at the top level");

            Dictionary<string, MethodInfo> complex, simple, condition;

            LoadParentLeafFunctions(out complex, out simple, out condition);

            this.rootNode = ParseXmlElement(doc.Root.Elements().First(), complex, simple, condition);
        }
    }

    public void Start()
    {
        this.coroutineHandle = parent.StartCoroutine(BehaviorCoroutine());
    }

    public void Stop()
    {
        parent.StopCoroutine(coroutineHandle);
    }

    private Coroutine BehaviorCoroutine()
    {
        while (true)
        {
            BTCoroutine btRoutine = rootNode.Procedure();

            do {
                yield return new WaitForFixedUpdate();
            } while (btRoutine.MoveNext());
        }
    }

    private void LoadParentLeafFunctions(
        out Dictionary<string, MethodInfo> complex,
        out Dictionary<string, MethodInfo> simple,
        out Dictionary<string, MethodInfo> condition)
    {
        complex = new Dictionary<string, MethodInfo>();
        simple = new Dictionary<string, MethodInfo>();
        condition = new Dictionary<string, MethodInfo>();

        Type parentType = parent.GetType();

        MethodInfo[] allMethods = parentType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

        foreach (MethodInfo method in allMethods)
        {
            object[] custAttrs = method.GetCustomAttributes(typeof(BTLeafAttribute), false);
            if (custAttrs == null || custAttrs.Length == 0)
                continue;

            BTLeafAttribute leafAttr = (BTLeafAttribute)custAttrs[0];

            string key = leafAttr.LeafName.Trim().ToLowerInvariant();

            if (method.ReturnType == typeof(bool) &&
                method.GetParameters().Count() != 0 &&
                method.GetParameters()[0].ParameterType == typeof(Scorer)) // condition
            {
                if (complex.ContainsKey(key))
                    throw new ArgumentException("Duplicate condition node name: '" + key + "'");

                condition[key] = method;
            }

            else if (method.ReturnType == typeof(bool)) // simple
            {
                if (simple.ContainsKey(key))
                    throw new ArgumentException("Duplicate simple node name: '" + key + "'");

                if (method.GetParameters().Length > 0)
                    throw new ArgumentException("Method '" + method.Name + "' cannot have parameters if it is marked as a simple node function");

                simple[key] = method;
            }
            else if (method.ReturnType == typeof(BTCoroutine)) // complex
            {
                if (complex.ContainsKey(key))
                    throw new ArgumentException("Duplicate complex node name: '" + key + "'");

                if (method.GetParameters().Length != 1)
                    throw new ArgumentException("Method '" + method.Name + "' must have a single Stopper parameter if it is a complex node function");

                complex[key] = method;
            }

            else
            {
                throw new ArgumentException("Method '" + method.Name + "' must return either bool or BTCoroutine");
            }
        }
    }

    private BTNode ParseXmlElement(XElement element, 
        Dictionary<string, MethodInfo> complexFunctions,
        Dictionary<string, MethodInfo> simpleFunctions,
        Dictionary<string, MethodInfo> conditionFunctions)
    {
        XElement[] children = (element.Elements() ?? new XElement[0]).ToArray();

        string nodeType = element.Name.ToString().ToLowerInvariant();
        XAttribute nameAttr = null;
        string name = null;

        XAttribute frequencyAttr = null;
        string frequency = null;

        XAttribute cooldownAttr = null;
        string cooldown = null;

        XAttribute switchThresholdAttr = null;
        string switchThreshold = null;

        switch (nodeType)
        {
            case "not":
                if (children.Length == 0)
                    throw new XmlException("The 'not' element must have one child");
                if (children.Length > 1)
                    throw new XmlException("The 'not' element must only have one child");

                return new BTNotNode(ParseXmlElement(children[0], complexFunctions, simpleFunctions, conditionFunctions));

            case "repeat_forever":
                if (children.Length == 0)
                    throw new XmlException("The 'not' element must have one child");
                if (children.Length > 1)
                    throw new XmlException("The 'not' element must only have one child");

                return new BTRepeat(BTRepeatTypes.Forever, ParseXmlElement(children[0], complexFunctions, simpleFunctions, conditionFunctions));

            case "repeat_success":
                if (children.Length == 0)
                    throw new XmlException("The 'not' element must have one child");
                if (children.Length > 1)
                    throw new XmlException("The 'not' element must only have one child");

                return new BTRepeat(BTRepeatTypes.UntilSuccess, ParseXmlElement(children[0], complexFunctions, simpleFunctions, conditionFunctions));

            case "repeat_failure":
                if (children.Length == 0)
                    throw new XmlException("The 'not' element must have one child");
                if (children.Length > 1)
                    throw new XmlException("The 'not' element must only have one child");

                return new BTRepeat(BTRepeatTypes.UntilFailure, ParseXmlElement(children[0], complexFunctions, simpleFunctions, conditionFunctions));

            case "sequence":
                if (children.Length == 0)
                    throw new XmlException("The 'sequence' element must have children");

                return new BTSequence(children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));

            case "selector":
                if (children.Length == 0)
                    throw new XmlException("The 'selector' element must have children");

                return new BTSelector(children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));

            case "decision":
                if (children.Length == 0 || children.Length % 2 != 0)
                    throw new XmlException("The 'decision' element must have {condition, action} pairs of children");

                frequencyAttr = element.Attribute("frequency");

                if (frequencyAttr != null)
                    frequency = (frequencyAttr.Value ?? "").Trim().ToLowerInvariant();
                else if (frequencyAttr == null || frequencyAttr.Value == null)
                    frequency = "";

                switchThresholdAttr = element.Attribute("switch");

                if (switchThresholdAttr != null)
                    switchThreshold = (switchThresholdAttr.Value ?? "").Trim().ToLowerInvariant();
                else if (switchThresholdAttr == null || switchThresholdAttr.Value == null)
                    switchThreshold = "";

                return new BTDecision(frequency, switchThreshold, children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));


            case "simple":
                if (children.Length > 0)
                    throw new XmlException("The 'simple leaf' element cannot have children");

                nameAttr = element.Attribute("name");

                if (nameAttr == null)
                    throw new XmlException("Missing 'name' attribute for 'simple leaf' element");

                name = (nameAttr.Value ?? "").Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(name))
                    throw new XmlException("The'name' attribute for the 'simple leaf' element was not given a value");

                if (!simpleFunctions.ContainsKey(name))
                    throw new XmlException("Simple Leaf not found: '" + name + "'");

                return new BTSimpleLeaf(() => (bool)simpleFunctions[name].Invoke(parent, null));

            case "parallel_first":
                if (children.Length <= 1)
                    throw new XmlException("The 'parallel' element should have at least 2 children");

                return new BTParallel(BTParallelTypes.FirstReturn, children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));

            case "parallel_success":
                if (children.Length <= 1)
                    throw new XmlException("The 'parallel' element should have at least 2 children");

                return new BTParallel(BTParallelTypes.FirstSuccess, children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));

            case "parallel_failure":
                if (children.Length <= 1)
                    throw new XmlException("The 'parallel' element should have at least 2 children");

                return new BTParallel(BTParallelTypes.FirstFailure, children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));

            case "parallel_all":
                if (children.Length <= 1)
                    throw new XmlException("The 'parallel' element should have at least 2 children");

                return new BTParallel(BTParallelTypes.AllReturn, children.Select(elem => ParseXmlElement(elem, complexFunctions, simpleFunctions, conditionFunctions)));

            case "condition":
                if (children.Length > 0)
                    throw new XmlException("The 'condition' element cannot have children");

                nameAttr = element.Attribute("name");

                if (nameAttr == null || nameAttr.Value == null)
                    throw new XmlException("Missing 'name' attribute for 'condition leaf' element");

                name = (nameAttr.Value ?? "").Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(name))
                    throw new XmlException("The'name' attribute for the 'condition leaf' element was not given a value");

                if (!conditionFunctions.ContainsKey(name))
                    throw new XmlException("Condition Leaf not found: '" + name + "'");

                cooldownAttr = element.Attribute("cooldown");

                if (cooldownAttr != null)
                    cooldown = (cooldownAttr.Value ?? "").Trim().ToLowerInvariant();
                else if (cooldownAttr == null || cooldownAttr.Value == null)
                    cooldown = "";

                BTConditionLeaf conditionLeaf = new BTConditionLeaf(cooldown);
                conditionLeaf.AttachHandler((Scorer) => (bool)conditionFunctions[name].Invoke(parent, new object[] { conditionLeaf.GetScorer() }));
                return conditionLeaf;


            case "complex":
                if (children.Length > 0)
                    throw new XmlException("The 'complex leaf' element cannot have children");

                nameAttr = element.Attribute("name");

                if (nameAttr == null || nameAttr.Value == null)
                    throw new XmlException("Missing 'name' attribute for 'complex leaf' element");

                name = (nameAttr.Value ?? "").Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(name))
                    throw new XmlException("The'name' attribute for the 'complex leaf' element was not given a value");

                if (!complexFunctions.ContainsKey(name))
                    throw new XmlException("Complex Leaf not found: '" + name + "'");

                BTComplexLeaf complexLeaf = new BTComplexLeaf();
                complexLeaf.AttachHandler((Stopper) => (BTCoroutine)complexFunctions[name].Invoke(parent, new object[] { complexLeaf.GetStopper() }));
                return complexLeaf;

            default:
                throw new XmlException("Unknown behavior tree node type: '" + nodeType);

        }
    }
}
