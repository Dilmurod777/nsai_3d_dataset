using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Sprites;
using UnityEngine;

public class FunctionalProgramsExecutor
{
    private Dictionary<string, object> _params;
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
            {"root", Context.Instance.Root}
        };
        
    }

    private FunctionalProgramsExecutor()
    {
        AssignParametersDict();
    }  
    private static FunctionalProgramsExecutor _instance;  
    
    public static FunctionalProgramsExecutor Instance => _instance ??= new FunctionalProgramsExecutor();

    public string Execute(QueryMeta queryMeta)
    {
        var output = "Result: ";
        AssignQueryMeta(queryMeta);

        foreach(var program in Context.Instance.Programs)
        {
            var components = program.Split(' ');
            var function = components[0];
            var functionParams = components.Skip(1).Take(components.Length - 1).ToArray();

            try
            {
                var functionCaller = Catalog.Instance.GetType().GetMethod(function);
                var objParams = GetFunctionParameters(functionParams);
                
                Context.Instance.Prev = functionCaller?.Invoke(Catalog.Instance, objParams);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                output = "Error: Could not execute programs";
                return output;
            }
        }
        output = Context.Instance.Prev?.ToString();
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
    private object[] GetFunctionParameters(string[] functionParams)
    {
        var parameters = new object[functionParams.Length];
        for(var i = 0; i < functionParams.Length; i++)
        {
            var param = functionParams[i];
            if (_params.Keys.Contains(param))
            {
                parameters[i] = _params[param];
            }
            else
            {
                parameters[i] = param;
            }
        }
        return parameters;
    }
}