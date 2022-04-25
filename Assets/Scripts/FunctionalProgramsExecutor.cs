using System;
using System.Collections.Generic;
using System.Linq;
using Catalogs;
using Constants;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class FunctionalProgramsExecutor
{
    private static FunctionalProgramsExecutor _instance;
    private Dictionary<string, object> _params;

    private FunctionalProgramsExecutor()
    {
        AssignParametersDict();
    }

    public static FunctionalProgramsExecutor Instance => _instance ??= new FunctionalProgramsExecutor();

    private void AssignParametersDict()
    {
        _params = new Dictionary<string, object>
        {
            {"True", true},
            {"False", false},
            {"None", null},
            {"CurrentTaskID", Context.Instance.CurrentTaskID},
            {"CurrentSubtaskID", Context.Instance.CurrentSubtaskID},
            {"CurrentInstructionOrder", Context.Instance.CurrentInstructionOrder},
            {"prev", Context.Instance.Prev},
            {"var1", Context.Instance.Var1},
            {"var2", Context.Instance.Var2},
            {"root", Context.Instance.Root},
            {"query", Context.Instance.Query}
        };
    }

    public string Execute(QueryMeta queryMeta)
    {
        string output;
        AssignQueryMeta(queryMeta);

        foreach (var program in Context.Instance.Programs)
        {
            AssignParametersDict();
            var components = program.Split(' ');
            var function = components[0];
            var functionParams = components.Skip(1).Take(components.Length - 1).ToArray();

            try
            {
                var functionCaller = Catalog.Instance.GetType().GetMethod(function);
                var objParams = GetFunctionParameters(functionParams);
                
                Context.Instance.Prev = functionCaller?.Invoke(Catalog.Instance, new object[]{objParams});
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
                output = "Error: Could not execute programs";
                return output;
            }
        }

        if (Context.Instance.Prev is Response)
        {
            var response = (Response) Context.Instance.Prev;
            var operation = response.operation;
            var objects = response.objects;
            var extra = response.extra;

            var parsedObjects = objects.ToArray().Select(obj => obj.name).ToList();
            var parsedExtra = new List<string>();
            foreach (var key in extra.Keys)
            {
                switch (key)
                {
                    case "degree": 
                        parsedExtra.Add(string.Format("by {0} degrees", extra[key]));
                        break;
                    case "axis": 
                        parsedExtra.Add(string.Format("in {0} axis", extra[key]));
                        break;
                    case "scale":
                        parsedExtra.Add(string.Format("by {0}", extra[key]));
                        break;
                }
            }
            
            var operationPart = HelperFunctions.GetOperationForResponse(operation["name"], operation.ContainsKey("extra") ? operation["extra"] : null);
            var objectsPart = string.Join(", ", parsedObjects);
            var extraPart = string.Join(" ", parsedExtra);
            output = operationPart + " " + objectsPart + " " + extraPart;
        }
        else
        {
            output = "None";
        }
        
        return output;
    }

    private void AssignQueryMeta(QueryMeta queryMeta)
    {
        Context.Instance.Query = queryMeta.Query;
        Context.Instance.Programs = queryMeta.Programs;
        Context.Instance.CurrentTaskID = Context.Instance.CurrentTaskID;
        Context.Instance.CurrentSubtaskID = Context.Instance.CurrentSubtaskID;
        Context.Instance.CurrentInstructionOrder = Context.Instance.CurrentInstructionOrder;
        Context.Instance.Prev = null;
        Context.Instance.Var1 = null;
        Context.Instance.Var2 = null;
    }

    private string GetFunctionParameters(string[] functionParams)
    {
        var parameters = new object[functionParams.Length];
        for (var i = 0; i < functionParams.Length; i++)
        {
            var param = functionParams[i];
            parameters[i] = param;
        }

        return string.Join(GeneralConstants.ArgsSeparator.ToString(), parameters);
    }
}